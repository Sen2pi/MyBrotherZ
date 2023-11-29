using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyHealth : Singleton<AllyHealth>, IDataPersisence
{
    [SerializeField] private int startingHealth = 3;
    public int StartingHealth { get => startingHealth; set => startingHealth = value; }
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackthrust = 15f;
    private int currentHealth;
    private KnockBack knockBack;
    private bool isDead;
    private Flash flash;

    protected override void Awake()
    {
        base.Awake();
        flash = GetComponent<Flash>();
        knockBack = GetComponent<KnockBack>();
    }

    private void Start()
    {
        startingHealth = 5 + (PlayerLevel.Instance.level * 2);
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    { 
        if (!isDead)
        {
            currentHealth -= damage; // No need for damage multiplier here
            knockBack.GetKnockedBack(transform, knockBackthrust); // Knock back self
            StartCoroutine(flash.FlashRoutine());
            StartCoroutine(CheckDeathRoutine());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<EnemyAI>())
        {
            TakeDamage(1);
        }
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
            isDead = true;
            // Ally does not gain experience or drop items, you can implement this if needed
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DeathRoutine()
    {
        GetComponent<Animator>().SetTrigger("Death");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void LoadData(GameData data)
    {
        currentHealth = data.allyHealth;
    }

    public void SaveData(ref GameData data)
    {
        data.allyHealth = currentHealth;
    }
}