using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class StageInGameDataHandler
{
    public event Action<BigInteger> OnUpdateCurrencyUI;

    private BigInteger inGameCurrencyAmount = 0;

    private BigInteger highestGold;

    public event Action<BigInteger> OnUpdateHighestGold;

    private BigInteger defaultAdsGold = 0;

    private Vector2 ui_LosePanelRectPos;

    public void Init()
    {
        defaultAdsGold = new BigInteger(Consts.DEFAULT_ADS_GOLD);
        highestGold = DataBaseManager.instance.Load(Consts.HIGHEST_INGAME_GOLD, defaultAdsGold);
        highestGold = highestGold < defaultAdsGold ? defaultAdsGold : highestGold;
        OnUpdateHighestGold?.Invoke(highestGold);
    }

    public void ClearReset()
    {
        highestGold = defaultAdsGold;
        OnUpdateHighestGold?.Invoke(highestGold);
        DataBaseManager.instance.Save(Consts.HIGHEST_INGAME_GOLD, highestGold);
        inGameCurrencyAmount = 0;
        OnUpdateCurrencyUI?.Invoke(inGameCurrencyAmount);
    }

    public void Reset()
    {
        UpdateInGameCurrencyToMain();
        highestGold = inGameCurrencyAmount > highestGold ? inGameCurrencyAmount : highestGold;
        OnUpdateHighestGold?.Invoke(highestGold);
        inGameCurrencyAmount = 0;
        OnUpdateCurrencyUI?.Invoke(inGameCurrencyAmount);
    }

    public void SetLosePanelRect(Vector2 losePanelRectPos)
    {
        ui_LosePanelRectPos = losePanelRectPos;
    }

    public void UpdateHighestGoldUI()
    {
        OnUpdateHighestGold?.Invoke(highestGold);
    }

    public void UpdateInGameCurrencyToMain()
    {
        if (inGameCurrencyAmount == 0)
        {
            return;
        }
        
        RewardMovingController.instance.MovingCurrency(Consts.REWARD_MOVING_CURRENCY_COUNT, CurrencyType.Gold, ui_LosePanelRectPos, inGameCurrencyAmount);
    }

    public void UpdateBonusGold(Vector2 pos)
    {
        RewardMovingController.instance.MovingCurrency(Consts.REWARD_MOVING_CURRENCY_COUNT, CurrencyType.Gold, pos, inGameCurrencyAmount);
    }

    public void UpdateCurrency(BigInteger amount)
    {
        inGameCurrencyAmount += amount;
        highestGold = inGameCurrencyAmount > highestGold ? inGameCurrencyAmount : highestGold;
        highestGold = highestGold < defaultAdsGold ? defaultAdsGold : highestGold;
        OnUpdateHighestGold?.Invoke(highestGold);
        DataBaseManager.instance.Save(Consts.HIGHEST_INGAME_GOLD, highestGold);
        OnUpdateCurrencyUI?.Invoke(inGameCurrencyAmount);
    }

    public BigInteger GetInGameCurrency()
    {
        return inGameCurrencyAmount;
    }

    public void GetHighestGold(Vector2 pos)
    {
        RewardMovingController.instance.MovingCurrency(Consts.REWARD_MOVING_CURRENCY_COUNT, CurrencyType.Gold, pos, highestGold);
    }
}
