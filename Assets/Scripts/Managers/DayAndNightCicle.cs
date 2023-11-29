using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class DayAndNightCicle : Singleton<DayAndNightCicle>, IDataPersisence
{
    [System.Serializable]
    public struct DayAnNightMark
    {
        public float timeRatio;
        public Color color;
        public float intensity;

    }

    [SerializeField] private DayAnNightMark[] marks;
     public DayAnNightMark[] Marks
     {
         get => marks;
         private set => marks = value;
     }
    private Light2D customLight;
    [SerializeField] private float cycleLenght = 24; // Dura��o de um dia em segundos traduzidos
    [SerializeField] private int startHour = 8; // hora de come�o do dia 
    

    private const int DayMarkIndex = 0;
    private const int NightMarkIndex = 2;
    
    private float currentCycleTime;
    private int currentMarkIndex = -1, nextMarkIndex;
    private float currentMarkTime, nextMarkTime, marksTimeDiference;


    protected override void Awake()
    {
        customLight = GameObject.Find("Global Light 2D").GetComponent<Light2D>();
        base.Awake();
      
    }
    private const float TIME_CHECK_EPSILON = 0.1f;

    private void Start()
    {
        
        CycleMarks();

    }
    
    // Method to check if it is day
    public bool IsDay()
    {
        // If the currentMarkIndex is equal to the index for day (0), return true, otherwise, return false
        return currentMarkIndex == DayMarkIndex;
    }

    // Method to check if it is night
    public bool IsNight()
    {
        // If the currentMarkIndex is equal to the index for night (2), return true, otherwise, return false
        return currentMarkIndex == NightMarkIndex;
    }

    private string GetFormattedTime()
    {
        int hours = (int)(currentCycleTime / cycleLenght * 24);
        return $"{((startHour + hours) % 24).ToString("00")}H";
    }

    private void Update()
    {

        currentCycleTime = (currentCycleTime + Time.deltaTime) % cycleLenght;

        // blend color and intensity
        float t = (currentCycleTime - currentMarkTime) / marksTimeDiference;
        DayAnNightMark cur = marks[currentMarkIndex], next = marks[nextMarkIndex];
        customLight.color = Color.Lerp(cur.color, next.color, t);
        customLight.intensity = Mathf.Lerp(cur.intensity, next.intensity, t);

        //passed a mark ? 
        if(Mathf.Abs(currentCycleTime - nextMarkTime) < TIME_CHECK_EPSILON) {
            
            customLight.color = next.color;
            customLight.intensity = next.intensity;

            CycleMarks();
        }


    }
    private void CycleMarks()
    {
        currentMarkIndex = (currentMarkIndex + 1) % marks.Length;
        nextMarkIndex = (currentMarkIndex + 1) % marks.Length;
        currentMarkTime = marks[currentMarkIndex].timeRatio * cycleLenght;
        nextMarkTime = marks[nextMarkIndex].timeRatio * cycleLenght;
        marksTimeDiference = nextMarkTime - currentMarkTime;
        if(marksTimeDiference < 0 )
        {
            marksTimeDiference += cycleLenght;
        }
    }

    public void LoadData(GameData data)
    {
        currentMarkIndex = data.currentMarkIndex;
        nextMarkIndex = data.nextMarkIndex;
        currentMarkTime = data.currentMarkTime;
        nextMarkTime = data.nextMarkTime;
        marksTimeDiference = data.marksTimeDiference;
        currentCycleTime = data.currentCycleTime;
    }

    public void SaveData(ref GameData data)
    {
        data.currentMarkIndex = currentMarkIndex ;
        data.nextMarkIndex=nextMarkIndex  ;
        data.currentMarkTime=currentMarkTime  ;
        data.nextMarkTime=nextMarkTime  ;
        data.marksTimeDiference=marksTimeDiference ;
        data.currentCycleTime=currentCycleTime  ;
    }
}
