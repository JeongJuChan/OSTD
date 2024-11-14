using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SummonProbabilityDataSO))]
public class SummonProbabilityDataSOEditor : ListDataSOEditor<SummonProbabilityDataSO>
{
    public static void LoadCSVToSO(SummonProbabilityDataSO summonProbabilityDataSO, TextAsset csv)
    {
        summonProbabilityDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<Proportion> proportions = new List<Proportion>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);

            int[] rankProbabilities = new int[Enum.GetValues(typeof(Rank)).Length - 1];

            for (int j = 0; j < rankProbabilities.Length && j < elements.Length - 1; j++)
            {
                if (j == rankProbabilities.Length - 1)
                {
                    rankProbabilities[j] = (int)(decimal.Parse(elements[j + 1].Trim('\r')) * Consts.PERCENT_UNIT_EQUIPMENT_VALUE);
                }
                else
                {
                    rankProbabilities[j] = (int)(decimal.Parse(elements[j + 1]) * Consts.PERCENT_UNIT_EQUIPMENT_VALUE);
                }
            }

            proportions.Add(new Proportion(i, rankProbabilities));
        }

        summonProbabilityDataSO.AddDatas(proportions);
        summonProbabilityDataSO.InitDict();
        EditorUtility.SetDirty(summonProbabilityDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<Proportion> proportions = new List<Proportion>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);

            int[] rankProbabilities = new int[Enum.GetValues(typeof(Rank)).Length - 1];

            for (int j = 0; j < rankProbabilities.Length; j++)
            {
                if (j == rankProbabilities.Length - 1)
                {
                    rankProbabilities[j] = (int)(float.Parse(elements[j + 1].Trim('\r')) * Consts.PERCENT_UNIT_EQUIPMENT_VALUE);
                }
                else
                { 
                    rankProbabilities[j] = (int)(float.Parse(elements[j + 1]) * Consts.PERCENT_UNIT_EQUIPMENT_VALUE);
                }
            }

            proportions.Add(new Proportion(level, rankProbabilities));
        }

        dataSO.AddDatas(proportions);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
