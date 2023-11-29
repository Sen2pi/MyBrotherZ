using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;




public class ClockManager : Singleton<ClockManager>
{
    public RectTransform ClockFace;
    public TextMeshProUGUI Date, Time, Season, Week;


    public float startingRotation;

    public Light2D sunlight;
    public float nightIntensity;
    public float dayIntensity;
    public AnimationCurve dayNightCurve;

    protected override void Awake()
    {
        base.Awake();
        if (ClockFace == null)
        {
            ClockFace = GameObject.Find("FAce").GetComponent<RectTransform>();
        }
        startingRotation = ClockFace.localEulerAngles.z;

    }

    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += UpdateDateTime;
    }
    private void OnDisable()
    {
        TimeManager.OnDateTimeChanged -= UpdateDateTime;
    }
    
    private void UpdateDateTime(TimeManager.DateTime dateTime)
    {
        Date.text = dateTime.DateToString();
        Time.text = dateTime.TimeToString();
        Season.text = dateTime.SeasonToString();
        Week.text = $"WK: {dateTime.CurrentWeek}";

        float t = (float)dateTime.Hour / 24f;

        float newRotation = Mathf.Lerp(0,260,t);
        if(ClockFace != null)
        {
            ClockFace.localEulerAngles = new Vector3(0, 0, newRotation + startingRotation);

        }
        float dayNightT = dayNightCurve.Evaluate(t);

        sunlight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, dayNightT);

    }
}
