using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private GameObject boomerangProjectionShadow;
    [SerializeField] private GameObject boomerangSplatterPrefab;
    [SerializeField] private int damageAmount = 1; // Adjust the damage amount as needed
    [SerializeField] private float trailInterval = 0.2f; // Interval for spawning trail splatters

    private Vector3 startPosition;
    private Vector3 playerPosition;
    private Vector3 throwerPosition; // Store the position of the object that throws the boomerang
    private GameObject boomerangShadow;
    private bool returning = false;
    private bool hitPlayer = false;

    private void Start()
    {
        startPosition = GetComponentInParent<Transform>().position;
        playerPosition = PlayerController.Instance.transform.position;
        throwerPosition = GetComponentInParent<Transform>().position; // Store the initial position of the parent (thrower)
        boomerangShadow = Instantiate(boomerangProjectionShadow, startPosition + new Vector3(0, -0.3f, 0), Quaternion.identity);

        Vector3 shadowTargetPosition = playerPosition; // Initially, the shadow goes towards the player
        Vector3 boomerangShadowStartPosition = boomerangShadow.transform.position;

        StartCoroutine(ProjectileCurveRoutine(startPosition, playerPosition, shadowTargetPosition));
        StartCoroutine(MoveBoomerangShadowRoutine(boomerangShadow, boomerangShadowStartPosition, shadowTargetPosition));
    }

    private void Update()
    {
        throwerPosition = GetComponentInParent<Transform>().position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to the player
            if (PlayerHealth.Instance.CurrentHealth != 0)
            {
                PlayerHealth.Instance.TakeDamage(damageAmount, transform);
            }
        }
    }

    private IEnumerator ProjectileCurveRoutine(Vector3 start, Vector3 end, Vector3 shadowTarget)
    {
        float timePassed = 0f;
        float trailTimer = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            trailTimer += Time.deltaTime;

            float linearT = timePassed / duration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            Vector3 targetPosition;
            Vector3 targetShadow;

            // Determine the target position and shadow target based on whether the boomerang is returning or not
            if (!returning)
            {
                targetPosition = end;
                targetShadow = playerPosition; // Shadow goes towards the player when the boomerang is thrown
            }
            else
            {
                targetPosition = startPosition;
                targetShadow = throwerPosition; // Shadow goes towards the starting position when the boomerang returns
            }

            // Move the boomerang towards the target position
            transform.position = Vector2.Lerp(start, targetPosition, linearT) + new Vector2(0f, height);

            // Check if the boomerang has reached the player, and if yes, start returning to the enemy
            if (!hitPlayer && Vector2.Distance(transform.position, playerPosition) < 0.1f)
            {
                hitPlayer = true;
                timePassed = 0f; // Reset the time for the return journey
            }

            // Check if the boomerang has reached the end of its throw and should start returning
            if (!returning && hitPlayer && Vector2.Distance(transform.position, end) < 0.1f)
            {
                returning = true;
                timePassed = 0f; // Reset the time for the return journey
            }

            // Instantiate the trail splatter prefab at regular intervals
            if (trailTimer >= trailInterval)
            {
                Instantiate(boomerangSplatterPrefab, transform.position, Quaternion.identity);
                trailTimer = 0f;
            }

            yield return null;
        }

        if (!returning)
        {
            // Switch the boomerang's direction to return
            returning = true;
            StartCoroutine(ReturnToStartRoutine());
        }
        else
        {
            // Boomerang has returned to the thrower, destroy it and create splatter
            Destroy(gameObject);
        }
    }

    private IEnumerator MoveBoomerangShadowRoutine(GameObject boomerangShadow, Vector3 start, Vector3 end)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            boomerangShadow.transform.position = Vector2.Lerp(start, end, linearT);
            yield return null;
        }

        // If the boomerang has returned, destroy the shadow as well
        if (returning)
            Destroy(boomerangShadow);
    }

    private IEnumerator ReturnToStartRoutine()
    {
        // Wait for a short delay before starting the return animation
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ProjectileCurveRoutine(transform.position, throwerPosition, throwerPosition));
    }
}
