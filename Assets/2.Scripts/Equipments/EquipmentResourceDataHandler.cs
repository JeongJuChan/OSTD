using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class EquipmentResourceDataHandler
{
    private Dictionary<EquipmentType, Dictionary<Rank, Dictionary<int, EquipmentStatData>>> equipmentStatDataDict = 
        new Dictionary<EquipmentType, Dictionary<Rank, Dictionary<int, EquipmentStatData>>>();

    private Dictionary<EquipmentType, Dictionary<Rank, Sprite>> equipmentSpriteDict = new Dictionary<EquipmentType, Dictionary<Rank, Sprite>>();

    private Dictionary<EquipmentType, string> equipmentDescriptionDict = new Dictionary<EquipmentType, string>();

    private Dictionary<int, (BigInteger, BigInteger)> levelUpEnforceCostDict = new Dictionary<int, (BigInteger, BigInteger)>();
    private Dictionary<int, (BigInteger, BigInteger)> levelUpEnforceBackpackCostDict = new Dictionary<int, (BigInteger, BigInteger)>();

    private Dictionary<Rank, int> sellingCurrencyDict = new Dictionary<Rank, int>();

    private EquipmentType[] equipmentTypes;
    private GameData equipmentData;
    private GameData equipmentLevelUpData;
    private GameData equipmentFixedData;

    private RankDataHandler rankDataHandler;

    private Dictionary<StatType, Sprite> statTypeSpriteDict = new Dictionary<StatType, Sprite>();

    private Sprite upperArrow;
    private Sprite lowerArrow;

    public void Init()
    {
        equipmentData = Resources.Load<GameData>($"{Consts.GAME_DATA}/EquipmentData");

        StatType[] statTypes = (StatType[])Enum.GetValues(typeof(StatType));
        for (int i = 1; i < statTypes.Length; i++)
        {
            if (!statTypeSpriteDict.ContainsKey(statTypes[i]))
            {
                statTypeSpriteDict.Add(statTypes[i], Resources.Load<Sprite>($"UI/Hero/{statTypes[i]}Sprite"));
            }
        }

        rankDataHandler = ResourceManager.instance.rank;

        List<SerializableRow> rows = equipmentData.GetDataRows();

        equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));

        for (int i = 0; i < rows.Count; i++)
        {
            List<string> elements = rows[i].rowData;

            int count = 0;


            for (int j = 0; j < elements.Count; j += 3)
            {
                EquipmentStatData tempData = new EquipmentStatData(equipmentTypes[count + 1], EnumUtility.GetEqualValue<Rank>(elements[j].Trim('\b')), 
                    int.Parse(elements[j + 1]), new BigInteger(elements[j + 2]));

                if (!equipmentStatDataDict.ContainsKey(tempData.equipmentType))
                {
                    equipmentStatDataDict.Add(tempData.equipmentType, new Dictionary<Rank, Dictionary<int, EquipmentStatData>>());
                }

                if (!equipmentStatDataDict[tempData.equipmentType].ContainsKey(tempData.rank))
                {
                    equipmentStatDataDict[tempData.equipmentType].Add(tempData.rank, new Dictionary<int, EquipmentStatData>());
                }

                if (!equipmentStatDataDict[tempData.equipmentType][tempData.rank].ContainsKey(tempData.level))
                {
                    equipmentStatDataDict[tempData.equipmentType][tempData.rank].Add(tempData.level, tempData);
                }

                count++;
            }
        }

        Rank[] ranks = rankDataHandler.GetRanks();

        for (int i = 1; i < equipmentTypes.Length; i++)
        {
            EquipmentType equipmentType = equipmentTypes[i];

            if (equipmentType == EquipmentType.Backpack)
            {
                Rank rank = Rank.Common;
                UpdateEquipmentSpriteDict(equipmentType, rank);
                continue;
            }

            for (int j = 1; j < ranks.Length; j++)
            {
                Rank rank = ranks[j];
                UpdateEquipmentSpriteDict(equipmentType, rank);
            }
        }

        equipmentFixedData = Resources.Load<GameData>("GameData/EquipmentFixedData");

        List<SerializableRow> equipmentFixedRows = equipmentFixedData.GetDataRows();

        for (int i = 1; i < equipmentTypes.Length; i++)
        {
            if (!equipmentDescriptionDict.ContainsKey(equipmentTypes[i]))
            {
                string description = equipmentFixedData.GetDataRows()[0].rowData[i - 1 + 2];
                equipmentDescriptionDict.Add(equipmentTypes[i], description);
            }    
        }


        for (int i = 0; i < equipmentFixedRows.Count; i++)
        {
            List<string> elements = equipmentFixedRows[i].rowData;
            Rank equipmentType = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int enforcePowder = int.Parse(elements[1]);
            if (!sellingCurrencyDict.ContainsKey(equipmentType))
            {
                sellingCurrencyDict.Add(equipmentType, enforcePowder);
            }
        }

        equipmentLevelUpData = Resources.Load<GameData>($"{Consts.GAME_DATA}/EquipmentLevelUpData");
        List<SerializableRow> equipmentLevelUpRows = equipmentLevelUpData.GetDataRows();
        for (int i = 0; i < equipmentLevelUpRows.Count; i++)
        {
            List<string> elements = equipmentLevelUpRows[i].rowData;

            int level = int.Parse(elements[0]);
            if (!levelUpEnforceCostDict.ContainsKey(level))
            {
                levelUpEnforceCostDict.Add(level, (new BigInteger(elements[1]), new BigInteger(elements[2])));
            }

            if (!levelUpEnforceBackpackCostDict.ContainsKey(level))
            {
                levelUpEnforceBackpackCostDict.Add(level, (new BigInteger(elements[3]), new BigInteger(elements[4])));
            }
        }

        upperArrow = Resources.Load<Sprite>("UI/Hero/UpperArrow");
        lowerArrow = Resources.Load<Sprite>("UI/Hero/LowerArrow");
    }

    private void UpdateEquipmentSpriteDict(EquipmentType equipmentType, Rank rank)
    {
        if (!equipmentSpriteDict.ContainsKey(equipmentType))
        {
            equipmentSpriteDict.Add(equipmentType, new Dictionary<Rank, Sprite>());
        }

        if (!equipmentSpriteDict[equipmentType].ContainsKey(rank))
        {
            equipmentSpriteDict[equipmentType].Add(rank, null);
        }

        Sprite sprite = Resources.Load<Sprite>($"Sprites/Equipments/{rank}{equipmentType}");
        equipmentSpriteDict[equipmentType][rank] = sprite;
    }

    // TODO : 가능한 Rank에 맞춰 필터링할 필요가 있음
    public Sprite GetRandomEquipmentSprite()
    {
        EquipmentType type = GetRandomEquipmentType();
        if (equipmentSpriteDict.ContainsKey(type))
        {
            Rank rank = rankDataHandler.GetRandomRank();
            if (equipmentSpriteDict[type].ContainsKey(rank))
            {
                return equipmentSpriteDict[type][rank];
            }
        }

        return default;
    }

    public EquipmentType GetRandomEquipmentType()
    {
        int index = UnityEngine.Random.Range(1, equipmentTypes.Length - 1);
        EquipmentType type = equipmentTypes[index];
        return type;
    }

    public Sprite GetEquipmentSprite(EquipmentType equipmentType, Rank rank)
    {
        if (equipmentSpriteDict.ContainsKey(equipmentType))
        {
            if (equipmentSpriteDict[equipmentType].ContainsKey(rank))
            {
                return equipmentSpriteDict[equipmentType][rank];
            }
        }

        return default;
    }

    public EquipmentStatData GetEquipmentStatData(EquipmentType equipmentType, Rank rank, int level)
    {
        if (equipmentStatDataDict.ContainsKey(equipmentType))
        {
            if (equipmentStatDataDict[equipmentType].ContainsKey(rank))
            {
                if (equipmentStatDataDict[equipmentType][rank].ContainsKey(level))
                {
                    return equipmentStatDataDict[equipmentType][rank][level];
                }
            }
        }

        return default;
    }

    public Sprite GetStatTypeSprite(StatType statType)
    {
        if (statTypeSpriteDict.ContainsKey(statType))
        {
            return statTypeSpriteDict[statType];
        }

        return default;
    }

    public string GetEquipmentDescription(EquipmentType equipmentType)
    {
        if (equipmentDescriptionDict.ContainsKey(equipmentType))
        {
            return equipmentDescriptionDict[equipmentType];
        }

        return default;
    }

    public (BigInteger, BigInteger) GetEquipmentLevelUpCost(int level)
    {
        if (levelUpEnforceCostDict.ContainsKey(level))
        {
            return levelUpEnforceCostDict[level];
        }

        return default;
    }

    public (BigInteger, BigInteger) GetBackpackLevelUpCost(int level)
    {
        if (levelUpEnforceBackpackCostDict.ContainsKey(level))
        {
            return levelUpEnforceBackpackCostDict[level];
        }

        return default;
    }

    public Sprite GetUpperArrow()
    {
        return upperArrow;
    }

    public Sprite GetLowerArrow()
    {
        return lowerArrow;
    }

    public int GetSellingCurrency(Rank rank)
    {
        if (sellingCurrencyDict.ContainsKey(rank))
        {
            return sellingCurrencyDict[rank];
        }

        return 0;
    }

}
