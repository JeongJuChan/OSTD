using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;

public class UI_EnergyAdsPanel : UI_Base
{
    [SerializeField] private Button adsButton;
    [SerializeField] private TextMeshProUGUI energyText;

    public event Action OnAddEnergyByAds;

    public override void Init()
    {
        adsButton.onClick.AddListener(() => ShowAds());
        GameManager.instance.OnStart += () => UpdateButtonActiveState(true);
        GameManager.instance.OnReset += () => UpdateButtonActiveState(false);
        UpdateButtonActiveState(false);
    }

    private void UpdateButtonActiveState(bool isActive)
    {
        adsButton.gameObject.SetActive(isActive);
    }

    private void ShowAds()
    {
        AdsManager.instance.ShowRewardedAdByTime(Consts.IN_GAME_ENERGY_ADS, (reward, adInfo) =>
        {
            FirebaseAnalytics.LogEvent($"Reward_ingame_energe_{AdsManager.instance.GetAdCount(Consts.IN_GAME_GOLD_ADS)}");
            OnAddEnergyByAds?.Invoke();
            UpdateButtonActiveState(false);
        });
    }

    public void UpdateEnergyAdsText(int energyAmount)
    {
        energyText.text = $"+{energyAmount}";
    }
}
