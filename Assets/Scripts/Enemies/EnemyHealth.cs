using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDataPersisence
{
    [SerializeField] private int startingHealth = 3;
    public int StartingHealth { get => startingHealth; set => startingHealth = value; }
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackthrust = 15f;
    public int currentHealth;
    private KnockBack knockBack;
    [SerializeField] private int xpOnKill = 3;
    private bool isDead;
    private Flash flash;
    public int damage = 1;
    [SerializeField] private HealthBarBehavior healthBar;

    public  void Awake()
    {
        flash = GetComponent<Flash>();
        knockBack = GetComponent<KnockBack>();
        
    }
    private void Start()
    {   startingHealth = 5 + (PlayerLevel.Instance.level * 2);
        if (currentHealth == 0)
        {
            currentHealth = startingHealth;
            UpdateHealthBar();
        }
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    { 
        if (!isDead)
        {
            currentHealth -= (int)PlayerLevel.Instance.SetDamageMulytiplier(damage) ;
            knockBack.GetKnockedBack(PlayerController.Instance.transform, knockBackthrust);
            StartCoroutine(flash.FlashRoutine());
            StartCoroutine(CheckDeathRoutine());
            UpdateHealthBar();
        }
    }
    private void UpdateHealthBar()
    {
        healthBar.SetHealth(currentHealth, startingHealth);
    }
    private IEnumerator CheckDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }
    private void DetectDeath()
    {
        if(currentHealth <= 0)
        {
            healthBar.SetHealth(0, startingHealth);
            isDead = true;
            PlayerLevel.Instance.GainExperienceFlatRate(xpOnKill);
            Instantiate(deathVFXPrefab,transform.position, Quaternion.identity);
            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DeathRoutine()
    {
        GetComponent<EnemyPathfinding>().enabled = false;
        GetComponent<Animator>().SetTrigger("Death");
        yield return new WaitForSeconds(2f);
        GetComponent<PickUpSpawner>().DropItem(8);
        GetComponent<PickUpSpawner>().DropRandomItems();
        gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        if (data.currentEnemyHealths.ContainsKey(GetComponent<EnemyAI>().GetID()))
        {
            data.currentEnemyHealths.TryGetValue(GetComponent<EnemyAI>().GetID(), out currentHealth);
            data.deadEnemys.TryGetValue(GetComponent<EnemyAI>().GetID(), out isDead);
            if (isDead)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.currentEnemyHealths.ContainsKey(GetComponent<EnemyAI>().GetID()))
        {
            data.currentEnemyHealths.Remove(GetComponent<EnemyAI>().GetID());
            data.currentEnemyHealths.Add(GetComponent<EnemyAI>().GetID(),currentHealth);
            data.deadEnemys.Remove(GetComponent<EnemyAI>().GetID());
            data.deadEnemys.Add(GetComponent<EnemyAI>().GetID(),isDead);
        }
        else
        {
            data.currentEnemyHealths.Add(GetComponent<EnemyAI>().GetID(),currentHealth);
            data.deadEnemys.Add(GetComponent<EnemyAI>().GetID(),isDead);
        }
        
    }
}
