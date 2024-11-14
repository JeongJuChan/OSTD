using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class EquipmentManager : MonoBehaviourSingleton<EquipmentManager>
{
    private Dictionary<EquipmentType, EquipmentStatData> equipmentStatdataDict = new Dictionary<EquipmentType, EquipmentStatData>();

    public Inventory inventory { get; private set; }

    private Hero hero;

    public event Action<BigInteger, BigInteger> OnUpdateHeroStatUI;

    public event Action<int, EquipmentStatData> OnUpdateCurrenEquipment;
    public event Action<int> OnRefreshPanel;

    public event Action<Sprite, string> OnUpdateEquipmentStatType;
    public event Action<Sprite, Sprite, string, string, Color, BigInteger> OnUpdateComparingCurrentEquipmentUI;
    public event Action<Sprite, Sprite, string, string, Color, BigInteger, Color, Sprite> OnUpdateComparingNewEquipmentUI;
    public event Action OnOpenComparisonPanel;
    public event Action<BigInteger> OnUpdateSellingEquipmentCurrency;

    private int currentEquipmentChosenIndex = -1;

    private EquipmentStatData newEquipmentStatData;

    private EquipmentResourceDataHandler equipmentResourceDataHandler;
    private RankDataHandler rankDataHandler;

    private RewardManager rewardManager;
    private CurrencyManager currencyManager;

    private Currency enforcePowderCurrency;

    private ResourceManager resourceManager;

    public event Action<bool> OnUpdateAutoEquipButtonInteractable;
    public event Action<bool> OnUpdateSellDupicateButtonInteractable;


    #region Initialize
    public void Init()
    {
        inventory = new Inventory();

        inventory.Init();

        resourceManager = ResourceManager.instance;

        equipmentResourceDataHandler = resourceManager.equipment;
        rankDataHandler = resourceManager.rank;

        hero = HeroManager.instance.hero;
        EquipmentType[] equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));
        equipmentStatdataDict.Clear();

        for (int i = 1; i < equipmentTypes.Length; i++)
        {
            EquipmentStatData equipmentStatData = 
                DataBaseManager.instance.Load($"{equipmentTypes[i]}{Consts.CURRENT_EQUIPMENT_STAT_DATA}", new EquipmentStatData());

            if (equipmentStatData.level == 0)
            {
                equipmentStatData = equipmentResourceDataHandler.GetEquipmentStatData(equipmentTypes[i], Rank.Common, 1);
            }
            else
            {
                equipmentStatData = equipmentResourceDataHandler.GetEquipmentStatData(equipmentStatData.equipmentType, equipmentStatData.rank, equipmentStatData.level);
            }

            UpdateEquipmentStat(equipmentTypes[i], equipmentStatData);
        }

        rewardManager = RewardManager.instance;
        currencyManager = CurrencyManager.instance;

        enforcePowderCurrency = currencyManager.GetCurrency(CurrencyType.EnforcePowder);
    }

    public void ApplyStats()
    {
        HeroStatData heroStatData = new HeroStatData(0, 0);

        BigInteger uiDamage = 0;

        foreach (EquipmentStatData equipmentStatData in equipmentStatdataDict.Values)
        {
            EquipmentType equipmentType = equipmentStatData.equipmentType;

            switch (equipmentType)
            {
                case EquipmentType.Shotgun:
                    heroStatData.damage = equipmentStatData.amount;
                    uiDamage += equipmentStatData.amount;
                    break;
                case EquipmentType.Granade:
                    HeroManager.instance.SetSkillDamage(equipmentStatData.amount);
                    uiDamage += equipmentStatData.amount;
                    break;
                case EquipmentType.Cap:
                    heroStatData.health += equipmentStatData.amount;
                    break;
                case EquipmentType.Armor:
                    heroStatData.health += equipmentStatData.amount;
                    break;
            }

            OnUpdateCurrenEquipment?.Invoke((int)equipmentType - 1, equipmentStatData);
        }

        hero.UpdateHeroStatData(heroStatData);
        OnUpdateHeroStatUI?.Invoke(uiDamage, heroStatData.health);
    }
    #endregion

    public void UpdateCurrentEquipment()
    {
        BigInteger uiDamage = 0;
        BigInteger ui_Health = 0;

        foreach (EquipmentStatData equipmentStatData in equipmentStatdataDict.Values)
        {
            EquipmentType equipmentType = equipmentStatData.equipmentType;
            OnUpdateCurrenEquipment?.Invoke((int)equipmentType - 1, equipmentStatData);
            
            if (equipmentType == EquipmentType.Shotgun || equipmentType == EquipmentType.Granade)
            {
                uiDamage += equipmentStatData.amount;
            }
            else
            {
                ui_Health += equipmentStatData.amount;
            }
        }

        OnUpdateHeroStatUI?.Invoke(uiDamage, ui_Health);
    }

    public void UpdateEquipmentStat(EquipmentType equipmentType, EquipmentStatData equipmentStatData)
    {
        if (!equipmentStatdataDict.ContainsKey(equipmentType))
        {
            equipmentStatdataDict.Add(equipmentType, equipmentStatData);
        }

        equipmentStatdataDict[equipmentType] = equipmentStatData;

        DataBaseManager.instance.Save($"{equipmentType}{Consts.CURRENT_EQUIPMENT_STAT_DATA}", equipmentStatData);

        int index = (int)equipmentStatData.equipmentType - 1;
        Sprite sprite = equipmentResourceDataHandler.GetEquipmentSprite(equipmentStatData.equipmentType, equipmentStatData.rank);
        hero.UpdateEquipmentSprite(index, sprite);

        ApplyStats();
    }

    public void UpdateCurrentEquipmentIndex(int index)
    {
        currentEquipmentChosenIndex = index;
    }

    public void LevelUpEquipment()
    {
        if (currentEquipmentChosenIndex == -1)
        {
            return;
        }

        bool isBackpack = currentEquipmentChosenIndex == (int)EquipmentType.Backpack - 1;

        EquipmentStatData equipmentStatData = equipmentStatdataDict[(EquipmentType)(currentEquipmentChosenIndex + 1)];
        int level = equipmentStatData.level;
        CurrencyType firstCurrencyType = isBackpack ? CurrencyType.Research : rewardManager.bluePrints[currentEquipmentChosenIndex];
        CurrencyType secondCurrencyType = isBackpack ? CurrencyType.Gem : CurrencyType.EnforcePowder;
        (BigInteger, BigInteger) levelUpData = isBackpack ? equipmentResourceDataHandler.GetBackpackLevelUpCost(level) : 
            equipmentResourceDataHandler.GetEquipmentLevelUpCost(level);
        BigInteger firstCurrencyCost = levelUpData.Item1;
        BigInteger secondCurrencyCost = levelUpData.Item2;

        if (currencyManager.GetCurrencyValue(firstCurrencyType) < firstCurrencyCost || enforcePowderCurrency.GetCurrencyValue() < secondCurrencyCost)
        {
            return;
        }

        ApplyResult(equipmentStatData, level, firstCurrencyType, firstCurrencyCost, secondCurrencyType, secondCurrencyCost);
        UpdateCurrentEquipment();
        DataBaseManager.instance.Save($"{equipmentStatData.equipmentType}{Consts.CURRENT_EQUIPMENT_STAT_DATA}", equipmentStatData);
        ApplyStats();
        OnRefreshPanel?.Invoke(currentEquipmentChosenIndex);
    }

    private void ApplyResult(EquipmentStatData equipmentStatData, int level, CurrencyType firstCurrencyType, BigInteger currencyCost, 
        CurrencyType secondCurrencyType, BigInteger secondTypeCost)
    {
        currencyManager.TryUpdateCurrency(firstCurrencyType, -currencyCost);
        currencyManager.TryUpdateCurrency(secondCurrencyType, -secondTypeCost);

        level++;

        EquipmentType equipmentType = equipmentStatData.equipmentType;

        EquipmentStatData newEquipmentStatData = equipmentResourceDataHandler.GetEquipmentStatData(equipmentStatData.equipmentType,
            equipmentStatData.rank, level);

        equipmentStatdataDict[equipmentType] = newEquipmentStatData;

        DataBaseManager.instance.Save($"{equipmentType}{Consts.CURRENT_EQUIPMENT_STAT_DATA}", equipmentStatData);
    }

    public void LevelUpMaxEquipment()
    {
        if (currentEquipmentChosenIndex == -1)
        {
            return;
        }

        bool isBackpack = currentEquipmentChosenIndex == (int)EquipmentType.Backpack - 1;
        EquipmentType equipmentType = (EquipmentType)(currentEquipmentChosenIndex + 1);

        while (true)
        {
            EquipmentStatData equipmentStatData = equipmentStatdataDict[equipmentType];
            int level = equipmentStatData.level;

            CurrencyType firstCurrencyType = isBackpack ? CurrencyType.Research : rewardManager.bluePrints[currentEquipmentChosenIndex];
            CurrencyType secondCurrencyType = isBackpack ? CurrencyType.Gem : CurrencyType.EnforcePowder;
            (BigInteger, BigInteger) levelUpData = isBackpack ? equipmentResourceDataHandler.GetBackpackLevelUpCost(level) :
                equipmentResourceDataHandler.GetEquipmentLevelUpCost(level);

            BigInteger firstCurrencyCost = levelUpData.Item1;
            BigInteger secondCurrencyCost = levelUpData.Item2;

            if (firstCurrencyCost == null || secondCurrencyCost == null)
            {
                break;
            }

            if (currencyManager.GetCurrencyValue(firstCurrencyType) < firstCurrencyCost || enforcePowderCurrency.GetCurrencyValue() < secondCurrencyCost)
            {
                break;
            }

            ApplyResult(equipmentStatData, level, firstCurrencyType, firstCurrencyCost, secondCurrencyType, secondCurrencyCost);
        }

        UpdateCurrentEquipment();

        foreach (EquipmentStatData equipmentStatData in equipmentStatdataDict.Values)
        {
            DataBaseManager.instance.Save($"{equipmentType}{Consts.CURRENT_EQUIPMENT_STAT_DATA}", equipmentStatData);
        }

        ApplyStats();
        OnRefreshPanel?.Invoke(currentEquipmentChosenIndex);
    }

    public void CompareEquipments(EquipmentStatData newEquipmentStatData)
    {
        EquipmentType newEquipmentType = newEquipmentStatData.equipmentType;

        if (newEquipmentType == EquipmentType.Backpack)
        {
            return;
        }

        EquipmentStatData currentEquipmentStatData = equipmentStatdataDict[newEquipmentType];
        EquipmentType currentEquipmentType = currentEquipmentStatData.equipmentType;

        Rank newRank = newEquipmentStatData.rank;
        Rank currentRank = currentEquipmentStatData.rank;

        Sprite newEquipmentSprite = equipmentResourceDataHandler.GetEquipmentSprite(newEquipmentType, newRank);
        Sprite currentEquipmentSprite = equipmentResourceDataHandler.GetEquipmentSprite(currentEquipmentType, currentRank);

        Sprite newRankSprite = rankDataHandler.GetRankBackgroundSprite(newRank);
        Sprite currentRankSprite = rankDataHandler.GetRankBackgroundSprite(currentRank);

        Color newColor = rankDataHandler.GetRankColor(newRank);
        Color currentColor = rankDataHandler.GetRankColor(currentRank);

        BigInteger newStat = newEquipmentStatData.amount;
        BigInteger currentStat = currentEquipmentStatData.amount;

        Color color = Color.white;
        Sprite arrowSprite = null;

        if (newStat > currentStat)
        {
            color = Color.green;
            arrowSprite = equipmentResourceDataHandler.GetUpperArrow();
        }
        else if (newStat < currentStat)
        {
            color = Color.red;
            arrowSprite = equipmentResourceDataHandler.GetLowerArrow();
        }

        OnUpdateComparingNewEquipmentUI?.Invoke(newEquipmentSprite, newRankSprite, EnumUtility.GetEquipmentTypeKR(newEquipmentType),
            EnumUtility.GetRankKR(newRank), newColor, newStat, color, arrowSprite);

        OnUpdateComparingCurrentEquipmentUI?.Invoke(currentEquipmentSprite, currentRankSprite, EnumUtility.GetEquipmentTypeKR(currentEquipmentType),
            EnumUtility.GetRankKR(currentRank), currentColor, currentStat);


        this.newEquipmentStatData = newEquipmentStatData;

        StatType statType = newEquipmentType == EquipmentType.Shotgun || newEquipmentType == EquipmentType.Granade ? StatType.Attack : StatType.Health;
        Sprite statTypeSprite = equipmentResourceDataHandler.GetStatTypeSprite(statType);

        OnUpdateEquipmentStatType?.Invoke(statTypeSprite, EnumUtility.GetStatTypeKR(statType));
        OnOpenComparisonPanel?.Invoke();
    }

    public void EquipNewEquipment()
    {
        bool isNewEquipmentUpperRank = GetIsNewEquipmentUpperRank(newEquipmentStatData.equipmentType, newEquipmentStatData.rank);
        OnUpdateAutoEquipButtonInteractable?.Invoke(!isNewEquipmentUpperRank);
        OnUpdateSellDupicateButtonInteractable?.Invoke(isNewEquipmentUpperRank);
        AddCurrentEquipmentInInventory(newEquipmentStatData.equipmentType);
        inventory.RemoveEquipment(newEquipmentStatData);
        UpdateEquipmentStat(newEquipmentStatData.equipmentType, newEquipmentStatData);
        inventory.UpdateInventoryUI();
    }

    public void AddCurrentEquipmentInInventory(EquipmentType equipmentType)
    {
        EquipmentStatData currentEquipmentStatData = equipmentStatdataDict[equipmentType];
        inventory.AddEquipment(currentEquipmentStatData);
    }

    public void AutoEquip()
    {
        inventory.EquipBest();
        UpdateSellDuplicateButtonActiveState();
    }

    public bool GetIsNewEquipmentUpperRank(EquipmentType equipmentType, Rank rank)
    {
        return rank > equipmentStatdataDict[equipmentType].rank;
    }

    public bool GetIsEquipmentUnderRank(EquipmentType equipmentType, Rank rank)
    {
        return rank <= equipmentStatdataDict[equipmentType].rank;
    }

    public void SellDuplicates()
    {
        BigInteger totalCurrency = 0;
        foreach (EquipmentStatData equipmentStatData in equipmentStatdataDict.Values)
        {
            totalCurrency += inventory.GetCurrencyBySellingDuplicate(equipmentStatData.equipmentType, equipmentStatData.rank);
        }

        inventory.UpdateInventoryUI();
        rewardManager.AddReward(new RewardData(RewardType.EnforcePowder, totalCurrency));
        rewardManager.GetReward();
        inventory.SaveEquipmentDatas();
        UpdateAutoEquipButtonActiveState();
    }

    public void AddEquipmentInInventory(EquipmentType equipmentType, Rank rank)
    {
        int level = equipmentStatdataDict[equipmentType].level;
        inventory.AddEquipment(equipmentResourceDataHandler.GetEquipmentStatData(equipmentType, rank, level));
    }

    public void UpdateAutoEquipButtonActiveState()
    {
        OnUpdateAutoEquipButtonInteractable?.Invoke(inventory.GetCanAutoEquippable()); 
    }

    public void UpdateSellDuplicateButtonActiveState()
    {
        OnUpdateSellDupicateButtonInteractable?.Invoke(inventory.GetCanSellingDuplicate());
    }

    public EquipmentStatData GetEquipmentStatData(EquipmentType equipmentType)
    {
        return equipmentStatdataDict[equipmentType];
    }

    public BigInteger GetBackpackGold()
    {
        return equipmentStatdataDict[EquipmentType.Backpack].amount;
    }
}
