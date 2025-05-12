using System.Collections.Generic;
using System;
using UnityEngine;

public class EffectDataHandler
{
    private Dictionary<int, GameObject> effectResourceDict = new Dictionary<int, GameObject>();
    private const string Hit_EFFECT_PATH = "Effects/";
    
    private Func<List<EffectData>> OnGetAllEffectData;

    // TODO : 테이블화 하여 인덱싱으로 바꿈
    private Dictionary<EffectMaterialType, Material> effectMaterialDict = new Dictionary<EffectMaterialType, Material>();

    private Dictionary<EffectType, Effect> effectDict = new Dictionary<EffectType, Effect>();

    public void Init()
    {
        effectMaterialDict.Add(EffectMaterialType.DamageFlash, Resources.Load<Material>($"{Hit_EFFECT_PATH}Materials/DamageFlash_Mat"));
        effectResourceDict.Add((int)EffectType.LaserFloorEffect, Resources.Load<GameObject>($"{Hit_EFFECT_PATH}Laser_Floor_Effect"));
    }

    public Material GetMaterial(EffectMaterialType effectMaterialType)
    {
        if (effectMaterialDict.ContainsKey(effectMaterialType))
        {
            return effectMaterialDict[effectMaterialType];
        }

        return default;
    }

    // public GameObject GetEffect(EffectType effectType)
    // {
    //     if (effectResourceDict.ContainsKey((int)effectType))
    //     {
    //         return effectResourceDict[(int)effectType];
    //     }

    //     return default;
    // }

    public GameObject GetResource(int index)
    {
        if (effectResourceDict.ContainsKey(index))
        {
            return effectResourceDict[index];
        }

        return null;
    }

    private void LoadEffect()
    {
        List<EffectData> effectDatas = OnGetAllEffectData.Invoke();

        foreach (EffectData effectData in effectDatas)
        {
            Effect effect = Resources.Load<Effect>($"{Hit_EFFECT_PATH}{effectData.name}");
            effect.SetIndex(effectData.index);
            if (!effectResourceDict.ContainsKey(effectData.index))
            {
                effectResourceDict.Add(effectData.index, effect.gameObject);
            }
        }
    }
}