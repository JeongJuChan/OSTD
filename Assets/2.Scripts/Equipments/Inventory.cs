using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class Inventory
{
    private List<EquipmentStatData> equipmentStatDatas = new List<EquipmentStatData>();

    public event Action<List<EquipmentStatData>> OnUpdateInventoryUI;

    private EquipmentType[] equipmentTypes;

    private EquipmentManager equipmentManager;

    private Queue<EquipmentStatData> removingDatas = new Queue<EquipmentStatData>();
    private Queue<EquipmentType> addingTypes = new Queue<EquipmentType>();

    private EquipmentResourceDataHandler equipmentResourceDataHandler;

    public void Init()
    {
        equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));
        equipmentManager = EquipmentManager.instance;
        equipmentResourceDataHandler = ResourceManager.instance.equipment;
        equipmentStatDatas = DataBaseManager.instance.Load(Consts.EQUIPMENTS_IN_INVENTORY, equipmentStatDatas);
        
        for (int i = 0; i < equipmentStatDatas.Count; i++)
        {
            EquipmentStatData equipmentStatData = equipmentStatDatas[i];
            equipmentStatDatas[i] = equipmentResourceDataHandler.GetEquipmentStatData(equipmentStatData.equipmentType, equipmentStatData.rank, 
                equipmentStatData.level);
        }

        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        SortEquipments();
        OnUpdateInventoryUI?.Invoke(equipmentStatDatas);
    }

    private void SortEquipments()
    {
        equipmentStatDatas.Sort((x, y) =>
        {
            int first = x.equipmentType.CompareTo(y.equipmentType);
            return first != 0 ? first : -x.rank.CompareTo(y.rank);
        });
    }

    public void AddEquipment(EquipmentStatData equipmentStatData)
    {
        equipmentStatDatas.Add(equipmentStatData);
        SaveEquipmentDatas();
    }

    public void RemoveEquipment(EquipmentStatData equipmentStatData)
    {
        equipmentStatDatas.Remove(equipmentStatData);
        SaveEquipmentDatas();
    }

    public void UpdateCurrentEquipmentStatData(int index)
    {
        EquipmentStatData newEquipmentStatData = equipmentStatDatas[index];
        equipmentManager.CompareEquipments(newEquipmentStatData);
    }

    public void EquipBest()
    {
        SortEquipments();
        EquipmentType equipmentType = default;
        int length = equipmentTypes.Length - 1;
        int count = 0;


        foreach (EquipmentStatData equipmentStatData in equipmentStatDatas)
        {
            if (equipmentStatData.equipmentType != equipmentType)
            {
                equipmentType = equipmentStatData.equipmentType;
                if (equipmentManager.GetIsNewEquipmentUpperRank(equipmentType, equipmentStatData.rank))
                {
                    addingTypes.Enqueue(equipmentType);
                    removingDatas.Enqueue(equipmentStatData);
                }

                count++;

                if (count == length)
                {
                    break;
                }
            }
        }

        while (addingTypes.Count > 0)
        {
            equipmentManager.AddCurrentEquipmentInInventory(addingTypes.Dequeue());
        }

        while (removingDatas.Count > 0)
        {
            EquipmentStatData equipmentData = removingDatas.Dequeue();
            equipmentManager.UpdateEquipmentStat(equipmentData.equipmentType, equipmentData);
            RemoveEquipment(equipmentData);
        }

        UpdateInventoryUI();
    }

    public BigInteger GetCurrencyBySellingDuplicate(EquipmentType equipmentType, Rank rank)
    {
        BigInteger totalCurrency = 0;

        List<EquipmentStatData> tempDatas = new List<EquipmentStatData>(equipmentStatDatas.Count);
        tempDatas.AddRange(equipmentStatDatas);

        foreach (var data in tempDatas)
        {
            if (data.equipmentType == equipmentType)
            {
                if (data.rank <= rank)
                {
                    int amount = equipmentResourceDataHandler.GetSellingCurrency(rank);
                    int result = UnityEngine.Random.Range(amount - 5, amount + 5);
                    totalCurrency += result;
                    RemoveEquipment(data);
                }
            }
        }

        return totalCurrency;
    }

    public bool GetCanSellingDuplicate()
    {
        List<EquipmentStatData> tempDatas = new List<EquipmentStatData>(equipmentStatDatas.Count);
        tempDatas.AddRange(equipmentStatDatas);

        foreach (var data in tempDatas)
        {
            if (equipmentManager.GetIsEquipmentUnderRank(data.equipmentType, data.rank))
            {
                return true;
            }
        }

        return false;
    }

    public void SaveEquipmentDatas()
    {
        DataBaseManager.instance.Save(Consts.EQUIPMENTS_IN_INVENTORY, equipmentStatDatas);
    }

    public bool GetCanAutoEquippable()
    {
        List<EquipmentStatData> tempDatas = new List<EquipmentStatData>(equipmentStatDatas.Count);
        tempDatas.AddRange(equipmentStatDatas);

        foreach (var data in tempDatas)
        {
            if (equipmentManager.GetIsNewEquipmentUpperRank(data.equipmentType, data.rank))
            {
                return true;
            }
        }

        return false;
    }

    public EquipmentStatData GetEquipmentStatDataByIndex(int index)
    {
        if (equipmentStatDatas.Count > index)
        {
            return equipmentStatDatas[index];
        }

        return default;
    }
}
