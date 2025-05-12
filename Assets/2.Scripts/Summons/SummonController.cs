using System;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class SummonController : MonoBehaviour
{
    private bool isInitialized = false;
    [SerializeField] private SummonSlot[] slots;
    private Dictionary<SummonType, Summon> summonDic;

    private SkillManager skillManager;

    public void Initalize()
    {
        if (isInitialized) return;

        GetManagers();
        AddSummonDatas();
        InitializeSlots();
        InitializeSummon();

        CurrencyManager.instance.GetCurrency(CurrencyType.Gem).OnCurrencyChange += UpdateRedDot;

        isInitialized = true;
    }

    public void InitUnlock()
    {
        foreach (SummonSlot summonSlot in slots)
        {
            summonSlot.InitUnlock();
        }
    }

    private void UpdateRedDot(BigInteger amount)
    {
        /*if (amount >= 600)
        {
            NotificationManager.instance.SetNotification(RedDotIDType.SkillSummonButton_15, true);
            NotificationManager.instance.SetNotification(RedDotIDType.SkillSummonButton_30, true);
            NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton, true);
            NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton_12, true);
        }
        else if (amount >= 300)
        {
            NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton, true);
            NotificationManager.instance.SetNotification(RedDotIDType.SkillSummonButton_15, true);

            NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton_12, false);
            NotificationManager.instance.SetNotification(RedDotIDType.SkillSummonButton_30, false);
        }
        else
        {
            NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton, false);
            NotificationManager.instance.SetNotification(RedDotIDType.SkillSummonButton_15, false);
            NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton_12, false);
            NotificationManager.instance.SetNotification(RedDotIDType.SkillSummonButton_30, false);
        }*/
    }

    private void OnEnable()
    {
        if (GameManager.instance.isInitializing)
        {
            UpdateRedDot(CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem));
        }
    }

    private void GetManagers()
    {
        skillManager = FindAnyObjectByType<SkillManager>();
    }

    public void AdsSummon(SummonType type)
    {
        summonDic[type].AdsSummon();
    }

    public void SmallSummon(SummonType type)
    {
        summonDic[type].SmallSummon();
    }

    public void LargeSummon(SummonType type)
    {
        summonDic[type].LargeSummon();
    }

    public void SmallSummon(SummonType type, CurrencyType currencyType, int price)
    {
        summonDic[type].SmallSummonByType(currencyType, price);
    }

    public void LargeSummon(SummonType type, CurrencyType currencyType, int price)
    {
        summonDic[type].LargeSummonByType(currencyType, price);
    }

    public void AddSummonCallbacks(SummonType type, Action<int> exp, Action<int> level, Action<int> maxExp)
    {
        summonDic[type].AddEventCallbacks(UpdateExp: exp, UpdateLevel: level, UpdateMaxExp: maxExp);
    }

    private void AddSummonDatas()
    {
        summonDic = new Dictionary<SummonType, Summon>();
        // for (int i = 0; i < summonTypes.Length; i++)
        // {
        //     summonDic[summonTypes[i]] = CreateSummonDatas(summonTypes[i]);
        // }

        summonDic[SummonType.Equipment] = CreateSummonDatas(SummonType.Equipment);
    }

    private Summon CreateSummonDatas(SummonType type)
    {
        Summon summon;
        SummonDataSO data = Resources.Load<SummonDataSO>($"SO/{type}SummonData");

        switch (type)
        {
            case SummonType.Equipment:
                summon = new EquipmentSummon(data);
                break;
            default:
                summon = null;
                break;
        }

        return summon;
    }

    private void InitializeSlots()
    {
        foreach (SummonSlot slot in slots)
        {
            slot.Initialize(this);
        }
    }

    private void InitializeSummon()
    {
        foreach (KeyValuePair<SummonType, Summon> kvp in summonDic)
        {
            if (kvp.Value != null) kvp.Value.Initialize();
        }
    }

    public SummonUnitInfo GetSmallSummonInfo(SummonType type)
    {
        return summonDic[type].GetSmallSummonInfo();
    }

    public SummonUnitInfo GetLargeSummonInfo(SummonType type)
    {
        return summonDic[type].GetLargeSummonInfo();
    }

    public SummonUnitInfo GetAdsSummonInfo(SummonType type)
    {
        return summonDic[type].GetAdsSummonInfo();
    }

    public void RemoveCallbacks()
    {
        foreach (KeyValuePair<SummonType, Summon> kvp in summonDic)
        {
            if (kvp.Value != null) kvp.Value.RemoveEventCallbacks();
        }
    }

    public void UpdateResultUI(SummonType summonType, CurrencyType currentSmallCurrencyType, CurrencyType currentLargeCurrencyType, 
        bool isSmallAvailable, bool isLargeAvailable, BigInteger smallPrice, BigInteger largePrice)
    {
        summonDic[summonType].UpdateResultUI(currentSmallCurrencyType, currentLargeCurrencyType, isSmallAvailable, isLargeAvailable, 
            smallPrice, largePrice);
    }
}