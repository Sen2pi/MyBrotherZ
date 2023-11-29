using System;
using UnityEngine;

public class ChainCollisionHandler : Singleton<ChainCollisionHandler>
{
    private CapsuleCollider2D myCollider;
    private Transform allyTransform;
    private Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        myCollider = GetComponent<CapsuleCollider2D>();
        allyTransform = transform.parent;
        playerTransform = GameObject.FindWithTag("Player").transform; // Adjust the tag or reference accordingly
    }

    private void Update()
    {
        UpdateColliderSizeAndRotation();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AllyAI>() || other.GetComponent<PlayerController>())
        {
            myCollider.isTrigger = true; // Enable trigger behavior
        }
        else
        {
            myCollider.isTrigger = false; // Disable trigger behavior
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        myCollider.isTrigger = true; // Enable trigger behavior
    }

    private void UpdateColliderSizeAndRotation()
    {
        // Calculate the distance and angle between ally and player
        Vector2 chainDirection = playerTransform.position - allyTransform.position;
        float chainDistance = chainDirection.magnitude;
        float chainAngle = Mathf.Atan2(chainDirection.y, chainDirection.x) * Mathf.Rad2Deg;

        // Update the capsule collider size to match the chain distance
        myCollider.size = new Vector2(chainDistance, myCollider.size.y);

        // Update the rotation of the capsule collider to match the chain direction
        transform.rotation = Quaternion.Euler(0f, 0f, chainAngle);
    }
}