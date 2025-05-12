using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObjectPooler : IPooler<Effect>
{
    private Dictionary<int, ObjectPool<Effect>> poolDict = new Dictionary<int, ObjectPool<Effect>>();

    private Transform parent;

    public int initCount { get; private set; }
    public int maxCount { get; private set; }

    public EffectObjectPooler(Transform parent)
    {
        this.parent = parent;
    }

    #region IPoolerInterfaceMethods
    public Effect Pool(int prefabIndex, Vector3 position, Quaternion quaternion)
    {
        Effect effect = poolDict[prefabIndex].Pool(position, quaternion);
        return effect;
    }

    public void AddPoolInfo(int index, int poolCount, int maxCount)
    {
        if (!poolDict.ContainsKey(index))
        {
            GameObject go = ResourceManager.instance.effect.GetResource(index);
            poolDict.Add(index, new ObjectPool<Effect>(FactoryMethod, go, poolCount, maxCount, parent));
        }
    }

    public void UpdatePoolCount(int index, int maxCountMod)
    {
        poolDict[index].UpdateMaxCount(maxCountMod);
    }


    public void RemovePoolInfo(int index)
    {
        if (poolDict.ContainsKey(index))
        {
            poolDict.Remove(index);
        }
    }

    public Effect FactoryMethod(GameObject go)
    {
        Effect effect = UnityEngine.Object.Instantiate(go).GetComponent<Effect>();
        effect.name = go.name;
        return effect;
    }

    public void UpdatePoolInfo(int index, int poolCount, int maxCount)
    {
        
    }

    #endregion
}
