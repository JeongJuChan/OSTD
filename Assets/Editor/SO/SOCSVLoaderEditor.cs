using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(SOCSVLoader))]
public class SOCSVLoaderEditor : Editor
{
    private SOCSVLoader soCSVLoader;
    private const string defaultCSVPath = "CSV/";
    private const string defaultSOPath = "ScriptableObjects/";

    private void OnEnable()
    {
        soCSVLoader = (SOCSVLoader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Load All CSV to SO"))
        {
            LoadAll();
        }
    }

    private void LoadAll()
    {
        EffectDataSOEditor.LoadCSVToSO(Resources.Load<EffectDataSO>($"{defaultSOPath}Effects/EffectData"), 
            Resources.Load<TextAsset>($"{defaultCSVPath}Effects/Effect CSV"));
        MonsterRewardDataSOEditor.LoadCSVToSO(Resources.Load<MonsterRewardDataSO>($"{defaultSOPath}Reward/MonsterRewardData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        MonsterRewardIncrementDataSOEditor.LoadCSVToSO(Resources.Load<MonsterRewardIncrementDataSO>($"{defaultSOPath}Reward/MonsterRewardIncrementData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Monster/Stage/MonsterStats CSV"));
        // SkillDataSOEditor.LoadCSVToSO(Resources.Load<SkillDataSO>($"{defaultSOPath}Skills/SkillData"), 
        //     Resources.Load<TextAsset>($"{defaultCSVPath}Skills/Skills CSV"));
        UnlockDataSOEditor.LoadCSVToSO(Resources.Load<UnlockDataSO>($"{defaultSOPath}UnlockDataSO/UnlockData"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Unlock/UnlockData"));
        SummonProbabilityDataSOEditor.LoadCSVToSO(Resources.Load<SummonProbabilityDataSO>($"{defaultSOPath}SummonProbabilityDataSO/EquipmentSummonProbabilityDataSO"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Summon/EquipmentProbability CSV"));
        SummonProbabilityDataSOEditor.LoadCSVToSO(Resources.Load<SummonProbabilityDataSO>($"{defaultSOPath}SummonProbabilityDataSO/SkillSummonProbabilityDataSO"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Summon/SkillProbability CSV"));
        SummonProbabilityDataSOEditor.LoadCSVToSO(Resources.Load<SummonProbabilityDataSO>($"{defaultSOPath}SummonProbabilityDataSO/ColleagueSummonProbabilityDataSO"),
            Resources.Load<TextAsset>($"{defaultCSVPath}Summon/ColleagueProbability CSV"));
        EnumToKRSOEditor.LoadCSVToSO(Resources.Load<EnumToKRSO>($"{defaultSOPath}ToKR/EnumToKR"),
            Resources.Load<TextAsset>($"{defaultCSVPath}ToKR/EnumToKR CSV"));
    }
}
