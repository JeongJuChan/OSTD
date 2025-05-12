using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterRewardDataSO))]
public class MonsterRewardDataSOEditor : DataSOEditor<MonsterRewardDataSO>
{
    public static void LoadCSVToSO(MonsterRewardDataSO monsterRewardDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        monsterRewardDataSO.SetData(new MonsterRewardData(int.Parse(elements[10]), int.Parse(elements[12]), 
            int.Parse(elements[13].Trim('%')), int.Parse(elements[15].Trim('\r'))));
        monsterRewardDataSO.InitData();
        EditorUtility.SetDirty(monsterRewardDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        dataSO.SetData(new MonsterRewardData(int.Parse(elements[10]), int.Parse(elements[12]),
            int.Parse(elements[13].Trim('%')), int.Parse(elements[15].Trim('\r'))));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
