using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooler<T> where T : MonoBehaviour, IPoolable<T>
{
    void UpdatePoolCount(int index, int maxCountMod);
    void AddPoolInfo(int index, int initCount, int maxCount);
    void RemovePoolInfo(int index);
    void UpdatePoolInfo(int index, int poolCount, int maxCount);
    T Pool(int prefabIndex, Vector3 position, Quaternion quaternion);
    T FactoryMethod(GameObject prefab);
}
