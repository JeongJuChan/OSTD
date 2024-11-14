using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct EquipmentStatData
{
    public EquipmentType equipmentType;
    public Rank rank;
    public int level;
    public BigInteger amount;

    public EquipmentStatData(EquipmentType equipmentType, Rank rank, int level, BigInteger amount)
    {
        this.equipmentType = equipmentType;
        this.rank = rank;
        this.level = level;
        this.amount = amount;
    }
}
