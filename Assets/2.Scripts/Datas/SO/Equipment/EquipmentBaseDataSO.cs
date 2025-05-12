using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EquipmentRankReference
{
    public Rank rank;
    public int equipmentCount;
    public int maxLevel;

    public int baseEquipEffectStartValue;
    public float baseEquipEffectIncreaseRate;
    public int equipEffectLevelUpUnitStartValue;
    public float equipEffectLevelUpUnitIncreaseRate;

    public int baseOwnerEffectStartValue;
    public float baseOwnerEffectIncreaseRate;
    public int ownerEffectLevelUpUnitStartValue;
    public float ownerEffectLevelUpUnitIncreaseRate;

    public EquipmentRankReference(Rank rank, int equipmentCount, int maxLevel, int baseEquipEffectStartValue, 
        float baseEquipEffectIncreaseRate, int equipEffectLevelUpUnitStartValue, float equipEffectLevelUpUnitIncreaseRate,
        int baseOwnerEffectStartValue, float baseOwnerEffectIncreaseRate, int ownerEffectLevelUpUnitStartValue,
        float ownerEffectLevelUpUnitIncreaseRate)
    {
        this.rank = rank;
        this.equipmentCount = equipmentCount;
        this.maxLevel = maxLevel;
        this.baseEquipEffectStartValue = baseEquipEffectStartValue;
        this.baseEquipEffectIncreaseRate = baseEquipEffectIncreaseRate;
        this.equipEffectLevelUpUnitStartValue = equipEffectLevelUpUnitStartValue;
        this.equipEffectLevelUpUnitIncreaseRate = equipEffectLevelUpUnitIncreaseRate;
        this.baseOwnerEffectStartValue = baseOwnerEffectStartValue;
        this.baseOwnerEffectIncreaseRate = baseOwnerEffectIncreaseRate;
        this.ownerEffectLevelUpUnitStartValue = ownerEffectLevelUpUnitStartValue;
        this.ownerEffectLevelUpUnitIncreaseRate = ownerEffectLevelUpUnitIncreaseRate;
    }
}

[Serializable]
public struct EquipmentBaseData
{
    public StatType effectType;
    public EquipmentRankReference[] rankReferences;

    public EquipmentBaseData(StatType effectType, EquipmentRankReference[] rankReferences)
    {
        this.effectType = effectType;
        this.rankReferences = rankReferences;
    }
}

[CreateAssetMenu(menuName = "SO/EquipmentBaseData")]
public class EquipmentBaseDataSO : DataSO<EquipmentBaseData>
{
    [field: SerializeField] public EquipmentType equipmentType;

    public EquipmentType EquipmentType => equipmentType;
    public StatType EffectType => data.effectType;
    public EquipmentRankReference[] RankReferences => data.rankReferences;
}
