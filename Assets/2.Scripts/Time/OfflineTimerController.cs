using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineTimerController : MonoBehaviour
{
    private float minTime = 300;
    private float maxTime = 10800;

    private float killCountPerSecond = 0.0025f;

    private float timePassed;

    private UIManager uiManager;
    private PushNotificationManager pushNotificationManager;
    private RewardManager rewardManager;

    private UI_OffLineReward uI_OffLineReward;

    private DateTime startTime;
    private string startTimeStr;

    public void StartApplication()
    {
        SetReferences();
        InitKey();
    }

    private void SetReferences()
    {
        uiManager = UIManager.instance;
        pushNotificationManager = PushNotificationManager.instance;
        rewardManager = RewardManager.instance;

        uI_OffLineReward = uiManager.GetUIElement<UI_OffLineReward>();
        uI_OffLineReward.Initialize();
        maxTime = uI_OffLineReward.GetMaxTime();
        minTime = uI_OffLineReward.GetMinTime();
    }

    private void InitKey()
    {
        if (!PlayerPrefs.HasKey("OfflineTimeStr_" + Application.productName)) TimeReset();
        else RefilKey();
    }

    public void RefilKey()
    {
        TimeLoad();

        TimeSpan ts = DateTime.Now - startTime;
        timePassed = (float)ts.TotalSeconds;

        ManagePushRewards();

        if (timePassed >= minTime)
        {
            OfflinePanelOpen();
            TimeReset();
        }
    }

    private void TimeLoad()
    {
        startTimeStr = PlayerPrefs.GetString("OfflineTimeStr_" + Application.productName);
        startTime = DateTime.Parse(startTimeStr);
    }

    public void TimeReset()
    {
        startTime = DateTime.Now;
        startTimeStr = startTime.ToString();
        PlayerPrefs.SetString("OfflineTimeStr_" + Application.productName, startTimeStr);
    }

    private void OfflinePanelOpen()
    {
        timePassed = Mathf.Min(timePassed, maxTime);
        int killCount = (int)(timePassed * killCountPerSecond);

        uI_OffLineReward.SetUI(killCount, (int)timePassed);
        uI_OffLineReward.OpenUI();
    }

    public void OnClickBtn_CloseOfflinePanel()
    {
        TimeReset();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void ManagePushRewards()
    {
        List<PushNotesDataSO> pushNotesDataSOs = pushNotificationManager.GetUnrecievedRewardDatas(((int)timePassed) / 3500);

        foreach (PushNotesDataSO data in pushNotesDataSOs)
        {
            pushNotificationManager.SetRewardRecieved(data.name);

            if (data.RewardType == RewardType.None) continue;

            // rewardManager.GiveReward(data.RewardType, data.Amount);
        }

        // rewardManager.ShowRewardPanel();
    }
}
