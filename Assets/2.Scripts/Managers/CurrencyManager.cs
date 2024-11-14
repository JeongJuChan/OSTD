using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Keiwando.BigInteger;

[Serializable]
public struct CurrencyInfo
{
    public CurrencyType type;
    public int initAmount;
    public Sprite icon;
}

public class CurrencyManager : MonoBehaviourSingleton<CurrencyManager>
{
    private Dictionary<CurrencyType, Currency> currencyDic;

    public void Initialize()
    {
        SetCollections();
        LoadDatas();
    }

    private void SetCollections()
    {
        currencyDic = new Dictionary<CurrencyType, Currency>();
    }

    private void LoadDatas()
    {
        CurrencyBaseDataSO baseData = Resources.Load<CurrencyBaseDataSO>("SO/CurrencyBaseData");

        foreach (CurrencyInfo info in baseData.Currencies)
        {
            Currency currency = new Currency(info);
            currencyDic[info.type] = currency;
        }
    }

    public BigInteger GetCurrencyValue(CurrencyType type)
    {
        return currencyDic[type].GetCurrencyValue();
    }

    public bool TryUpdateCurrency(CurrencyType type, BigInteger increase)
    {
        return currencyDic[type].TryUpdateCurrency(increase);
    }

    public void ResetCurrency(CurrencyType type)
    {
        currencyDic[type].ResetCurrency();
    }

    public bool TryUpdateCurrency(RewardType type, BigInteger increase)
    {
        if (EnumUtility.ChangeRewardCurrency(type) == CurrencyType.None) return false;
        return currencyDic[EnumUtility.ChangeRewardCurrency(type)].TryUpdateCurrency(increase);
    }

    public Currency GetCurrency(CurrencyType type)
    {
        return currencyDic[type];
    }

    public Currency GetCurreny(RewardType type)
    {
        return null;
    }

    public void UpdateCurrencyUI()
    {
        foreach (var currency in currencyDic.Values)
        {
            currency.UpdateUI();
        }
    }
}
