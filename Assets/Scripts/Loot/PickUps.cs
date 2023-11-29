using System;
using System.Collections;
using System.Collections.Generic;
using SaveAndLoad;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickUps : MonoBehaviour, IDataPersisence
{
    private enum PickUpType
    {
        ZCoin,
        StaminaGlobe,
        HealthGlob,
        Bullets,
        Arrows,
        katana,
        ZBlood,
        HBlood,
        Metralhadora,
        LaserGun,
        Consumable,
        Material
    }

    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] private bool collected = false;
    [SerializeField] private PickUpType pickUpType;
    [SerializeField] private float pickUpDistance = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float accelarationRate = .2f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1F;
    [SerializeField] private ItemClass item;


    private Rigidbody2D rb;
    private Vector3 moveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(AnimCurveSpawnRoutine());
    }

    private void Update()
    {
        Vector3 playerPos = PlayerController.Instance.transform.position;
        if (Vector3.Distance(transform.position, playerPos) < pickUpDistance)
        {
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += accelarationRate;
        }
        else
        {
            moveDir = Vector3.zero;
            moveSpeed = 0;
        }
    }


    private void FixedUpdate()
    {
        rb.velocity = moveDir * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            DetectPicupType();
            collected = true;
            Destroy(gameObject);
        }
    }

    private IEnumerator AnimCurveSpawnRoutine()
    {
        Vector2 startPoint = transform.position;
        float randomX = transform.position.x + Random.Range(-2f, 2f);
        float randomY = transform.position.y + Random.Range(-1f, 1f);
        Vector2 endPoint = new Vector2(randomX, randomY);

        float timePassed = 0f;
        while (timePassed < popDuration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;
            float heighT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heighT);

            transform.position = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height);
            yield return null;
        }
    }

    private void DetectPicupType()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpDistance);
        foreach (Collider2D collider in colliders)
        {
            GameObject objectCollided = collider.gameObject;

            // Verifique se o objeto colidido foi destruído
            if (objectCollided == null)
            {
                continue;
            }

            PickUps itemPickup = objectCollided.GetComponent<PickUps>();

            // Verifique se o componente ItemPickup não é nulo antes de acessá-lo
            if (itemPickup != null)
            {
                switch (pickUpType)
                {
                    case PickUpType.HealthGlob:
                        PlayerHealth.Instance.Heal(1);
                        break;
                    case PickUpType.StaminaGlobe:
                        Stamina.Instance.RefreshStamina();
                        break;
                    case PickUpType.ZCoin:
                        InventoryManager.Instance.Add(item, 1);
                        EconomyManager.Instance.UpdateCurrentGold();
                        break;
                    case PickUpType.ZBlood:
                        InventoryManager.Instance.Add(item, 1);
                        break;
                    case PickUpType.HBlood:
                        InventoryManager.Instance.Add(item, 1);
                        break;
                    case PickUpType.Bullets:
                        if (InventoryManager.Instance.Contains(item) == null)
                        {
                            InventoryManager.Instance.Add(item, 1);
                            InventoryManager.Instance.RefresUI();
                            InventoryManager.Instance.Contains(item).Quantity -= 1;
                            InventoryManager.Instance.RefresUI();
                        }

                        PlayerAmmo.Instance.CollectAmmo();
                        PlayerAmmo.Instance.UpdateBulletCountText();
                        break;
                    case PickUpType.Arrows:
                        if (InventoryManager.Instance.Contains(item) == null)
                        {
                            InventoryManager.Instance.Add(item, 1);
                            InventoryManager.Instance.RefresUI();
                            InventoryManager.Instance.Contains(item).Quantity -= 1;
                            InventoryManager.Instance.RefresUI();
                        }

                        PlayerAmmo.Instance.CollectArrow();
                        PlayerAmmo.Instance.UpdateArrowCountText();
                        break;
                    case PickUpType.katana:
                        InventoryManager.Instance.Add(item, 1);
                        break;
                    case PickUpType.Consumable:
                        InventoryManager.Instance.Add(item, 1);
                        break;
                    case PickUpType.Material:
                        InventoryManager.Instance.Add(item, 1);
                        break;
                    case PickUpType.Metralhadora:
                        InventoryManager.Instance.Add(item, 1);
                        break;
                    case PickUpType.LaserGun:
                        InventoryManager.Instance.Add(item, 1);
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void LoadData(GameData data)
    {
        try
        {
            data.itemsCollected.TryGetValue(id, out collected);
            if (collected)
            {
                Destroy(gameObject);
            }
        }
        catch
        {
            
        }
        
    }

    public void SaveData(ref GameData data)
    {
        try
        {
            if (id == "")
            {
                return;
            }
            if (data.itemsCollected.ContainsKey(id))
            {
                data.itemsCollected.Remove(id);
            }

            data.itemsCollected.Add(id, collected);
        }
        catch 
        {
           
        }
       
    }
}