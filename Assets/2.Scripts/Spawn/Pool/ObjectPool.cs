using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour, IPoolable<T>
{
    private GameObject prefab;
    private Queue<T> pool;
    private int offsetMaxCount;
    private int maxCount;
    private int initCount;
    private int activeCount;
    private Transform parent;
    private Func<GameObject, T> factoryFunc;

    public ObjectPool(Func<GameObject, T> factoryFunc, GameObject prefab, int initCount, int maxCount, Transform parent)
    {
        this.factoryFunc = factoryFunc;
        this.prefab = prefab;
        pool = new Queue<T>();
        this.initCount = initCount;
        offsetMaxCount = maxCount;
        this.maxCount = maxCount;
        this.parent = parent;
        Init(initCount);
    }

    #region PoolingMethods
    public void DestroyAll()
    {
        while (pool.Count > 0)
        {
            UnityEngine.Object.Destroy(pool.Dequeue()); 
        }
    }

    public void UpdateMaxCount(int maxCountMod)
    {
        maxCount = offsetMaxCount * maxCountMod;
    }

    public T Pool()
    {
        if (TryPool(out T t))
            return t;
        else
            return null;
    }

    public T Pool(Vector2 position, Quaternion quaternion)
    {
        if (TryPool(out T t))
        {
            t.transform.SetPositionAndRotation(position, quaternion);
            return t;
        }
        else
        {
            return null;
        }
    }


    public T Pool(Vector2 position, Quaternion quaternion, Transform parent)
    {
        if (TryPool(out T t))
        {
            t.transform.SetParent(parent);
            t.transform.SetPositionAndRotation(position, quaternion);
            return t;
        }
        else
        {
            return null;
        }
    }

    private bool TryPool(out T newT)
    {
        if (pool.Count <= initCount / 2)
        {
            for (int i = 0; activeCount + pool.Count < maxCount; i++)
            {
                pool.Enqueue(SpawnNewT());
            }
        }

        if (activeCount >= maxCount)
        {
            newT = null;
            return false;
        }

        T t = pool.Dequeue();
        newT = t;
        activeCount++;
        return newT != null;
    }

    private T SpawnNewT()
    {
        T t;
        t = factoryFunc?.Invoke(prefab);
        t.name = prefab.name;
        t.gameObject.SetActive(false);
        t.transform.SetParent(parent);
        t.Initialize(Push);
        return t;
    }

    protected virtual void Push(T t)
    {
        if (t != null)
        {
            t.gameObject.SetActive(false);
            pool.Enqueue(t);
            activeCount--;
        }
    }

    private void Init(int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            pool.Enqueue(SpawnNewT());
        }
    }
    #endregion
}
