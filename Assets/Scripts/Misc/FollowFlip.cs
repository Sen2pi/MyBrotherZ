using UnityEngine;

public class FollowFlip : MonoBehaviour
{
    private Transform parentTransform;
    private Vector3 initialLocalPosition;
    private PlayerController playerController;
    [SerializeField] private float ajustYAxis = 1f;

    private void Start()
    {
        // Get a reference to the parent's transform
        parentTransform = transform.parent;
        // Store the initial local position of the child object
        initialLocalPosition = transform.localPosition;
        // Get a reference to the PlayerController component
        playerController = parentTransform.GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        // Update the facing direction based on the player input
        Vector2 facingDirection = playerController.GetFacingDirection();

        // Check if the player is facing right
        bool facingRight = playerController.FacingRight;

        // Check if the parent's local scale has changed in X
        if (facingRight)
        {
            // If the player is facing left, flip the local position of the child object
            Vector3 flippedPosition = new Vector3(-initialLocalPosition.x, initialLocalPosition.y, initialLocalPosition.z);
            transform.localPosition = flippedPosition + new Vector3(0f, ajustYAxis, 0f); // Adjust the Y position here
        }
        else
        {
            // If the player is facing right, apply the initial local position to the child
            transform.localPosition = initialLocalPosition + new Vector3(0f, ajustYAxis, 0f); // Adjust the Y position here
        }
    }
}