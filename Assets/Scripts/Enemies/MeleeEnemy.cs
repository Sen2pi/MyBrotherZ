using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float attackCooldown = 2f;

    private Animator animator;
    private SpriteRenderer sprite;
    private Transform playerTransform;
    private bool canAttack = true;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }
        
    }

    private void Update()
    {
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }
        
        // Check if the player is within attack range
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && canAttack)
        {
            Attack();
        }
    }

    public void Attack()
    {
        StartCoroutine(AttackCooldown());
        animator.SetTrigger(ATTACK_HASH);

        // Flip the enemy sprite based on the player's position
        if (transform.position.x - playerTransform.position.x < 0)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }

        // Perform attack logic to deal damage to the player
        PlayerHealth.Instance.TakeDamage(GetDamageAmount(), transform);

    }

    public int GetDamageAmount()
    {
        return attackDamage;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}