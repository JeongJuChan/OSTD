using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/EffectData", fileName = "EffectData")]
public class EffectDataSO : ListDataSO<EffectData>
{
    private Dictionary<EffectType, List<EffectData>> effectDataDict = new Dictionary<EffectType, List<EffectData>>();
    private Dictionary<int, EffectType> effectTypeDict = new Dictionary<int, EffectType>();

    [field: SerializeField] public int defaultPoolCount { get; private set; } = 10;

    public List<EffectData> GetAllEffectData()
    {
        return datas;
    }

    public List<EffectData> GetEffectData(EffectType effectType)
    {
        if (effectDataDict.Count == 0)
        {
            InitDict();
        }

        if (effectDataDict.ContainsKey(effectType))
        {
            return effectDataDict[effectType];
        }

        return default;
    }

    public EffectType GetEffectType(int index)
    {
        if (effectTypeDict.Count == 0)
        {
            InitDict();
        }

        if (effectTypeDict.ContainsKey(index))
        {
            return effectTypeDict[index];
        }

        return default;
    }

    public override void InitDict()
    {
        effectDataDict.Clear();

        foreach (var data in datas)
        {
            if (!effectDataDict.ContainsKey(data.effectType))
            {
                effectDataDict.Add(data.effectType, new List<EffectData>());
            }

            effectDataDict[data.effectType].Add(data);

            if (!effectTypeDict.ContainsKey(data.index))
            {
                effectTypeDict.Add(data.index, data.effectType);
            }
        } 
    }
}
