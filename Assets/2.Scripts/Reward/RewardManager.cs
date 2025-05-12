using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RewardManager : MonoBehaviourSingleton<RewardManager>
{
    private RewardResourceDataHandler rewardResourceDataHandler;
    private Dictionary<CurrencyType, BigInteger> currentCurrencyDict = new Dictionary<CurrencyType, BigInteger>();

    public event Action<CurrencyType, BigInteger> OnGetReward;
    public event Action<Sprite, Sprite> OnAddEquipmentUI;

    public CurrencyType[] bluePrints { get; private set; }
    private BigInteger[] bluePrintAmounts;

    public event Func<EquipmentData> OnGetRandomEquipmentData;

    private RankDataHandler rankDataHandler;
    private EquipmentResourceDataHandler equipmentResourceDataHandler;

    private int bluePrintLength;

    public event Action<int, bool> OnUnlockHero;

    public void Init()
    {
        rewardResourceDataHandler = ResourceManager.instance.reward;
        currentCurrencyDict.Clear();
        StageManager.instance.OnUpdateReward += TryAddReward;

        bluePrints = new CurrencyType[]
        {
            CurrencyType.ShotGunBluePrint,
            CurrencyType.GranadeBluePrint,
            CurrencyType.CapBluePrint,
            CurrencyType.ArmorBluePirnt,
        };

        bluePrintLength = bluePrints.Length;
        bluePrintAmounts = new BigInteger[bluePrintLength];
        for (int i = 0; i < bluePrintAmounts.Length; i++)
        {
            bluePrintAmounts[i] = 0;
        }

        rankDataHandler = ResourceManager.instance.rank;
        equipmentResourceDataHandler = ResourceManager.instance.equipment;
    }

    private void Reset()
    {
        currentCurrencyDict.Clear();
        for (int i = 0; i < bluePrintAmounts.Length; i++)
        {
            bluePrintAmounts[i] = 0;
        }
    }

    private void TryAddReward(int stageNum, int checkPointNum)
    {
        List<RewardData> rewardDatas = rewardResourceDataHandler.GetRewardDatas(stageNum, checkPointNum);

        if (rewardDatas == null || rewardDatas.Count == 0)
        {
            return;
        }

        foreach (RewardData rewardData in rewardDatas)
        {
            AddReward(rewardData);
        }
    }

    public void AddReward(RewardData rewardData)
    {
        RewardType rewardType = rewardData.rewardType;
        BigInteger rewardAmount = rewardData.rewardCount;

        if (rewardData.rewardType == RewardType.BluePrint)
        {
            AddBluePrintRewards(rewardAmount);
            return;
        }
        else if (rewardData.rewardType == RewardType.Equipment)
        {
            AddEquipmentRewards(rewardAmount);
            return;
        }

        CurrencyType currencyType = EnumUtility.GetEqualValue<CurrencyType, RewardType>(rewardType);
        if (!currentCurrencyDict.ContainsKey(currencyType))
        {
            currentCurrencyDict.Add(currencyType, 0);
        }

        currentCurrencyDict[currencyType] += rewardAmount;
    }

    public void GetReward()
    {
        if (currentCurrencyDict.Count == 0)
        {
            return;
        }

        foreach (var rewardData in currentCurrencyDict)
        {
            CurrencyType currencyType = rewardData.Key;
            BigInteger amount = rewardData.Value;
            CurrencyManager.instance.TryUpdateCurrency(currencyType, amount);
            OnGetReward?.Invoke(currencyType, amount);
        }

        if (DataBaseManager.instance.ContainsKey(Consts.EQUIPMENT_ENFORCE_REWARDED))
        {
            if (!DataBaseManager.instance.ContainsKey(Consts.HERO_TAP_TOUCHED_GUIDE))
            {
                OnUnlockHero?.Invoke(1, false);
                GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.HERO_TAP_TOUCHED_GUIDE, true);
            }
        }

        Reset();
    }

    private void AddBluePrintRewards(BigInteger totalAmount)
    {
        for (int i = 0; i < totalAmount; i++)
        {
            int index = UnityEngine.Random.Range(0, bluePrints.Length);
            bluePrintAmounts[index]++;
        }

        for (int i = 0; i < bluePrints.Length; i++)
        {
            BigInteger bluePrintAmount = bluePrintAmounts[i];
            if (bluePrintAmount == 0)
            {
                continue;
            }

            CurrencyType currencyType = bluePrints[i];
            if (!currentCurrencyDict.ContainsKey(currencyType))
            {
                currentCurrencyDict.Add(currencyType, 0);
            }

            currentCurrencyDict[currencyType] += bluePrintAmount;
        }
    }

    private void AddEquipmentRewards(BigInteger totalAmount)
    {
        for (int i = 0; i < totalAmount; i++)
        {
            // TODO: 실제 아이템 생기게
            EquipmentData equipmentData = OnGetRandomEquipmentData.Invoke();
            // UI

            UpdateEquipmentToReward(equipmentData);
        }
    }

    private void UpdateEquipmentToReward(EquipmentData equipmentData)
    {
        Sprite equipmentSprite = equipmentResourceDataHandler.GetEquipmentSprite(equipmentData.equipmentType, equipmentData.rank);
        Sprite rankSprite = rankDataHandler.GetRankBackgroundSprite(equipmentData.rank);

        EquipmentManager.instance.AddEquipmentInInventory(equipmentData.equipmentType, equipmentData.rank);
        OnAddEquipmentUI?.Invoke(equipmentSprite, rankSprite);
    }

    public void AddTutorialGranadeRewards()
    {
        UpdateEquipmentToReward(new EquipmentData(EquipmentType.Granade, Rank.Great));
        CurrencyType currencyType = bluePrints[1];
        if (!currentCurrencyDict.ContainsKey(currencyType))
        {
            currentCurrencyDict.Add(currencyType, 0);
        }

        currentCurrencyDict[currencyType] += 1;
    }

    #region Legacy
    // private RewardResultPanel rewardResultPanel;
    // private RewardInfoProvider rewardInfoProvider;
    // public Func<int, int, int, int> offlineRewardCallback;

    // public void Initialize()
    // {
    //     rewardInfoProvider = new RewardInfoProvider();

    //     rewardResultPanel = UIManager.instance.GetUIElement<RewardResultPanel>();

    //     rewardResultPanel.Initalize();

    // }

    // // public void AddReward(RewardType type, int amount)
    // // {
    // //     rewards.Add((rewardInfoProvider.GetRewardInfo(type), amount));
    // // }

    // public IRewardInfo GetRewardInfo(RewardType type)
    // {
    //     return rewardInfoProvider.GetRewardInfo(type);
    // }

    // public RewardSlot GetRewardSlot(RewardType type, int amount)
    // {
    //     return rewardResultPanel.GetRewardSlot(type, amount, rewardInfoProvider.GetRewardInfo(type));
    // }

    // public void GiveReward(RewardType type, int amount)
    // {
    //     if (rewardInfoProvider.GetRewardInfo(type) == null) return;
    //     IRewardInfo rewardInfo = rewardInfoProvider.GetRewardInfo(type);
    //     RewardSlot slot = rewardResultPanel.GetRewardSlot(type, amount, rewardInfo);
    //     slot.SetUI(rewardInfo, amount);
    // }

    // public void GiveReward(RewardType type, BigInteger amount)
    // {
    //     if (rewardInfoProvider.GetRewardInfo(type) == null) return;
    //     IRewardInfo rewardInfo = rewardInfoProvider.GetRewardInfo(type);
    //     RewardSlot slot = rewardResultPanel.GetRewardSlot(type, amount, rewardInfo);
    //     slot.SetUI(rewardInfo, amount);
    // }

    // public Sprite GetIcon(RewardType type)
    // {
    //     return rewardInfoProvider.GetRewardInfo(type).GetIcon();
    // }

    // public void ShowRewardPanel()
    // {
    //     rewardResultPanel.ShowRewardSlot();
    // }

    // public void ShowStageClearRewardPanel()
    // {
    //     rewardResultPanel.ShowStageClearRewardSlot();
    // }
    #endregion
}

