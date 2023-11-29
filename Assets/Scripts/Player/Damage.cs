using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private int damage;
    private void Start()
    {
        MonoBehaviour currentActiveWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
        //damage = (currentActiveWeapon as IWeapon).GetWeaponInfo().weaponDamage;
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        enemyHealth?.TakeDamage(damage);
        
    }
    
}
