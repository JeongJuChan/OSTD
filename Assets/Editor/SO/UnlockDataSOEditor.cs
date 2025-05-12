using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnlockDataSO))]
public class UnlockDataSOEditor : ListDataSOEditor<UnlockDataSO>
{
    public static void LoadCSVToSO(UnlockDataSO unlockDataSO, TextAsset csv)
    {
        unlockDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<UnlockData> unlockDatas = new List<UnlockData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            FeatureID featureID = (FeatureID)Enum.Parse(typeof(FeatureID), elements[0]);
            FeatureType featureType = (FeatureType)Enum.Parse(typeof(FeatureType), elements[1]);
            int count = int.Parse(elements[2].Trim('\r'));

            UnlockData unlockData = new UnlockData(featureType, count, featureID);

            unlockDatas.Add(unlockData);
        }

        unlockDataSO.AddDatas(unlockDatas);

        unlockDataSO.InitDict();

        EditorUtility.SetDirty(unlockDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<UnlockData> unlockDatas = new List<UnlockData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            FeatureID featureID = (FeatureID)Enum.Parse(typeof(FeatureID), elements[0]);
            FeatureType featureType = (FeatureType)Enum.Parse(typeof(FeatureType), elements[1]);
            int count = int.Parse(elements[2].Trim());

            UnlockData unlockData = new UnlockData(featureType, count, featureID);

            unlockDatas.Add(unlockData);
        }

        dataSO.AddDatas(unlockDatas);

        dataSO.InitDict();

        EditorUtility.SetDirty(dataSO);
    }
}
