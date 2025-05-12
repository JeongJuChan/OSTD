using System;
using UnityEngine;

public class EquipmentData : ISummonable
{
    private class EquipmentDataSave
    {
        public int count;
        public bool isEquipped;

        public EquipmentDataSave(int count, bool isEquipped)
        {
            this.count = count;
            this.isEquipped = isEquipped;
        }
    }

    public EquipmentType equipmentType;
    public Rank rank;

    public EquipmentData(EquipmentType equipmentType, Rank rank)
    {
        this.equipmentType = equipmentType;
        this.rank = rank;
    }
}
