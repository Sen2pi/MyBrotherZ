using System;
using System.Collections;
using System.Collections.Generic;
using MyBrotherZ.SaveAndLoad;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour, IDataPersisence
{
    [SerializeField] private float roamChangeDirection = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float randomRange = 5f;
    [SerializeField] private MonoBehaviour enemyType;
    public MonoBehaviour EnemyType { get => enemyType; set => enemyType = value; }
    [SerializeField] private float attackCoolDown;
    [SerializeField] private bool stopMovingWhileattacking = false;
    [SerializeField] string id;
    private enum State
    {
        Roaming,
        Attacking
    }
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;
    private Vector2 roamPosition;
    private float timeRoaming = 0f;
    private bool canAttack = true;
   
    

    private  void Awake()
    {
        
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
    }
    
    private void Start()
    {
        roamPosition = GetRoamingPosition();  
    }
    private void Update()
    {
        MovementStateControl();
        
    }

    public string GetID()
    {
        return id;
    }
    private void MovementStateControl()
    {
        switch (state)
        {
            default:
                GetComponent<Animator>().SetBool("isWalking", false);
                break;
            case State.Roaming:         
                Roaming();
                break;
            case State.Attacking:
                Attacking();
                break;
        }
    }

    private void Roaming()
    {
        if (GetComponent<EnemyHealth>().currentHealth > 0)
        {
            GetComponent<Animator>().SetBool("isWalking", true);
            timeRoaming += Time.deltaTime;
            enemyPathfinding.MoveTo(roamPosition, moveSpeed);
            if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange)
            {
                state = State.Attacking;
            }
            if (timeRoaming > roamChangeDirection)
            {
                roamPosition = GetRoamingPosition(); 
            }
        }
    }

    private void Attacking()
    {
        if (GetComponent<EnemyHealth>().currentHealth > 0)
        {
            if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange)
            {
                state = State.Roaming;
            }
            if (attackRange !=0 && canAttack)
            {   GetComponent<Animator>().SetTrigger("Attack");
                canAttack = false;
                (enemyType as IEnemy).Attack();
                if (stopMovingWhileattacking)
                {
                    enemyPathfinding.StopMoving();
                }
                else
                {
                    enemyPathfinding.MoveTo(roamPosition, moveSpeed);
                }
                StartCoroutine(AttackCoolDownRoutine());
            }
        }
        
    }
    private IEnumerator AttackCoolDownRoutine()
    {
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }

    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;

        // Calculate a random position within a certain range for roaming around the current position
        Vector2 randomOffset = new Vector2(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
        return (Vector2)transform.position + randomOffset;
    }


    public void LoadData(GameData data)
    {
        if (data.enemysPosition.ContainsKey(id))
        {
            transform.position = data.enemysPosition.TryGetValue(id, out Vector3 position) ? position : transform.position;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.enemysPosition.ContainsKey(id))
        {
            data.enemysPosition.Remove(id);
            data.enemysPosition.Add(id, transform.position);
        }
        else
        {
            data.enemysPosition.Add(id, transform.position);
        }
       
    }
}
