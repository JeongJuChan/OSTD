using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentSummon : Summon
{
    private HashSet<EquipmentData> hashSummonedItems;
    private List<EquipmentData> summonedItems;
    private Dictionary<EquipmentData, int> summonedCounts;

    private EquipmentManager equipmentManager;

    public EquipmentSummon(SummonDataSO data) : base(data)
    {
        resultUI = UIManager.instance.GetUIElement<UI_SummonResult>();

        hashSummonedItems = new HashSet<EquipmentData>();
        summonedItems = new List<EquipmentData>();
        summonedCounts = new Dictionary<EquipmentData, int>();

        type = SummonType.Equipment;
        maxLevel = ResourceManager.instance.rank.GetProbabilityMaxLevel();

        ranks = new Rank[100000];

        equipmentManager = EquipmentManager.instance;

        RewardManager.instance.OnGetRandomEquipmentData += GetRandomEquipmentData;
    }

    protected override void SummonItem(int quantity)
    {
        summonedCounts.Clear();
        summonedItems.Clear();
        hashSummonedItems.Clear();

        resultUI.SetResultUI(this.type, () => SmallSummon(), () => LargeSummon(), () => AdsSummon());
        resultUI.SetButtonInfo(smallInfo.currencyType, small: smallInfo, large: largeInfo);

        for (int i = 0; i < quantity; i++)
        {
            EquipmentType equipmentType = ResourceManager.instance.equipment.GetRandomEquipmentType();
            Rank rank = ResourceManager.instance.rank.GetRandomRank();
            EquipmentData equipmentData = new EquipmentData(equipmentType, rank);

            summonedItems.Add(equipmentData);
            hashSummonedItems.Add(equipmentData);
            if (summonedCounts.ContainsKey(equipmentData)) summonedCounts[equipmentData]++;
            else summonedCounts[equipmentData] = 1;
        }

        foreach (EquipmentData equipment in summonedItems)
        {
            resultUI.AddSlot(equipment);
            equipmentManager.AddEquipmentInInventory(equipment.equipmentType, equipment.rank);
        }
        // foreach (EquipmentData equipment in hashSummonedItems)
        // {
        //     SummonManager.instance.UpdateEquipmentCount(equipment, summonedCounts[equipment]);
        // }

        // SummonManager.instance.AllSort();

        //QuestManager.instance.UpdateCount(QuestType.EquipmentSummonCount, quantity);
        // DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Forge_Equipment, quantity);

        resultUI.ShowSlots();
    }

    protected override void SummonItem(CurrencyType currencyType, int quantity, int price, int quantityNum)
    {
        summonedCounts.Clear();
        summonedItems.Clear();
        hashSummonedItems.Clear();
        resultUI.ClearSlots();
        resultUI.SetResultUI(this.type, SmallSummonByType, LargeSummonByType);
        resultUI.SetInfo(smallInfo, largeInfo);
        resultUI.SetCurrentQuantityType(quantityNum);
        //resultUI.SetButtonInfo(currencyType, small: smallInfo, large: largeInfo);

        SummonItem(quantity);
    }

    private EquipmentData GetRandomEquipmentData()
    {
        EquipmentType equipmentType = ResourceManager.instance.equipment.GetRandomEquipmentType();
        Rank rank = ResourceManager.instance.rank.GetRandomRank();
        EquipmentData equipmentData = new EquipmentData(equipmentType, rank);
        return equipmentData;
    }
}
