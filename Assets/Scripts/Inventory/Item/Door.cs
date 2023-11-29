using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]private Sprite closedSprite;
    [SerializeField]private Sprite openSprite;
    [SerializeField]private bool isLocked = false;
    [SerializeField]private Consumable requiredKey ; // Adjust this key name based on your game's requirements

    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;
    private int count = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedSprite;
    }

    private void Update()
    {
        if (isLocked)
        {
            // Check if the player has the key in their inventory
            bool hasKey = true;

            for (int i = 0; i < InventoryManager.Instance.Items.Length; i++)
            {
                if (InventoryManager.Instance.Items[i].Item.name == requiredKey.name)
                {
                    hasKey = true;
                    count = i;
                }
                else
                {
                    hasKey = false;
                }
            }
            if (hasKey && !isOpen)
            {
                UnlockDoor();
            }
            else if (!hasKey && isOpen)
            {
                LockDoor();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Automatically open the door when the player enters the trigger area
        if (collision.CompareTag("Player"))
        {
            if (!isLocked || InventoryManager.Instance.Items[count].Item.name == requiredKey.name)
            {
                if (!isOpen)
                {
                    OpenDoor();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Close the door when the player exits the trigger area
        if (collision.CompareTag("Player"))
        {
            if (!isOpen)
            {
                CloseDoor();
            }
        }
    }

    private void OpenDoor()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        isOpen = true;
        spriteRenderer.sprite = openSprite;
        // Implement any other logic, such as allowing player movement through the door.
    }

    private void CloseDoor()
    {
        GetComponent<CapsuleCollider2D>().enabled = true;
        isOpen = false;
        spriteRenderer.sprite = closedSprite;
        // Implement any other logic, such as preventing player movement through the door.
    }

    private void LockDoor()
    {
        //isLocked = true;
        // Optionally, you can change the door's appearance to indicate that it's locked.
    }

    private void UnlockDoor()
    {
        isLocked = false;
        InventoryManager.Instance.Items[count].Item.Use(PlayerController.Instance);
        // Optionally, you can change the door's appearance to indicate that it's unlocked.
    }
}
