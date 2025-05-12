using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;

public class UI_BonusGoldPanel : UI_Base
{
    [SerializeField] private int hours = 0;
    [SerializeField] private int minutes = 5;
    [SerializeField] private int seconds = 0;

    private int totalSeconds;

    [SerializeField] private TextMeshProUGUI remainTimeText;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private GameObject remainTimeObject;


    [SerializeField] private Button showAdsButton;

    [SerializeField] private Image adsImage;

    private DateTime startTime;

    private bool isTimerOn;

    private void Update() 
    {
        if (isTimerOn)
        { 
            UpdateTimer();
        }
    }

    public override void Init()
    {
        base.Init();

        totalSeconds = Date.GetSecondsByTime(hours, minutes, seconds);
        if (DataBaseManager.instance.ContainsKey(Consts.BONUS_GOLD_TIME))
        {
            string bonusGoldStr = DataBaseManager.instance.Load(Consts.BONUS_GOLD_TIME, "0");
            startTime = DateTime.Parse(bonusGoldStr);
            UpdateTimerActiveState(true);
            ChangeButtonInteractableState(false);
            UpdateTimer();
        }
        else
        {
            ChangeButtonInteractableState(true);
        }

        showAdsButton.onClick.AddListener(ShowAds);
        GameManager.instance.OnReset += () => UpdateActiveState(true);
        GameManager.instance.OnStart += () => UpdateActiveState(false);
    }

    private void ShowAds()
    {
        AdsManager.instance.ShowRewardedAdByTime(Consts.IN_GAME_GOLD_ADS, (reward, adInfo) =>
        {
            FirebaseAnalytics.LogEvent($"Reward_battle_gold_{AdsManager.instance.GetAdCount(Consts.IN_GAME_GOLD_ADS)}");
            startTime = DateTime.Now;
            DataBaseManager.instance.Save(Consts.BONUS_GOLD_TIME, startTime.ToString());
            UpdateTimerActiveState(true);
            ChangeButtonInteractableState(false);
            StageManager.instance.stageInGameDataHandler.GetHighestGold(rect.position);
        });
    }

    private void UpdateTimerActiveState(bool isOn)
    {
        isTimerOn = isOn;
    }

    private void UpdateTimer()
    {
        if (startTime == null)
        {
            UpdateTimerActiveState(false);
            ChangeButtonInteractableState(true);
            return;
        }

        TimeSpan timeSpan = DateTime.Now - startTime;
        if (timeSpan.TotalSeconds < totalSeconds)
        {
            UpdateRemainTime(Date.GetTimeBySeconds(totalSeconds - (int)timeSpan.TotalSeconds));
        }
        else
        {
            UpdateTimerActiveState(false);
            ChangeButtonInteractableState(true);
            if (StageManager.instance.stageInGameDataHandler != null)
            {
                StageManager.instance.stageInGameDataHandler.UpdateHighestGoldUI();
            }
            else
            {
                BigInteger gold = new BigInteger(DataBaseManager.instance.Load(Consts.HIGHEST_INGAME_GOLD, Consts.DEFAULT_ADS_GOLD));
                UpdateInGameHightestGold(gold);
            }
        }
    }

    private void ChangeButtonInteractableState(bool isInteractable)
    {
        showAdsButton.interactable = isInteractable;
        adsImage.gameObject.SetActive(isInteractable);
        remainTimeObject.SetActive(!isInteractable);
    }

    private void UpdateRemainTime(string[] time)
    {
        remainTimeText.text = $"{time[1]}분 {time[2]}초";
    }

    public void UpdateInGameHightestGold(BigInteger amount)
    {
        goldText.text = $"+{amount}";
    }

    private void UpdateActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
