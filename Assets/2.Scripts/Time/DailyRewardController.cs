using System;
using UnityEngine;

public class DailyRewardController
{
    public DailyTimeCalculator dailyTimeCalculator { get; private set; }

    private string[] rewardTime;
    private string[] rewardDay;

    [SerializeField] private int rewardHour = 0;
    [SerializeField] private int rewardMinute = 0;
    [SerializeField] private int rewardSecond = 0;

    public event Action OnTimerEnd;

    public DailyRewardController()
    {
        dailyTimeCalculator = new DailyTimeCalculator();

        dailyTimeCalculator.InitCompleteTime(rewardHour, rewardMinute, rewardSecond);

        LoadSaveDatas();
    }

    public void Update()
    {
        if (GetIsGetRewardPossible())
        {
            GiveReward();
        }
    }

    private void LoadSaveDatas()
    {
        if (!ES3.KeyExists(Consts.REWARD_DAY, ES3.settings))
        {
            GiveReward();
            return;
        }
        else
        {
            if (!ES3.KeyExists(Consts.REWARD_TIME, ES3.settings))
            {
                GiveReward();
                return;
            }
            else
            {
                rewardDay = ES3.Load<string[]>(Consts.REWARD_DAY, ES3.settings);
                rewardTime = ES3.Load<string[]>(Consts.REWARD_TIME, ES3.settings);

                if (GetIsGetRewardPossible())
                {
                    GiveReward();
                }
            }
        }
    }

    private bool GetIsGetRewardPossible()
    {
        return dailyTimeCalculator.GetIsRewardPossible(rewardDay, rewardTime);
    }

    private void GiveReward()
    {
        rewardDay = Date.GetDaySplit();
        rewardTime = Date.GetTimeSplit();
        ES3.Save<string[]>(Consts.REWARD_DAY, rewardDay, ES3.settings);
        ES3.Save<string[]>(Consts.REWARD_TIME, rewardTime, ES3.settings);

        ES3.StoreCachedFile();
    }
}
