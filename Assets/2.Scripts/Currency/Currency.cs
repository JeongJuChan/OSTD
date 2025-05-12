using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Keiwando.BigInteger;

public class Currency
{
    string key;

    public CurrencyInfo info { get; private set; }

    private BigInteger currentAmount;

    public event Action<BigInteger> OnCurrencyChange;

    public Currency(CurrencyInfo info)
    {
        this.info = info;
        currentAmount = info.initAmount;

        key = $"Currency_{info.type}";

        LoadData();
    }

    public bool TryUpdateCurrency(BigInteger changeValue)
    {
        if (currentAmount + changeValue >= 0)
        {
            UpdateCurrency(changeValue);
            return true;
        }
        else return false;
    }

    public void UpdateCurrency(BigInteger changeValue)
    {
        currentAmount += changeValue;

        OnCurrencyChange?.Invoke(currentAmount);

        SaveData();
    }

    public void ResetCurrency()
    {
        currentAmount = 0;
        OnCurrencyChange?.Invoke(currentAmount);
        SaveData();
    }

    public BigInteger GetCurrencyValue()
    {
        return currentAmount;
    }

    public Sprite GetIcon()
    {
        return info.icon;
    }

    public void UpdateUI()
    {
        OnCurrencyChange?.Invoke(currentAmount);
    }

    private void LoadData()
    {
        if (ES3.KeyExists(key))
        {
            string savedValue = ES3.Load<string>(key);
            currentAmount = new BigInteger(savedValue);
        }
        OnCurrencyChange?.Invoke(currentAmount);
    }

    private void SaveData()
    {
        ES3.Save(key, currentAmount.ToString(), ES3.settings);

        ES3.StoreCachedFile();
    }
}