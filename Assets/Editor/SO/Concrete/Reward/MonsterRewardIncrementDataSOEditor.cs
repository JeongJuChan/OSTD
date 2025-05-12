using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterRewardIncrementDataSO))]
public class MonsterRewardIncrementDataSOEditor : DataSOEditor<MonsterRewardIncrementDataSO>
{
    public static void LoadCSVToSO(MonsterRewardIncrementDataSO monsterRewardIncrementDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[2].Split(',');

        int goldIncrement = int.Parse(elements[10]);
        int goldIncrementPercent = int.Parse(elements[11]);
        int forgeTicketIncrement = int.Parse(elements[13]);
        int forgeTicketIncrementPercent = int.Parse(elements[14].Trim('\r'));

        monsterRewardIncrementDataSO.SetData(new MonsterRewardIncrementData(goldIncrement, goldIncrementPercent, forgeTicketIncrement,
            forgeTicketIncrementPercent));

        EditorUtility.SetDirty(monsterRewardIncrementDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[2].Split(',');

        int goldIncrement = int.Parse(elements[10]);
        int goldIncrementPercent = int.Parse(elements[11]);
        int forgeTicketIncrement = int.Parse(elements[13]);
        int forgeTicketIncrementPercent = int.Parse(elements[14].Trim('\r'));

        dataSO.SetData(new MonsterRewardIncrementData(goldIncrement, goldIncrementPercent, forgeTicketIncrement, 
            forgeTicketIncrementPercent));


        EditorUtility.SetDirty(dataSO);
    }
}
