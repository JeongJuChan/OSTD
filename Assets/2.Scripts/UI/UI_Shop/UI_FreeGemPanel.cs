using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Analytics;

public class UI_FreeGemPanel : CoroutinableUI
{
    [SerializeField] private int dailyCount = 2;
    [SerializeField] private UI_Button dailyOfferGemButton;
    [SerializeField] private DailyAdsType dailyAdsType;
    [SerializeField] private RewardType rewardType;
    [SerializeField] private int rewardAmount = 10;

    private DailyRewardController dailyRewardController;

    [SerializeField] private TextMeshProUGUI remainTimeText;
    [SerializeField] private TextMeshProUGUI adsText;

    [SerializeField] private GameObject remainTimeObject;

    private Func<bool> OnGetRedDotEnable;

    public override void Init()
    {
        base.Init();
        dailyOfferGemButton.Init();
        dailyOfferGemButton.AddButtonAction(ShowAds);
        UpdateButtonInteractableState(AdsManager.instance.GetAdCount($"{dailyAdsType}") < dailyCount);
        adsText.text = $"{dailyCount - AdsManager.instance.GetAdCount($"{dailyAdsType}")} / {dailyCount}";
        dailyRewardController = new DailyRewardController();
        dailyRewardController.dailyTimeCalculator.OnUpdateDailyRewardTime += UpdateRemainTime;
    }

    public void SetReddotFunc(Func<bool> OnGetRedDotEnable)
    {
        this.OnGetRedDotEnable = OnGetRedDotEnable;
    }

    private void OnEnable() {
        if (dailyRewardController != null)
        {
            dailyRewardController.dailyTimeCalculator.ToggleCalculateDailyRemainTime(this, true, null);
        }
    }

    private void OnDisable() 
    {
        if (dailyRewardController != null)
        {
            dailyRewardController.dailyTimeCalculator.ToggleCalculateDailyRemainTime(this, false, null);
        }
    }

    private void ShowAds()
    {
        bool isRemained = false;

        AdsManager.instance.ShowRewardedAdByDay($"{dailyAdsType}", (reward, adInfo) =>
        {
            RewardMovingController.instance.MovingCurrency(1, CurrencyType.Gem, rect.position, rewardAmount);
            FirebaseAnalytics.LogEvent($"{dailyAdsType}{AdsManager.instance.GetAdCount($"{dailyAdsType}")}");
            isRemained = AdsManager.instance.GetAdCount($"{dailyAdsType}") < dailyCount;
            UpdateButtonInteractableState(isRemained);
            if (isRemained)
            {
                adsText.text = $"{dailyCount - AdsManager.instance.GetAdCount($"{dailyAdsType}")} / {dailyCount}";
            }
            else
            {
                remainTimeText.text = $"{dailyCount - AdsManager.instance.GetAdCount($"{dailyAdsType}")} / {dailyCount}";
            }
        });
        
    }

    private void UpdateButtonInteractableState(bool isActive)
    {
        dailyOfferGemButton.UpdateInteractable(isActive);
        remainTimeObject.SetActive(!isActive);
        bool isButtonEnabled = GetButtonInteractableState();
        NotificationManager.instance.SetNotification(RedDotIDType.Shop_Free_Gem_Ads, isButtonEnabled);
        if (DataBaseManager.instance.ContainsKey(Consts.SHOP_TAP_TOUCHED_GUIDE))
        {
            NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Shop, OnGetRedDotEnable.Invoke());
        }
    }

    private void UpdateRemainTime(string remainTimeText)
    {
        string[] time = remainTimeText.Split(':');
        this.remainTimeText.text = $"{time[0]}시 {time[1]}분";
    }

    public bool GetButtonInteractableState()
    {
        return dailyOfferGemButton.enabled;
    }
}
