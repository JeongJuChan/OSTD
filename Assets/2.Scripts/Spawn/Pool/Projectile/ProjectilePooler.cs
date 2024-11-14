using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : IPooler<Projectile>
{
    private Dictionary<int, ObjectPool<Projectile>> poolDict = new Dictionary<int, ObjectPool<Projectile>>();

    private Transform parent;

    public ProjectilePooler(Transform parent)
    {
        this.parent = parent;
    }

    #region IPoolerInterfaceMethods
    public Projectile Pool(int prefabIndex, Vector3 position, Quaternion quaternion)
    {
        Projectile projectile = poolDict[prefabIndex].Pool(position, quaternion);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void AddPoolInfo(int index, int initCount, int maxCount)
    {
        Projectile projectile = ResourceManager.instance.projectile.GetProjectileData(index);

        if (!poolDict.ContainsKey(index))
        {
            poolDict.Add(index, new ObjectPool<Projectile>(FactoryMethod, projectile.gameObject, initCount, maxCount, parent));
        }
    }

    public void RemovePoolInfo(int index)
    {
        if (poolDict.ContainsKey(index))
        {
            poolDict[index].DestroyAll();
            poolDict.Remove(index);
        }
    }

    public Projectile FactoryMethod(GameObject go)
    {
        Projectile projectile = UnityEngine.Object.Instantiate(go).GetComponent<Projectile>();
        projectile.Init();
        projectile.name = go.name;
        GameManager.instance.OnReset += projectile.ReturnToPool;
        // projectile.OnResetData += ResetMonsterBaseData;
        // projectile.OnDeadCallback += OnDead;
        // projectile.OnAttacked += OnCastleAttacked;
        // ResetMonsterBaseData(projectile);
        return projectile;
    }

    public void UpdatePoolCount(int index, int maxCountMod)
    {
        poolDict[index].UpdateMaxCount(maxCountMod);
    }
    #endregion

    public void UpdatePoolInfo(int index, int initCount, int maxCount)
    {
        // Projectile monster = ResourceManager.instance.monster.GetResource(index);
        // poolDict[index].DestroyAll();
        // poolDict[index] = new ObjectPool<Projectile>(FactoryMethod, monster.gameObject, initCount, maxCount, parent);
    }
}
