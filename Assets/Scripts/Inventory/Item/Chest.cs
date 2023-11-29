using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Chest : MonoBehaviour, IDataPersisence
{
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    [Tooltip("1-zCoin, 2- healthGlobe, 3-staminaGlobe, 4-bullet")] [SerializeField]
    private int spawnItem = 1;

    public Sprite emptyChest;
    private PickUpSpawner spawner;
    [SerializeField] private int amount;
    public bool collected = false;
    private BoxCollider2D myCollider;

    private void Awake()
    {
        spawner = GetComponent<PickUpSpawner>();
       
        myCollider = GetComponent<BoxCollider2D>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected)
        {
            if (other.gameObject.GetComponent<PlayerController>())
            {
                for (int i = 0; i < amount; i++)
                {
                    OnCollect();
                }
            }

            collected = true;
        }
        
    }

    public void OnCollect()
    {
        GetComponent<SpriteRenderer>().sprite = emptyChest;
        myCollider.enabled = false;
        spawner.DropItem(spawnItem);
        StopAllCoroutines();
        StartCoroutine(ChestRoutine());
    }

    private IEnumerator ChestRoutine()
    {
        this.transform.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        this.transform.GetComponent<CapsuleCollider2D>().enabled = true;
    }
    public void LoadData(GameData data)
    {
        data.chestsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            myCollider.enabled = false;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (id != "")
        {
            if (data.chestsCollected.ContainsKey(id))
            {
                data.chestsCollected.Remove(id);
            }

            data.chestsCollected.Add(id, collected);
        }
    }
}