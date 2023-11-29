using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevel : Singleton<PlayerLevel>
{
    [SerializeField] private TMP_Text levelText;
    private const int PlayerDamageMultiplier = 2;
    private const int EnemyHealthMultiplier = 2;
    private const int EnemyDamageMultiplier = 2;
    public float damageMultiplier;
    public int level = 1;
    public float currentxp;
    public float requiredXP;

    private float lerpTimer;
    private float delayTimer;
    [Header("UI")]
    public Image frontXpBar;
    public Image backXpBar;


    private void Start()
    {
        frontXpBar.fillAmount = currentxp / requiredXP;
        backXpBar.fillAmount = currentxp / requiredXP;
    }
    public void Update()
    {
        UpdateXpUI();

        levelText.text = level.ToString("D2");
        if(currentxp >= requiredXP)
        {
            LevelUp();
        }
    }

    public void UpdateXpUI()
    {
        float xpFraction = currentxp / requiredXP;
        float FXP = frontXpBar.fillAmount;
        if (FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;
            backXpBar.fillAmount = xpFraction;
            if (delayTimer > 3)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / 4;
                frontXpBar.fillAmount = Mathf.Lerp(FXP, backXpBar.fillAmount, percentComplete);
            }
        }



    }
    public void GainExperienceFlatRate(int xpGained)
    {
        currentxp += 5 + (level * EnemyHealthMultiplier) + xpGained;
        lerpTimer = 0f;
        delayTimer = 0f;
    }
    public float SetDamageMulytiplier(int damageAmout)
    {
        return damageMultiplier =  level * PlayerDamageMultiplier + damageAmout;
    }
    public float SetRequiredXP()
    {
        return requiredXP = ((1f / 4f) * (level - 1f + 300f * (2f * (level - 1f / 7f))));
    }
    public void LevelUp()
    {
        if (!PlayerHealth.Instance.IsDead)
        {
            level++;
            frontXpBar.fillAmount = 0f;
            frontXpBar.fillAmount = 0f;
            // Cálculo dos valores escalados para o jogador
            PlayerHealth.Instance.IncreaseHealth(level);
            if (level % 4 == 0)
                Stamina.Instance.AddStaminaBar();

            SetRequiredXP();
            
        }
        

    }
}

