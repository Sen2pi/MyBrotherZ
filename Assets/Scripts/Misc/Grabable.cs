using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Grabable : Singleton<Grabable>
{

    [SerializeField] private Transform holdSpot;
    [SerializeField] private LayerMask pickMask;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private float throwDistance = 10f;
    [SerializeField] private float throwSpeed = 0.02f;
    private Vector3 endPoint;
    public bool isThrowing = false;
    public Vector3 Direction { get; set; }

    private GameObject itemHolding;

    private void Update()
    {
        // Get the mouse position in the world
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Calculate the direction from the player's position to the mouse position
        Direction = (mousePosition - transform.position).normalized;

        // Optionally, you can visualize the direction by drawing a line from the player's position towards the mouse position:
        Debug.DrawRay(transform.position, Direction, Color.red);
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemHolding)
            {
                itemHolding.transform.position = transform.position + Direction;
                itemHolding.transform.parent = null;
                if (itemHolding.GetComponent<Rigidbody2D>())
                {
                    itemHolding.GetComponent<Rigidbody2D>().simulated = true;
                }
                transform.GetComponent<Animator>().SetBool("isHolding",false);
                itemHolding = null;
                
            }
            else
            {
                isThrowing = false;
                Collider2D pickUpItem = Physics2D.OverlapCircle(transform.position +Direction,.4f,pickMask);
                if (pickUpItem)
                {
                    itemHolding = pickUpItem.gameObject;
                    transform.GetComponent<Animator>().SetBool("isHolding",true);
                    itemHolding.transform.position = holdSpot.position;
                    itemHolding.transform.parent = holdSpot.transform;
                    if (itemHolding.GetComponent<Rigidbody2D>())
                    {
                        itemHolding.GetComponent<Rigidbody2D>().simulated = false;
                    }
                }
            }
        }
        if (Input.GetKeyDown((KeyCode.Q)))
        {   
            transform.GetComponent<Animator>().SetBool("isHolding",false);
            if (itemHolding)
            {
                isThrowing = true;
                StartCoroutine(ThrowItem(itemHolding));
                itemHolding = null;
            }
            
        }

    }
    
    private IEnumerator ThrowItem(GameObject item)
    {
        transform.GetComponent<Animator>().SetTrigger("Throw");
        item.layer = 0;
        Rigidbody2D itemRigidbody = item.GetComponent<Rigidbody2D>();
        if (!itemRigidbody)
        {
            itemRigidbody = item.AddComponent<Rigidbody2D>();
        }

        // Make sure the item is not a child of the player during the throw
        item.transform.parent = null;

        // Calculate the end position based on the throw distance and direction
        Vector3 endPoint = transform.position + Direction * throwDistance;

        // Calculate the throwing force
        Vector2 throwingForce = (endPoint - item.transform.position) / throwSpeed;

        // Apply the force to the item
        itemRigidbody.AddForce(throwingForce, ForceMode2D.Impulse);

        // Wait for the throw to complete
        yield return new WaitForSeconds(throwSpeed);

        // Reset the item's properties after the throw is complete
        if (item.GetComponent<Rigidbody2D>())
        {
            item.GetComponent<Rigidbody2D>().simulated = true;
        }

        isThrowing = false;

    }
}
