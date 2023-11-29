using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EconomyManager : Singleton<EconomyManager>, IDataPersisence
{
    [SerializeField]private TMP_Text goldText;
    const string COIN_AMOUNT_TEXT = "ZAmount";
    
    private int currentCoin = 0;

    public void UpdateCurrentGold()
    {
        currentCoin += 1;
        if(goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
        }
        goldText.text = currentCoin.ToString("D4");
    }

    public void LoadData(GameData data)
    {
        currentCoin = data.currentCoin;
        UpdateCurrentGold();
        currentCoin -= 1;
    }

    public void SaveData(ref GameData data)
    {
        data.currentCoin = currentCoin;
    }
}
