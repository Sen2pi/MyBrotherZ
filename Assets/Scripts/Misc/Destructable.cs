using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private GameObject destroyVFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Damage>())
        {
            PickUpSpawner pickupSpawner = GetComponent<PickUpSpawner>();
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
            pickupSpawner?.DropRandomItems();
            Destroy(gameObject);
        }
    }
}
