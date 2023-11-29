using System;
using UnityEngine;
using UnityEngine.Events;



public class TimeManager : Singleton<TimeManager>, IDataPersisence
{
    [Header("Date & Time Settings")]
    [Range(1, 28)]
    public int dateInMonth;
    [Range(1, 4)]
    public int season;
    [Range (1, 99)]
    public int year;
    [Range(0,24)]
    public int hour;
    [Range(0,6)]
    public int minute;

    private DateTime dateTime;

    [Header("Tick Settings")]
    public int tickSecondsIncrease = 10;
    public float timeBetweenTicks = 1;
    private float currentTimeBetweenTicks = 0;

    public static UnityAction<DateTime> OnDateTimeChanged;

    protected override void Awake()
    {
        base.Awake();
        dateTime = new DateTime(dateInMonth, season - 1, year,hour,(minute * 10));
    }
    private void Start()
    {
        OnDateTimeChanged?.Invoke(dateTime);
        
    }
    private void Update()
    {
        currentTimeBetweenTicks +=Time.deltaTime;

        if(currentTimeBetweenTicks >= timeBetweenTicks) { 
            currentTimeBetweenTicks = 0;
            Tick();
        }
    }
    private void Tick()
    {
        AdvanceTime();
    }

    private void AdvanceTime()
    {
        dateTime.AdvanceMinutes(tickSecondsIncrease);
        OnDateTimeChanged?.Invoke(dateTime);
    }
    [System.Serializable]
    public struct DateTime
    {

        #region Fields
        private Days day;
        private int date;
        private int year;
        private int hour;
        private int minute;
        private Season season;
        private int totalNumDays;
        private int totalNumWeeks;
        #endregion

        #region Properties
        public Days Day => day;
        public int Date => date;
        public int Hour => hour;
        public int Minute => minute;
        public Season Season => season;
        public  int Year => year;
        public int TotalNumDays => totalNumDays;
        public int TotalNumWeeks => totalNumWeeks;
        public int CurrentWeek => totalNumWeeks % 16 == 0 ? 16 : totalNumWeeks % 16;

        public DateTime(int date, int season, int year , int hour, int minutes)
        {
            this.day = (Days)(date % 7);
            if (day == 0) day = (Days)7;
            this.date = date;
            this.season = (Season)season;
            this.year = year;
            this.hour = hour;
            this.minute = minutes;

            totalNumDays = (int)this.season > 0 ? date + (28 * (int)this.season) : date;
            totalNumDays = year > 1 ? totalNumDays + (112* (year-1)) : totalNumDays;

            totalNumWeeks = 1 + totalNumDays / 7;
        }
        #endregion

        #region Time Advancement
        public void AdvanceMinutes(int secondsToAdvance)
        {
            if(minute + secondsToAdvance >= 60)
            {
                minute = (minute + secondsToAdvance) % 60;
                AdvanceHour();
            }
            else
            {
                minute += secondsToAdvance;
            }
        }
        private void AdvanceHour()
        {
            if((hour + 1) == 24)
            {
                hour = 0;
                AdvanceDay();
            }
            else
            {
                hour++;
            }
        }
        private void AdvanceDay()
        {
            if (day + 1 > (Days)7)
            {
                day = (Days)1;
                totalNumWeeks++;
            }
            else
            {
                day++;
            }
            date++;

            if (date % 29 == 0)
            {
                AdvanceSeason();
                date = 1;
            }

            totalNumDays++;
        }
        private void AdvanceSeason()
        {
            if (season == Season.Winter)
            {
                season = Season.Spring;
                AdvanceYear();
            }
            else season++;
        }
        private void AdvanceYear()
        {
            date=1;
            year++;
        }
        #endregion

        #region bool checks
        public bool IsNight()
        {
            return hour > 18 || hour < 6;
        }
        public bool IsMorning()
        {
            return hour> 12 && hour <= 12;
        }
        public bool IsAfternoon()
        {
            return hour > 12 || hour < 18;
        }

        public bool IsWeekend()
        {
            return day > Days.Fri? true: false;
        }
        public bool IsParticularDay(Days day)
        {
            return this.day == day;
        }
        #endregion

        #region Key Dates
        public DateTime NewYearsDay(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(1, 0, year, 6, 0);
        }
        public DateTime SummerSolstice(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(28, 1, year, 6, 0);
        }
        public DateTime PumpkinHarvest(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(28, 2, year, 6, 0);
        }
        #endregion

        #region Season starts
        public System.DateTime StartOfSeason(int season, int year)
        {
            System.DateTime startOfSeason;
            switch (season)
            {
                case 0:
                    startOfSeason = new System.DateTime(year, 3, 20); //in�cio da primavera � sempre 20 de mar�o
                    break;
                case 1:
                    startOfSeason = new System.DateTime(year, 6, 21); //in�cio do ver�o � sempre 21 de junho
                    break;
                case 2:
                    startOfSeason = new System.DateTime(year, 9, 22); //in�cio do outono � sempre 22 de setembro
                    break;
                case 3:
                    startOfSeason = new System.DateTime(year, 12, 21); //in�cio do inverno � sempre 21 de dezembro
                    break;
                default:
                    throw new ArgumentException("Invalid season value.");
            }
            return startOfSeason;
        }
        public System.DateTime StartOfSpring(int year)
        {
            return StartOfSeason(0, year);
        }
        public System.DateTime StartOfSummer(int year)
        {
            return StartOfSeason(1, year);
        }
        public System.DateTime StartOfAutumn(int year)
        {
            return StartOfSeason(2, year);
        }
        public System.DateTime StartOfWinter(int year)
        {
            return StartOfSeason(3, year);
        }
        #endregion

        #region ToStrings
        public override string ToString()
        {
            
            return $"Date: {DateToString()} Season: {season.ToString()} Time: {TimeToString()}"+
                $"\nTotal Days: {totalNumDays} | Total Weeks: {totalNumWeeks}";
        }
        public string DateToString()
        {
            return $"{day} {date} {year.ToString("D2")}";
        }
        public string TimeToString()
        {
            int adjustedHour = 0;
            if (hour == 0) adjustedHour = 0;
            else if (hour == 24) adjustedHour = 0;
            else adjustedHour = hour;
            string AmPm = hour == 0 || hour < 12 ? "AM" : "PM";
            return $"{adjustedHour.ToString("D2")}:{minute.ToString("D2")} {AmPm}"; 
        }
        public string SeasonToString()
        {
            return $"{season}";
        }
        #endregion



    }
    [System.Serializable]
    public enum Days { 
        NULL= 0,
        Mon = 1,
        Tue = 2,
        Wed = 3,
        Thu = 4,
        Fri = 5,
        Sat = 6,
        Sun = 7
        
    }
    [System.Serializable]
    public enum Season
    {
        Spring = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 3
    }

    public void LoadData(GameData data)
    {
        this.dateInMonth = data.dateInMonth;
        this.season = data.season;
        this.year = data.year;
        this.hour = data.hour;
        this.minute = data.minute;
    }

    public void SaveData(ref GameData data)
    {
        data.dateInMonth = this.dateInMonth;
        data.season = this.season;
        data.year = this.year;
        data.hour = this.hour;
        data.minute = this.minute;
    }
}
