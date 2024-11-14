using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class EnergyDataHandler
{
    private Dictionary<int, EnergyData> energyDataDict = new Dictionary<int, EnergyData>();
    private Dictionary<SkillType, int> skillEnergyDict = new Dictionary<SkillType, int>();

    private GameData energyData;
    private GameData skillEnergyData;

    #region Initialize
    public void Init()
    {
        energyData = Resources.Load<GameData>("GameData/EnergyData");
        skillEnergyData = Resources.Load<GameData>("GameData/SkillEnergyData");

        List<SerializableRow> energyRows = energyData.GetDataRows();
        List<SerializableRow> skillEnergyRows = skillEnergyData.GetDataRows();

        energyDataDict.Clear();

        for (int i = 0; i < energyRows.Count; i++)
        {
            List<string> elements = energyRows[i].rowData;
            EnergyData energyData = new EnergyData(int.Parse(elements[0]), float.Parse(elements[1]), new BigInteger(elements[2]));

            if (!energyDataDict.ContainsKey(energyData.level))
            {
                energyDataDict.Add(energyData.level, energyData);
            }
        }

        SkillType[] skillTypes = (SkillType[])Enum.GetValues(typeof(SkillType));
        List<string> skillEnergyElements = skillEnergyRows[0].rowData;
        for (int i = 1; i < skillTypes.Length; i++)
        {
            skillEnergyDict.Add(skillTypes[i], int.Parse(skillEnergyElements[i - 1]));
        }
    }
    #endregion

    public EnergyData GetEnergyData(int level)
    {
        if (energyDataDict.ContainsKey(level))
        {
            return energyDataDict[level];
        }

        return default;
    }

    public int GetSkillEnergyData(SkillType skillType)
    {
        if (skillEnergyDict.ContainsKey(skillType))
        {
            return skillEnergyDict[skillType];
        }

        return default;
    }
}
