using System.Collections;
using System.Collections.Generic;
using MyBrotherZ.Dialogue;
using UnityEngine;

public class NPCAI : MonoBehaviour
{
    [SerializeField] private float wanderRange = 5f; // Maximum distance NPC can wander from its initial position
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float idleTime = 2f; // Time NPC waits before selecting a new destination
    private Vector3 initialPosition;
    private Vector3 currentDestination;
    private float idleTimer;
    private bool isWandering = false;

    private Animator animator; // Reference to the Animator component

    private void Start()
    {
        initialPosition = transform.position;
        SelectRandomDestination();

        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isWandering)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                SelectRandomDestination();
                idleTimer = 0f;
            }
        }
        else if(enabled)
        {
            // Move towards the current destination
            Vector3 direction = (currentDestination - transform.position).normalized;
            transform.position += direction * wanderSpeed * Time.deltaTime;

            // Check if reached the destination
            float distanceToDestination = Vector3.Distance(transform.position, currentDestination);
            if (distanceToDestination <= 0.1f)
            {
                isWandering = false;
            }
        }

        // Calculate moveX and moveY based on the normalized movement direction
        Vector3 movement = (transform.position - initialPosition).normalized;
        float moveX = movement.x;
        float moveY = movement.y;

        // Update Animator parameters
        animator.SetFloat("moveX", moveX);
        animator.SetFloat("moveY", moveY);

        // Set isWalking parameter based on movement
        animator.SetBool("isWalking", isWandering);
    }

    public void StartConversation()
    {
        // Toggle the NPCAI component
        enabled = !enabled;
    }

    private void SelectRandomDestination()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRange;
        currentDestination = initialPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
        isWandering = true;
    }
}
