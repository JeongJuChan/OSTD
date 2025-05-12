using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Summon
{
    public SummonType type;

    protected SummonProbabilityDataSO proportions;
    protected Rank[] ranks = new Rank[100000];


    protected int currentSummonExp;
    protected int currentSummonLevel;

    protected int maxSummonExp;
    protected int maxLevel;

    private event Action<int> OnExpChange;
    private event Action<int> OnLevelChange;
    private event Action<int> OnMaxExpChange;

    protected SummonUnitInfo adsInfo;
    protected SummonUnitInfo smallInfo;
    protected SummonUnitInfo largeInfo;

    protected SummonDataSO data;

    private static System.Random random = new System.Random();

    protected UI_SummonResult resultUI;


    public Summon(SummonDataSO data)
    {
        this.data = data;

        adsInfo = data.AdsSummonInfo;
        smallInfo = data.SmallSummonInfo;
        largeInfo = data.LargeSummonInfo;
    }


    public virtual void Initialize()
    {
        LoadSummonInfo();
        GetProportionData();

        OnExpChange?.Invoke(currentSummonExp);
        OnLevelChange?.Invoke(currentSummonLevel);
        OnMaxExpChange?.Invoke(maxSummonExp);

        ResourceManager.instance.rank.SetRanks(currentSummonLevel);
    }

    public void AddEventCallbacks(Action<int> UpdateExp, Action<int> UpdateLevel, Action<int> UpdateMaxExp)
    {
        OnExpChange += UpdateExp;
        OnLevelChange += UpdateLevel;
        OnMaxExpChange += UpdateMaxExp;
    }

    public void AdsSummon()
    {
        SummonItem(adsInfo.quantity);
        UpdateSummonExp(adsInfo.quantity);
    }

    public void SmallSummon()
    {
        if (!CurrencyManager.instance.TryUpdateCurrency(smallInfo.currencyType, -smallInfo.price)) return;

        SummonItem(smallInfo.quantity);
        UpdateSummonExp(smallInfo.quantity);
    }

    public void LargeSummon()
    {
        if (!CurrencyManager.instance.TryUpdateCurrency(largeInfo.currencyType, -largeInfo.price)) return;

        SummonItem(largeInfo.quantity);
        UpdateSummonExp(largeInfo.quantity);
    }

    public void SmallSummonByType(CurrencyType currencyType, int price)
    {
        if (!CurrencyManager.instance.TryUpdateCurrency(currencyType, -price)) 
        {
            resultUI.ActivateSummonButtons(true);
            return;
        }

        SummonItem(currencyType, smallInfo.quantity, price, 1);
        UpdateSummonExp(smallInfo.quantity);
    }

    public void LargeSummonByType(CurrencyType currencyType, int price)
    {
        if (!CurrencyManager.instance.TryUpdateCurrency(currencyType, -price))
        {
            resultUI.ActivateSummonButtons(true);
            return; 
        }

        SummonItem(currencyType, largeInfo.quantity, price, 2);
        UpdateSummonExp(largeInfo.quantity);
    }

    protected abstract void SummonItem(int quantity);
    protected abstract void SummonItem(CurrencyType currencyType, int quantity, int price, int quantityNum);

    protected static int GetRandomInt(int rangeAddOne)
    {
        return random.Next(0, rangeAddOne);
    }
    protected void UpdateSummonExp(int increase)
    {
        currentSummonExp += increase;

        UpdateSummonLevel();

        OnExpChange?.Invoke(currentSummonExp);

        SaveSummonInfo();
    }

    protected void UpdateExpEvents()
    {
        OnExpChange?.Invoke(currentSummonExp);
        OnLevelChange?.Invoke(currentSummonLevel);
        OnMaxExpChange?.Invoke(maxSummonExp);
    }

    protected virtual void UpdateSummonLevel()
    {
        if (currentSummonExp >= maxSummonExp)
        {
            if (currentSummonLevel == maxLevel)
            {
                currentSummonExp = maxSummonExp;
            }
            else
            {
                currentSummonExp -= maxSummonExp;
                currentSummonLevel++;
                OnLevelChange?.Invoke(currentSummonLevel);
                ResourceManager.instance.rank.SetRanks(currentSummonLevel);
                UpdateSummonMaxExp();
                SetRanks();
            }
        }
    }

    protected int[] GetCurrentProportion()
    {
        int[] proportion = proportions.GetProbabillitiesOfLevel(currentSummonLevel);

#if UNITY_EDITOR
        #region Assertion
        Debug.Assert(proportion != null, "Proportion of current level does not exist.");

        int sum = 0;
        foreach (int num in proportion)
        {
            sum += num;
        }
        Debug.Assert(sum == 100000, "Elements of the proportion does not sum up 1000.");
        #endregion
#endif
        return proportion;
    }

    protected void SetRanks()
    {
        int[] proportion = GetCurrentProportion();

        int count = 0;
        for (int i = 0; i < proportion.Length; i++)
        {
            int repetition = proportion[i];
            for (int j = 0; j < repetition; j++)
            {
                ranks[count] = (Rank)(i + 1);
                count++;
            }
        }
    }

    private void UpdateSummonMaxExp()
    {
        maxSummonExp += maxSummonExp / 2;
        OnMaxExpChange?.Invoke(maxSummonExp);
    }

    public SummonUnitInfo GetSmallSummonInfo()
    {
        return smallInfo;
    }

    public SummonUnitInfo GetLargeSummonInfo()
    {
        return largeInfo;
    }

    public SummonUnitInfo GetAdsSummonInfo()
    {
        return adsInfo;
    }

    protected void GetProportionData()
    {
        proportions = Resources.Load<SummonProbabilityDataSO>($"SO/{type}SummonProbabilityDataSO");
        SetRanks();
    }

    private void SaveSummonInfo()
    {
        ES3.Save($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_EXP}", currentSummonExp, ES3.settings);
        ES3.Save($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_LEVEL}", currentSummonLevel, ES3.settings);
        ES3.Save($"{Consts.SUMMON_PREFIX}{type}{Consts.MAX_SUMMON_EXP}", maxSummonExp, ES3.settings);

        ES3.StoreCachedFile();
    }

    private void LoadSummonInfo()
    {
        currentSummonExp = ES3.KeyExists($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_EXP}") ? ES3.Load<int>($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_EXP}") : 0;
        currentSummonLevel = ES3.KeyExists($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_LEVEL}") ? ES3.Load<int>($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_LEVEL}") : 1;
        maxSummonExp = ES3.KeyExists($"{Consts.SUMMON_PREFIX}{type}{Consts.MAX_SUMMON_EXP}") ? ES3.Load<int>($"{Consts.SUMMON_PREFIX}{type}{Consts.MAX_SUMMON_EXP}") : 100;
    }

    public void RemoveEventCallbacks()
    {
        OnExpChange = null;
        OnLevelChange = null;
        OnMaxExpChange = null;
    }

    public void UpdateResultUI(CurrencyType currentSmallCurrencyType, CurrencyType currentLargeCurrencyType, bool isSmallAvailable, 
        bool isLargeAvailable, BigInteger smallPrice, BigInteger largePrice)
    {
        resultUI.UpdateSummonInfo(currentSmallCurrencyType, currentLargeCurrencyType, isSmallAvailable, isLargeAvailable, smallPrice, 
            largePrice, smallInfo, largeInfo);
    }
}
