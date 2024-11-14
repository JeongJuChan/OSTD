using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class UI_CurrencyRewardPanel : UI_Pooler<UI_RewardCurrencyPanel>
{
    private CurrencyManager currencyManager;

    public override void Init()
    {
        base.Init();
        currencyManager = CurrencyManager.instance;
    }

    public void AddReward(CurrencyType currencyType, BigInteger amount)
    {
        UI_RewardCurrencyPanel ui_RewardCurrencyPanel = GetUI();
        ui_RewardCurrencyPanel.UpdateCurrency(currencyManager.GetCurrency(currencyType).GetIcon(), amount);
    }
}
