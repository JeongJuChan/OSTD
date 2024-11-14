using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DailyTimeCalculator
{
    private int rewardHour = 0;
    private int rewardMinute = 0;
    private int rewardSecond = 0;

    public event Action<string> OnUpdateDailyRewardTime;

    private readonly int TEN_DIGIT = 10;

    private int previousHour = 0;
    private int previousMinute = 0;
    private int previousSecond = 0;

    private Dictionary<CoroutinableUI, bool> coroutineActiveStateDict = new Dictionary<CoroutinableUI, bool>();

    public DailyTimeCalculator()
    {
        coroutineActiveStateDict = new Dictionary<CoroutinableUI, bool>();
    }

    public void InitCompleteTime(int rewardHour, int rewardMinute, int rewardSecond)
    {
        string[] time = ConvertTime(rewardHour, rewardMinute, rewardSecond).Split(',', ':', '/');
        this.rewardHour = int.Parse(time[0]);
        this.rewardMinute = int.Parse(time[1]);
        this.rewardSecond = int.Parse(time[2]);
    }

    public void UpdateCompleteTime(int rewardHour, int rewardMinute, int rewardSecond, string saveKey)
    {
        string[] time = ConvertTime(rewardHour, rewardMinute, rewardSecond).Split(',', ':', '/');
        this.rewardHour -= int.Parse(time[0]);
        this.rewardMinute -= int.Parse(time[1]);
        this.rewardSecond -= int.Parse(time[2]);

        if (this.rewardSecond < 0)
        {
            this.rewardSecond += Consts.MAX_SECOND;
            this.rewardMinute--;
        }

        if (this.rewardMinute < 0)
        {
            this.rewardMinute += Consts.MAX_SECOND;
            this.rewardHour--;
        }

        ES3.Save<string[]>(saveKey, new string[] { this.rewardHour.ToString(), this.rewardMinute.ToString(),
            this.rewardSecond.ToString() }, ES3.settings);
    }

    public void UpdateCompleteTime(int rewardHour, int rewardMinute, int rewardSecond)
    {
        string[] time = ConvertTime(rewardHour, rewardMinute, rewardSecond).Split(',', ':', '/');
        this.rewardHour -= int.Parse(time[0]);
        this.rewardMinute -= int.Parse(time[1]);
        this.rewardSecond -= int.Parse(time[2]);

        if (this.rewardSecond < 0)
        {
            int subtractMinute = -this.rewardSecond / Consts.MAX_SECOND;
            this.rewardSecond += subtractMinute * Consts.MAX_SECOND;
            this.rewardMinute -= subtractMinute;

            if (this.rewardSecond < 0)
            {
                this.rewardSecond += Consts.MAX_SECOND;
                this.rewardMinute--;
            }
        }

        if (this.rewardSecond < 0)
        {
            int subtractHour = -this.rewardSecond / Consts.MAX_MINUTE;
            this.rewardMinute += subtractHour * Consts.MAX_MINUTE;
            this.rewardHour -= subtractHour;

            if (this.rewardSecond < 0)
            {
                this.rewardMinute += Consts.MAX_SECOND;
                this.rewardHour--;
            }
        }
    }

    public int GetUseableTicketCount()
    {
        string[] time = Date.GetTimeSplit();
        int minutes = int.Parse(time[0]) * Consts.MAX_MINUTE + int.Parse(time[1]);
        int seconds = int.Parse(time[2]);
        int count = 0;
        if (minutes / 5 == 0)
        {
            count = seconds == 0 ? 0 : 1;
        }
        else
        {
            count = minutes % 5 == 0 && seconds == 0 ? minutes / 5 : minutes / 5 + 1;
        }
        return count;
    }

    public void ToggleCalculateRemainTime(CoroutinableUI coroutinable, bool isActive, Action completeAction)
    {
        if (!coroutineActiveStateDict.ContainsKey(coroutinable))
        {
            coroutineActiveStateDict.Add(coroutinable, false);
        }

        if (isActive)
        {
            if (coroutinable != null && (coroutinable.currentCoroutine == null || !coroutineActiveStateDict[coroutinable]))
            {
                if (coroutinable != null)
                {
                    coroutinable.currentCoroutine = coroutinable.StartCoroutine(CoCalculateRemainTime(completeAction));
                }
            }
        }
        else
        {
            if (coroutinable != null && coroutinable.currentCoroutine != null && coroutineActiveStateDict[coroutinable])
            {
                coroutinable.StopCoroutine(coroutinable.currentCoroutine);
            }
        }

        coroutineActiveStateDict[coroutinable] = isActive;
    }

    public void ToggleCalculateDailyRemainTime(CoroutinableUI coroutinable, bool isActive, Action completeAction)
    {
        if (!coroutineActiveStateDict.ContainsKey(coroutinable))
        {
            coroutineActiveStateDict.Add(coroutinable, false);
        }

        if (isActive)
        {
            if (coroutinable.currentCoroutine == null || !coroutineActiveStateDict[coroutinable])
            {
                coroutinable.currentCoroutine = coroutinable.StartCoroutine(CoCalculateDailyRemainTime(completeAction));
            }
        }
        else
        {
            if (coroutinable.currentCoroutine != null && coroutineActiveStateDict[coroutinable])
            {
                coroutinable.StopCoroutine(coroutinable.currentCoroutine);
            }
        }

        coroutineActiveStateDict[coroutinable] = isActive;
    }

    public int GetHourByDayDiff(string[] startDay)
    {
        string[] nowDay = Date.GetDaySplit(Date.GetDateTime());
        if (nowDay[0] != startDay[0] || nowDay[1] != startDay[1])
        {
            return Consts.MAX_HOUR * 7;
        }

        if (nowDay[2] != startDay[2])
        {
            int currentDay = int.Parse(nowDay[2]);
            int beforeDay = int.Parse(startDay[2]);
            int dayDiff = 0;
            if (currentDay < beforeDay)
            {
                int daysInMonth = DateTime.DaysInMonth(int.Parse(startDay[0]), int.Parse(startDay[1]));
                dayDiff = currentDay + daysInMonth - beforeDay;
            }
            else
            {
                dayDiff = currentDay - beforeDay;
            }

            return dayDiff * Consts.MAX_HOUR;
        }

        return 0;
    }

    public string ConvertTime(int hour, int minute, int second)
    {
        int additionalMinute = second / Consts.MAX_SECOND;
        int abtractSecond = second % Consts.MAX_SECOND;

        minute += additionalMinute;

        int additionalHour = minute / Consts.MAX_MINUTE;
        int abtractMinute = minute % Consts.MAX_MINUTE;

        hour += additionalHour;

        string abtractHourStr = hour / TEN_DIGIT == 0 ? $"0{hour}" : hour.ToString();
        string abtractMinuteStr = abtractMinute / TEN_DIGIT == 0 ? $"0{abtractMinute}" : abtractMinute.ToString();
        string abtractSecondStr = abtractSecond / TEN_DIGIT == 0 ? $"0{abtractSecond}" : abtractSecond.ToString();
        string currentTime = $"{abtractHourStr}:{abtractMinuteStr}:{abtractSecondStr}";
        return currentTime;
    }

    public bool CompareTime(string[] currentTime)
    {
        int hourComparison = int.Parse(currentTime[0]).CompareTo(rewardHour);
        if (hourComparison > 0)
        {
            return true;
        }
        else if (hourComparison == 0)
        {
            int minuteComparison = int.Parse(currentTime[1]).CompareTo(rewardMinute);
            if (minuteComparison > 0)
            {
                return true;
            }
            else if (minuteComparison == 0)
            {
                int secondComparison = int.Parse(currentTime[2]).CompareTo(rewardSecond);
                if (secondComparison > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /*public string[] GetDaySplit(string currentTime)
    {
        string[] temp = currentTime.Split();
        string[] date = temp[0].Split('-', '/', ':');
        return date;
    }*/

/*#elif UNITY_ANDROID
    public string[] GetTimeSplit(string currentTime)
    {
        string[] temp = currentTime.Split();
        string[] date = temp[2].Split('-', '/', ':');
        if (temp[1] == AFTER_NOON_ANDROID || temp[1] == AFTER_NOON)
        {
            if (date[0] != "12")
            {
                date[0] = (int.Parse(date[0]) + HALF_MAX_HOUR).ToString();
            }
        }
        return date;
    }
#endif*/

    public bool GetIsRewardPossible(string[] rewardDay, string[] rewardTime)
    {
        string[] newDay = Date.GetDaySplit();

        int yearComparison = rewardDay[0].CompareTo(newDay[0]);
        if (yearComparison < 0)
        {
            return true;
        }
        else if (yearComparison == 0)
        {
            int monthComparison = rewardDay[1].CompareTo(newDay[1]);
            if (monthComparison < 0)
            {
                return true;
            }
            else if (monthComparison == 0)
            {
                int dayComparison = rewardDay[2].CompareTo(newDay[2]);
                if (dayComparison < 0)
                {
                    return true;
                }
                else if (dayComparison == 0)
                {
                    string[] newTime = Date.GetTimeSplit();
                    // RewardTime should be lower than standard
                    // Also, currentTime should be upper than standard
                    if (!CompareTime(rewardTime))
                    {
                        return CompareTime(newTime);
                    }
                }
            }
        }

        return false;
    }

    public int GetSecondsByTime(int hour, int minute, int second)
    {
        minute += hour * Consts.MAX_MINUTE;
        second += minute * Consts.MAX_SECOND;
        return second;
    }

    public int GetMinutesByTime(int hour, int minute, int second)
    {
        minute += hour * Consts.MAX_MINUTE;
        minute += second <= 0 ? 0 : 1;

        return minute;
    }

    public int GetRewardMinutesByTime()
    {
        string[] time = Date.GetTimeSplit();
        int hour = rewardHour - int.Parse(time[0]);
        int minute = rewardMinute - int.Parse(time[1]);
        int second = rewardSecond - int.Parse(time[2]);

        Debug.Log($"hour {hour}, minute {minute}, second, {second}");
        Debug.Log($"rewardHour {rewardHour}, rewardMinute {rewardMinute}, rewardSecond, {rewardSecond}");
        Debug.Log($"time[0] {int.Parse(time[0])}, time[1] {int.Parse(time[1])}, time[2], {int.Parse(time[2])}");

        if (second < 0)
        {
            second += Consts.MAX_SECOND;
            minute--;
        }

        if (minute < 0)
        {
            minute += Consts.MAX_MINUTE;
            hour--;
        }

        if (hour < 0)
        {
            return 0;
        }

        return GetMinutesByTime(hour, minute, second);
    }

    public int GetRewardSecondsByTime()
    {
        string[] time = Date.GetTimeSplit();
        //string[] time = GetTimeSplit(DateTime.Now.ToString());
        
        int hour = rewardHour - int.Parse(time[0]);
        int minute = rewardMinute - int.Parse(time[1]);
        int second = rewardSecond - int.Parse(time[2]);

        Debug.Log($"hour {hour}, minute {minute}, second, {second}");
        Debug.Log($"rewardHour {rewardHour}, rewardMinute {rewardMinute}, rewardSecond, {rewardSecond}");
        Debug.Log($"time[0] {int.Parse(time[0])}, time[1] {int.Parse(time[1])}, time[2], {int.Parse(time[2])}");

        if (second < 0)
        {
            second += Consts.MAX_SECOND;
            minute--;
        }

        if (minute < 0)
        {
            minute += Consts.MAX_MINUTE;
            hour--;
        }

        if (hour < 0)
        {
            return 0;
        }

        return GetSecondsByTime(hour, minute, second);
    }

    // TODO: 껐다 켰을 때 말고 게임 중에도 초기화 할 수 있도록 하기
    private IEnumerator CoCalculateRemainTime(Action completeAction)
    {
        while (true)
        {
            string[] time = Date.GetTimeSplit();
            bool isChanged = int.Parse(time[0]) != previousHour || int.Parse(time[1]) != previousMinute || int.Parse(time[2]) != previousSecond;

            if (!isChanged)
            {
                yield return null;
                continue;
            }

            string currentTime = CalculateCurrentTime(time);

            UpdatePreviousTimes(time);
            if (currentTime != null)
            {
                OnUpdateDailyRewardTime?.Invoke(currentTime);
            }

            if (currentTime == null)
            {
                completeAction?.Invoke();
                yield return null;
                continue;
            }

            if (completeAction != null)
            {
                TryInvokeCompleteAction(completeAction, time);
            }

            yield return null;
        }
    }

    // TODO: 껐다 켰을 때 말고 게임 중에도 초기화 할 수 있도록 하기
    private IEnumerator CoCalculateDailyRemainTime(Action completeAction)
    {
        while (true)
        {
            string[] time = Date.GetTimeSplit();
            bool isChanged = int.Parse(time[0]) != previousHour || int.Parse(time[1]) != previousMinute || int.Parse(time[2]) != previousSecond;

            if (!isChanged)
            {
                yield return null;
                continue;
            }

            string currentTime = CalculateDailyCurrentTime(time);

            UpdatePreviousTimes(time);

            OnUpdateDailyRewardTime?.Invoke(currentTime);

            if (currentTime == null)
            {
                completeAction?.Invoke();
                yield return null;
                continue;
            }

            if (completeAction != null)
            {
                TryInvokeCompleteAction(completeAction, time);
            }

            yield return null;
        }
    }

    private void TryInvokeCompleteAction(Action completeAction, string[] timeStr)
    {
        int[] time = new int[] { int.Parse(timeStr[0]), int.Parse(timeStr[1]), int.Parse(timeStr[2]) };

        if (time[0] > rewardHour)
        {
            completeAction?.Invoke();
            Debug.Log($"dateTime.Hour : {time[0]}, rewardHour : {rewardHour}");
        }
        else if (time[0] == rewardHour)
        {
            if (time[1] > rewardMinute)
            {
                Debug.Log($"dateTime.Minute : {time[1]}, rewardMinute : {rewardMinute}");
                completeAction?.Invoke();
            }
            else if (time[1] == rewardMinute)
            {
                if (time[2] > rewardSecond)
                {
                    Debug.Log($"dateTime.Minute : {time[2]}, rewardMinute : {rewardSecond}");
                    completeAction?.Invoke();
                }
            }
        }
    }

    private void UpdatePreviousTimes(string[] time)
    {
        // TODO: 여기에서 검토하기
        //OnUpdateDay?.Invoke();
        previousHour = int.Parse(time[0]);
        previousMinute = int.Parse(time[1]);
        previousSecond = int.Parse(time[2]);
    }

    private string CalculateCurrentTime(string[] time)
    {
        int hour = int.Parse(time[0]);
        int minute = int.Parse(time[1]);
        int second = int.Parse(time[2]);

        bool isSecondUpperThanZero = rewardSecond - second >= 0;

        int abtractSecond = isSecondUpperThanZero ? rewardSecond - second : rewardSecond + Consts.MAX_MINUTE - second;

        if (!isSecondUpperThanZero)
        {
            minute++;
        }

        bool isMinuteUpperThanZero = rewardMinute - minute >= 0;

        int abtractMinute = isMinuteUpperThanZero ? rewardMinute - minute : rewardMinute + Consts.MAX_MINUTE - minute;

        if (!isMinuteUpperThanZero)
        {
            hour++;
        }

        int abtractHour = rewardHour - hour;/* >= 0 ?
                            rewardHour - hour : rewardHour + Consts.MAX_HOUR - hour;*/

        if (abtractHour < 0 || abtractMinute < 0 || abtractSecond < 0)
        {
            return null;
        }

        string abtractHourStr = abtractHour / TEN_DIGIT == 0 ? $"0{abtractHour}" : abtractHour.ToString();
        string abtractMinuteStr = abtractMinute / TEN_DIGIT == 0 ? $"0{abtractMinute}" : abtractMinute.ToString();
        string abtractSecondStr = abtractSecond / TEN_DIGIT == 0 ? $"0{abtractSecond}" : abtractSecond.ToString();
        string currentTime = $"{abtractHourStr}:{abtractMinuteStr}:{abtractSecondStr}";
        return currentTime;
    }

    private string CalculateDailyCurrentTime(string[] time)
    {
        int hour = int.Parse(time[0]);
        int minute = int.Parse(time[1]);
        int second = int.Parse(time[2]);

        bool isSecondUpperThanZero = rewardSecond - second >= 0;

        int abtractSecond = isSecondUpperThanZero ? rewardSecond - second : rewardSecond + Consts.MAX_MINUTE - second;

        if (!isSecondUpperThanZero)
        {
            minute++;
        }

        bool isMinuteUpperThanZero = rewardMinute - minute >= 0;

        int abtractMinute = isMinuteUpperThanZero ? rewardMinute - minute : rewardMinute + Consts.MAX_MINUTE - minute;

        if (!isMinuteUpperThanZero)
        {
            hour++;
        }

        int abtractHour = rewardHour - hour >= 0 ?
                            rewardHour - hour : rewardHour + Consts.MAX_HOUR - hour;

        if (abtractHour < 0 || abtractMinute < 0 || abtractSecond < 0)
        {
            return null;
        }

        string abtractHourStr = abtractHour / TEN_DIGIT == 0 ? $"0{abtractHour}" : abtractHour.ToString();
        string abtractMinuteStr = abtractMinute / TEN_DIGIT == 0 ? $"0{abtractMinute}" : abtractMinute.ToString();
        string abtractSecondStr = abtractSecond / TEN_DIGIT == 0 ? $"0{abtractSecond}" : abtractSecond.ToString();
        string currentTime = $"{abtractHourStr}:{abtractMinuteStr}:{abtractSecondStr}";
        return currentTime;
    }
}