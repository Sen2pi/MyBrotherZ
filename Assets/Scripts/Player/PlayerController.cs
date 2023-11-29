
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using MyBrotherZ.Dialogue;

public class PlayerController : Singleton<PlayerController>, IDataPersisence
{
    
    public bool FacingRight { get => facingRight; set => facingRight = value; }
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform weaponCollider;
   
    //PauseMenu
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Text saveText;
    [SerializeField] private Button loadButton;
    [SerializeField] private TMP_Text loadText;
    [SerializeField] private Button newGameButton;
    [SerializeField] private TMP_Text newGameText;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private TMP_Text ContinueText;
    [SerializeField] private float maxChainDistance = 10f;
    [SerializeField] private Transform allyPosition;

    private GameObject slash;
    private KnockBack knockBack;
    private float startingMoveSpeed;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private bool facingRight = false;
    private bool isDashing = false;
    private Grabable grabbedObject;
    private Vector2 facingDirection = Vector2.right;
    private SpriteRenderer mySpriteRenderer;
    
    private PlayerControls playerControls;
    
    protected override void Awake()
    {
        base.Awake();
        slash = transform.GetChild(6).gameObject;
        knockBack = GetComponent<KnockBack>();
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        playerControls.Inventory.Open.performed += _ => OpenInventory();
        playerControls.Inventory.HotBar.performed += _ => UseItem();
        playerControls.Inventory.OpenCraft.performed += _ => OpenCraftInventory();
        playerControls.Pause.PauseMenu.performed += _ => OpenPauseMenu();
        startingMoveSpeed = moveSpeed;

        ActiveInventory.Instance.EquipStartingWeapon();
    }

 

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Update()
    {
        PlayerInput();
        if (PlayerController.Instance.IsHoldingObject())
        {
            weaponCollider.gameObject.SetActive(false);
        }

        if (!ActiveWeapon.Instance.isAttacking)
        {
            weaponCollider.gameObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {

        if (movement.x != 0 || movement.y != 0)
        {
            myAnimator.SetBool("isWalking", true);
        }
        else
        {
            myAnimator.SetBool("isWalking", false);
        }
        AdjustPlayerFacingDirection();
        Move();
    }
    public bool IsHoldingObject()
    {
        return grabbedObject != null;
    }

    public void SetGrabbedObject(Grabable obj)
    {
        grabbedObject = obj;
    }

    public void ClearGrabbedObject()
    {
        grabbedObject = null;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object has the MoveableObject component
        if (other.GetComponent<MoveableObject>() != null)
        {
            // Set the animator parameter to true (player is pushing)
            myAnimator.SetBool("isPushing", true);
            myAnimator.SetBool("isWalking", false);
        }
    }

 

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the colliding object has the MoveableObject component
        if (other.GetComponent<MoveableObject>() != null)
        {
            // Set the animator parameter to false (player is not pushing)
            myAnimator.SetBool("isPushing", false);
            myAnimator.SetBool("isWalking", true);
        }
    }
    public void OpenPauseMenu()
    {
        Canvas canvas = GameObject.Find("PauseMenu").GetComponent<Canvas>();
        bool isMenuActive = !canvas.enabled;
        canvas.enabled = isMenuActive;
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            canvas.transform.GetChild(i).gameObject.SetActive(isMenuActive);
        }
        Time.timeScale = isMenuActive ? 0f : 1f;
    }
    public void OpenCraftInventory()
    {
        
        GameObject craftui = GameObject.Find("UI_Craft").transform.GetChild(0).gameObject;
        bool isMenuActive;
      
        if (!craftui.activeSelf)
        {
            craftui.SetActive(true);
            isMenuActive = true;
        }
        else
        {
            craftui.SetActive(false);
            isMenuActive = false;
        }
        Time.timeScale = isMenuActive ? 0f : 1f;
        
    }

    private void UseItem()
    {
        if(InventoryManager.Instance.selectedItem != null)
        {
            InventoryManager.Instance.selectedItem.Use(this);

        }
    }

    public bool Talk()
    {
        return true;
    }
    public void DeactivateWeaponCollider()
    {
        weaponCollider.gameObject.SetActive(false);
    }

    public void OpenInventory()
    {
        Canvas canvas = GameObject.Find("UI_Inventory").GetComponent<Canvas>();
        GameObject slots = GameObject.Find("UI_Inventory").GetComponent<Transform>().GetChild(2).gameObject;
        Debug.Log(canvas.name);
        bool isMenuActive = !canvas.enabled;

        if (!canvas.enabled)
        {
            canvas.enabled = true;
            slots.SetActive(true);
        }
        else
        {
            slots.SetActive(false);
            canvas.enabled = false;
        }
        Time.timeScale = isMenuActive ? 0f : 1f;
    }
    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }
   

    private void PlayerInput()
    {
        if(PlayerHealth.Instance.CurrentHealth<=0) return;
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        if (AllyAI.Instance.state != AllyAI.State.Waiting)
        {
            // Check chain distance and stop movement if needed
            Vector2 playerToAlly = allyPosition.position - (Vector3)transform.position;
            Vector2 normalizedPlayerToAlly = playerToAlly.normalized;
            float distanceToAlly = playerToAlly.magnitude;
            if (allyPosition.gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                if (distanceToAlly > maxChainDistance && Vector2.Dot(normalizedPlayerToAlly, movement) < 0)
                {
                    movement = Vector2.zero; // Stop player movement
                }
            }
        }
       
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
        slash.GetComponent<Animator>().SetFloat("moveX", movement.x);
        slash.GetComponent<Animator>().SetFloat("moveY", movement.y);
    }
   
    private void Move() 
    {
        if (knockBack.GetingKnockedBack || PlayerHealth.Instance.IsDead) return;
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }
    
    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        if (mousePos.x < playerPos.x)
        {
            
            mySpriteRenderer.flipX = true;
            slash.GetComponent<SpriteRenderer>().flipX = false;
            FacingRight = true;
        }
        else
        {
      
            mySpriteRenderer.flipX = false;
            slash.GetComponent<SpriteRenderer>().flipX = true;
            FacingRight = false;
        }
    }
    public Vector2 GetFacingDirection()
    {
        // Update the facing direction based on player input.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0f || verticalInput != 0f)
        {
            // Normalize the direction to get a unit vector.
            facingDirection = new Vector2(horizontalInput, verticalInput).normalized;
        }

        return facingDirection;
    }
    private void Dash()
    {
        if (!isDashing && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();
            isDashing = true;
            tr.emitting = true;
            moveSpeed *= dashSpeed;
            StartCoroutine(EndDashRoutine());
        }
        
        
    }
    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        tr.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPos;
    }

    public void SaveData(ref GameData data)
    {
        data.playerPos = this.transform.position;
    }

     
}
