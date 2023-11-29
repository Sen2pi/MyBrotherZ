using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using MyBrotherZ.Quest;
using MyBrotherZ.SaveAndLoad;
using SaveAndLoad;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public int currentAmmo , currentArrows;
    public Vector3 playerPos;
    public SerializableDictionary<string, bool> chestsCollected;
    public int currentCoin, dateInMonth, season, year, hour, minute;
    public float currentMarkTime, nextMarkTime, marksTimeDiference; 
    public SerializableDictionary<string, bool> itemsCollected;
    public SerializableDictionary<string, Vector3> enemysPosition;
    public SerializableDictionary<string, bool> deadEnemys;
    public SerializableDictionary<string, int> currentEnemyHealths;
    public float currentCycleTime;
    public int currentMarkIndex, nextMarkIndex;
    public float currentHealth;
    public SlotData[] inventoryItems;
    public Vector3 allyPos;
    public int allyHealth;
    public AllyAI.State state;
    public int selectedSlotIndex;
    public QuestStatusRecord[] quests;
    public QuestStatusRecord[] completedQuests;
    

    //TUDO O QUE ESTIVER  neste construtor sera o default do novo jogo.
    public GameData()
    {
        dateInMonth = 1;
        season = 1;
        year = 23;
        hour = 6;
        minute = 0;
        currentCoin = 0;
        currentAmmo = 0;
        currentArrows = 0;
        currentCycleTime = 0;
        currentMarkIndex = 0;
        nextMarkIndex = 0;
        currentMarkTime = 0;
        nextMarkTime = 0;
        marksTimeDiference = 0;
        selectedSlotIndex = 0;
        playerPos.x = -303.6f;
        playerPos.y = -280.7f;
        chestsCollected = new SerializableDictionary<string, bool>();
        itemsCollected = new SerializableDictionary<string, bool>();
        enemysPosition = new SerializableDictionary<string,  Vector3>();
        deadEnemys = new SerializableDictionary<string, bool>();
        currentEnemyHealths = new SerializableDictionary<string, int>();
        state = AllyAI.State.Waiting;
    }
    
    
}