using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct Proportion
{
    public int level;
    public int[] proportionArray;

    public Proportion(int level, int[] proportionArray)
    {
        this.level = level;
        this.proportionArray = proportionArray;
    }
}

[CreateAssetMenu(menuName = "SO/SummonProbabilityData")]
public class SummonProbabilityDataSO : ListDataSO<Proportion>
{
    private Dictionary<int, Proportion> proportionDict = new Dictionary<int, Proportion>();

    public int[] GetProbabillitiesOfLevel(int level)
    {
        if (proportionDict.Count == 0)
        {
            InitDict();
        }

        return proportionDict[level].proportionArray;
    }

    public int GetMaxLevel()
    {
        return proportionDict.Count;
    }

    public override void InitDict()
    {
        foreach (Proportion data in datas)
        {
            if (!proportionDict.ContainsKey(data.level))
            {
                proportionDict.Add(data.level, data);
            }
        }
    }
}