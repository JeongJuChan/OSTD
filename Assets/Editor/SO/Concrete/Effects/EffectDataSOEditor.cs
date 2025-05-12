using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectDataSO))]
public class EffectDataSOEditor : ListDataSOEditor<EffectDataSO>
{
    public static void LoadCSVToSO(EffectDataSO effectDataSO, TextAsset csv)
    {
        effectDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<EffectData> effectDatas = new List<EffectData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');
            int index = int.Parse(element[0]);
            string effectName = element[1];
            EffectType effectType = EnumUtility.GetEqualValue<EffectType>(element[2].Trim('\r'));

            effectDatas.Add(new EffectData(index, effectName, effectType));
        }

        effectDataSO.AddDatas(effectDatas);
        effectDataSO.InitDict();
        EditorUtility.SetDirty(effectDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<EffectData> effectDatas = new List<EffectData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');
            int index = int.Parse(element[0]);
            string effectName = element[1];
            EffectType effectType = EnumUtility.GetEqualValue<EffectType>(element[2].Trim('\r'));

            effectDatas.Add(new EffectData(index, effectName, effectType));
        }

        dataSO.AddDatas(effectDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
