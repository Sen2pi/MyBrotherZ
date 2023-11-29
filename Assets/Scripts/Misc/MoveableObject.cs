using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private bool playerInsideZone = false;

    void Start()
    {
        // Get the Rigidbody2D component attached to the object
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (playerInsideZone)
        {
            // Get player input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate the movement direction
            Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;

            // Move the object using physics-based movement (Rigidbody2D)
            rb.velocity = movement * moveSpeed;
        }
        else
        {
            // If the player is not inside the zone, stop the movement
            rb.velocity = Vector2.zero;
        }
    }

    // This function is called when the player enters the collider zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideZone = true;
        }
    }

    // This function is called when the player exits the collider zone
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideZone = false;
            rb.velocity = Vector2.zero; // Stop the movement when the player exits the zone
        }
    }
}