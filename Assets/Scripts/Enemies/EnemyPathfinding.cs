using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float roamSpeed = 1.5f; // Speed when roaming
    private Transform playerTransform;
    private Vector2 moveDir;
    private Rigidbody2D rb;
    private KnockBack knockBack;
    private SpriteRenderer sprite;
    private Animator animator;
    private bool isRoaming = false; // Flag to determine if the enemy is currently roaming
    private Vector2 roamTarget; // Target position for roaming

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        knockBack = GetComponent<KnockBack>();
        rb = GetComponent<Rigidbody2D>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!knockBack.GetingKnockedBack)
        {
            if (ShouldChasePlayer())
            {
                isRoaming = false;
                MoveTo(playerTransform.position, moveSpeed);
            }
            else if (!isRoaming)
            {
                isRoaming = true;
                CalculateRoamTarget();
            }
        }

        animator.SetFloat("moveX", moveDir.x);
        animator.SetFloat("moveY", moveDir.y);

        if (!knockBack.GetingKnockedBack)
        {
            if (isRoaming)
            {
                MoveTo(roamTarget, roamSpeed);
            }
            else
            {
                StopMoving();
            }

            if (moveDir.x < 0)
            {
                sprite.flipX = true;
            }
            else if (moveDir.x > 0)
            {
                sprite.flipX = false;
            }
        }
    }

    public bool ShouldChasePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= detectionRange;
    }

    public void CalculateRoamTarget()
    {
        // Calculate a random position within a certain range for roaming
        roamTarget = (Vector2)transform.position + new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }

    public void MoveTo(Vector2 targetPosition, float speed)
    {
        animator.SetBool("isWalking", true);
        moveDir = (targetPosition - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + moveDir * (speed * Time.deltaTime));
    }

    public void StopMoving()
    {
        animator.SetBool("isWalking", false);
        moveDir = Vector2.zero;
    }
}
