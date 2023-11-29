using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spit : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject spitProjectilePrefab;

    private Animator animator;
    private SpriteRenderer sprite;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }
    public void Attack() {
        animator.SetTrigger(ATTACK_HASH);
        if (transform.position.x - PlayerController.Instance.transform.position.x < 0)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }
    }

    public int GetDamageAmount()
    {
        return 1;
    }

    public void SpawnProjectileAnimEvent()
    {
        var newProjectile = Instantiate(spitProjectilePrefab, transform.position, Quaternion.identity);
        newProjectile.transform.SetParent(this.transform); 
    }
}
