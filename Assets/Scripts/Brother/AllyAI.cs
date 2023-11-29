using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AllyAI : Singleton<AllyAI>, IDataPersisence
{
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float minDistanceToPlayer = 1.5f;
    [SerializeField] private float enemyDetectionRange = 6f; // Range to detect enemies
    [SerializeField] private Sprite alertSprite;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite followSprite;
    [SerializeField] private Sprite waitingSprite;
    [SerializeField] private Image stateImage;
    [SerializeField] private LineRenderer chainRenderer;
    [SerializeField] private Transform chainPoint;
    [SerializeField] private float chainTilingFactor = 0.2f; // Adjust this factor as needed
    [SerializeField] private float maxChainDistance = 10f;

    public enum State
    {
        Following,
        Alert,
        Waiting,
        Attacking
    }

    public State state;
    private AllyPathfinding allyPathfinding;
    private Transform playerTransform;
    private Transform detectedEnemy; // Reference to detected enemy
    private bool canAttack = true;
    [SerializeField] private Transform collisionHandlerTransform;

    protected override void Awake()
    {
        base.Awake();
        allyPathfinding = GetComponent<AllyPathfinding>();
        state = State.Following;
    }

    private void Update()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateChain();
        DetectEnemies(); // Detect nearby enemies
        HandleInput(); // Handle state transitions
        MovementStateControl();

    }
    private void UpdateChain()
    {
        if (chainRenderer)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = chainPoint.position; // Ally's position
            positions[1] = playerTransform.position; // Player's position
           

            chainRenderer.positionCount = 2;
            chainRenderer.SetPositions(positions);
            
            // Update the position of the collision handler GameObject
            Vector3 middlePoint = (positions[0] + positions[1]) * 0.5f;
            collisionHandlerTransform.position = middlePoint;
            // Calculate the distance between player and ally
            float distance = Vector3.Distance(positions[0], positions[1]);

            // Adjust the tiling of the LineRenderer's material based on the distance
            chainRenderer.material.mainTextureScale = new Vector2(distance * chainTilingFactor, 1f);
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            switch (state)
            {
                case State.Following:
                    state = State.Alert;
                    break;
                case State.Alert:
                    state = State.Waiting;
                    break;
                case State.Waiting:
                    state = State.Following;
                    break;
            }
        }
    }

    private void MovementStateControl()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxChainDistance)
        {
            state = State.Waiting; // Ally is beyond chain distance, stop moving
            allyPathfinding.StopMoving();
            return;
        }

        switch (state)
        {
            case State.Following:
                stateImage.sprite = followSprite;
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
                Following();
                break;
            case State.Alert:
                stateImage.sprite = alertSprite;
                Alert();
                break;
            case State.Waiting:
                stateImage.sprite = waitingSprite;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                gameObject.transform.GetChild(1).gameObject.SetActive(false);
                Waiting();
                break;
            case State.Attacking:
                stateImage.sprite = attackSprite;
                Attacking();
                break;
        }
    }

    private void Attacking()
    {
        if (detectedEnemy == null)
        {
            state = State.Alert; // Transition to Alert state if no enemy is detected
            return;
        }

        float distanceToEnemy = Vector2.Distance(transform.position, detectedEnemy.position);

        if (distanceToEnemy > enemyDetectionRange)
        {
            detectedEnemy = null; // Reset detected enemy
            state = State.Alert;
        }

        if (attackRange != 0 && canAttack)
        {
            canAttack = false;

            // Perform the attack action
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        // Play attack animation or trigger attack action
        GetComponent<Animator>().SetTrigger("Attack");

        // Wait for the attack animation to complete

        // Find and damage nearby enemies
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D collider in colliders)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage((int)PlayerLevel.Instance.damageMultiplier + 1);
            }
        }

        // Wait for the attack cooldown
        yield return new WaitForSeconds(0.2f);

        canAttack = true;
    }

    private void Following()
    {
        

        // If no enemy is detected, follow the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > minDistanceToPlayer)
        {
            allyPathfinding.MoveTo(playerTransform.position);
        }
        else
        {
            allyPathfinding.StopMoving();
        }
    }

    private void Alert()
    {
        if (detectedEnemy != null)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, detectedEnemy.position);

            if (distanceToEnemy <= attackRange)
            {
                state = State.Attacking;
                return;
            }
            else
            {
                allyPathfinding.MoveTo(detectedEnemy.position);
                return;
            }
        }
        Following();
        
    }

    private void Waiting()
    {
        // Ally stays in place and doesn't move
        allyPathfinding.StopMoving();
    }

    private void DetectEnemies()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, enemyDetectionRange);

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<EnemyHealth>())
            {
                detectedEnemy = collider.transform;
                break;
            }
        }
    }

    public void LoadData(GameData data)
    {
        transform.position = data.allyPos;
        state = data.state;
    }

    public void SaveData(ref GameData data)
    {
        if(PlayerController.Instance != null)
        {
            data.allyPos = transform.position;
            data.state = state;
        }
    }
    
}
