using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    
    [SerializeField] private  int damage;
    [SerializeField] private float effectTime;
    [SerializeField] private GameObject afterEffect;
    public bool isColliding { get; set; } = false;
    private Vector3 collPos;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        collPos = other.GetComponent<Transform>().position;
        isColliding = true;
        if (other.GetComponent<EnemyHealth>())
        {
            transform.position = other.GetComponent<EnemyHealth>().transform.position;
            GetComponent<CapsuleCollider2D>().enabled = false;
            if (this.transform.childCount > 1)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                gameObject.transform.GetChild(1).gameObject.SetActive(false);

            }

            other.GetComponent<EnemyHealth>().TakeDamage(damage);
            StopAllCoroutines();
            isColliding = false;
            StartCoroutine(ColliderEffectRoutine()); 
        }
        else
        {
            this.gameObject.layer = 3;
        }
    }

    private IEnumerator ColliderEffectRoutine()
    {
        
        this.gameObject.transform.GetChild(transform.childCount-1).gameObject.SetActive(true);
        yield return new WaitForSeconds(effectTime);
        Instantiate(afterEffect, collPos, Quaternion.identity);
        Destroy(gameObject);
        
    }
}
