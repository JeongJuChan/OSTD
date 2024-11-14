using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DailyQuestRewardData
{
    public int achievement;
    public CurrencyType currencyType;
    public int currencyAmount;

    public DailyQuestRewardData(int achievement, CurrencyType currencyType, int currencyAmount)
    {
        this.achievement = achievement;
        this.currencyType = currencyType;
        this.currencyAmount = currencyAmount;
    }
}
