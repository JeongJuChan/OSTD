using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class EffectSpawner : MonoBehaviour
{
    private IPooler<Effect> effectPooler;

    [SerializeField] private EffectDataSO effectDataSO;

    [SerializeField] private int initCountMod = 2;
    [SerializeField] private int maxCountMod = 10;

    private List<EffectData> effectDatas;

    private Action<int, int> OnUpdatePoolCount;

    #region UnityMethods

    #endregion

    #region EffectPoolMethods
    public void Init()
    {
        effectPooler = new EffectObjectPooler(transform);

        int poolCount = effectDataSO.defaultPoolCount;

        OnUpdatePoolCount += effectPooler.UpdatePoolCount;

        foreach (var effectData in effectDataSO.GetAllEffectData())
        {
            effectPooler.AddPoolInfo(effectData.index, poolCount, maxCountMod);
        }

        //skillManager.
    }

    public void UpdatePoolCount(int heroCount)
    {
        // foreach (var effectData in effectDataSO.GetEffectData(EffectType.LaserFloorEffect))
        // {
        //     OnUpdatePoolCount.Invoke(effectData.index, heroCount);
        // }
    }

    #endregion

    public void Spawn(EffectType effectType, Vector2 position)
    {
        effectDatas = effectDataSO.GetEffectData(effectType);
        int index = UnityEngine.Random.Range(0, effectDatas.Count);
        Effect effect = effectPooler.Pool(effectDatas[index].index, position, Quaternion.identity);
        effect.ReturnAfterAnimation();
    }
}