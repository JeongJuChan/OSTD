using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EquipmentBaseDataSO))]
public class EquipmentBaseDataSOEditor : DataSOEditor<EquipmentBaseDataSO>
{
    public static void LoadCSVToSO(EquipmentBaseDataSO equipmentBaseDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        int rankLength = Enum.GetValues(typeof(Rank)).Length - 1;

        StatType statType = StatType.Health;

        int startIndex = rankLength * (int)equipmentBaseDataSO.equipmentType + 1;
        int endIndex = rankLength + rankLength * (int)equipmentBaseDataSO.equipmentType + 1;

        int equipmentCount = 0;
        int maxLevel = 0;

        EquipmentRankReference[] equipmentRankReferences = new EquipmentRankReference[rankLength];

        int indexCount = 0;

        for (int i = startIndex; i < endIndex; i++)
        {
            string[] elements = rows[i].Split(',');

            if (i == startIndex)
            {
                string[] tempElements = rows[1].Split(',');
                statType = EnumUtility.GetEqualValue<StatType>(tempElements[1]);
                equipmentCount = int.Parse(tempElements[3]);
                maxLevel = int.Parse(tempElements[4]);
            }

            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[2]);
            int equipStart = int.Parse(elements[5]);
            float equipIncrement = float.Parse(elements[6]);
            int equipLevelUpStart = int.Parse(elements[7]);
            float equipLevelUpIncrement = float.Parse(elements[8]);
            int ownStart = int.Parse(elements[9]);
            float ownIncrement = float.Parse(elements[10]);
            int ownLevelUpStart = int.Parse(elements[11]);
            float ownLevelUpIncrement = float.Parse(elements[12]);

            equipmentRankReferences[indexCount] = new EquipmentRankReference(rank, equipmentCount, maxLevel, equipStart, equipIncrement,
                equipLevelUpStart, equipLevelUpIncrement, ownStart, ownIncrement, ownLevelUpStart, ownLevelUpIncrement);

            indexCount++;
        }

        equipmentBaseDataSO.SetData(new EquipmentBaseData(statType, equipmentRankReferences));
        equipmentBaseDataSO.InitData();
        EditorUtility.SetDirty(equipmentBaseDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
    }
}
