using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : Singleton<PlayerHealth>, IDataPersisence
{
    public bool IsDead { get; private set; }
    [SerializeField]private float maxHealth = 5;
    
    [SerializeField]private float knockBackThrustAmount = 10f;
    [SerializeField]private float damageRecoveryTime  = 1f;
    [SerializeField]private float currentHealth;
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    private KnockBack knockBack;
    private Flash flash;
    private bool canTakeDamage = true;
    private Slider healthSlider;
    const string HEALTH_SLIDER_TEXT = "Slider";
    const string SCENE2_TEXT = "Scene_2";
    readonly int DEATH_HASH = Animator.StringToHash("Death");
    protected override void Awake()
    {
        base.Awake();
        flash = GetComponent<Flash>();
        knockBack = GetComponent<KnockBack>();
    }

    private void Start()
    {
        IsDead = false;
        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
            UpdateHealthSlider();
        }
        UpdateHealthSlider();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if (enemy && canTakeDamage)
        {
            TakeDamage(1,collision.transform);
            
        }
    }
    public void Heal(int amount)
    {
        if(currentHealth < maxHealth)
        {
            currentHealth += amount;
            UpdateHealthSlider();
        }
        
    }
    private void CheckIfPlayerDeath()
    {
        if(currentHealth <= 0 && !IsDead)
        {
            IsDead = true;
            Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }
    private IEnumerator DeathLoadSceneRoutine()
    {

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        Stamina.Instance.ReplenishStaminaOnDeath();
        SceneManager.LoadScene(SCENE2_TEXT);
        
    }
    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) { return; }
        knockBack.GetKnockedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        currentHealth -= (PlayerLevel.Instance.SetDamageMulytiplier(damageAmount));
        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfPlayerDeath();

    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
      
        canTakeDamage = true;
    }
    private void UpdateHealthSlider()
    {
        if(healthSlider == null)
        {
            healthSlider = GameObject.Find(HEALTH_SLIDER_TEXT).GetComponent<Slider>();
        }
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
    public void IncreaseHealth(int level)
    {
        maxHealth += (currentHealth * 0.1f) * ((10 - level) * .1f);
        currentHealth = maxHealth;
    }

    public void LoadData(GameData data)
    {
        this.currentHealth = data.currentHealth;
    }

    public void SaveData(ref GameData data)
    {
        data.currentHealth = this.currentHealth;
    }
    
}
