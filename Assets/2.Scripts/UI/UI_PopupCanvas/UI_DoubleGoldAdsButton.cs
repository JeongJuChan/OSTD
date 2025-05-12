using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;

public class UI_DoubleGoldAdsButton : UI_Button
{
    public event Action OnGetDoubleGold;

    public override void Init()
    {
        base.Init();
        AddButtonAction(ShowAds);
    }

    private void ShowAds()
    {
        AdsManager.instance.ShowRewardedAdByTime(Consts.IN_GAME_DOUBLE_GOLD_ADS, (reward, adInfo) =>
        {
            FirebaseAnalytics.LogEvent($"Reward_result_gold_{AdsManager.instance.GetAdCount(Consts.IN_GAME_DOUBLE_GOLD_ADS)}");
            OnGetDoubleGold?.Invoke();
            StageManager.instance.stageInGameDataHandler.UpdateBonusGold(BoxManager.instance.transform.position);
        });
    }
}

