using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Vector2 moveDir;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;
    private bool isMoving;

    private  void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        if (moveDir.x < 0)
        {
            sprite.flipX = true;
        }
        else if (moveDir.x > 0)
        {
            sprite.flipX = false;
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        isMoving = true;
        animator.SetBool("isWalking", true);
        moveDir = (targetPosition - (Vector2)transform.position).normalized;
    }

    public void StopMoving()
    {
        isMoving = false;
        animator.SetBool("isWalking", false);
        moveDir = Vector3.zero;
    }
}