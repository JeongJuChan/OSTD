using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectPooler : IPooler<Weapon>
{
    private Dictionary<int, ObjectPool<Weapon>> poolDict = new Dictionary<int, ObjectPool<Weapon>>();

    private Transform parent;

    public WeaponObjectPooler(Transform parent)
    {
        this.parent = parent;
    }

    #region IPoolerInterfaceMethods
    public Weapon Pool(int prefabIndex, Vector3 position, Quaternion quaternion)
    {
        Weapon projectile = poolDict[prefabIndex].Pool(position, quaternion);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void AddPoolInfo(int index, int initCount, int maxCount)
    {
        Weapon projectile = ResourceManager.instance.weapon.GetWeapon((WeaponType)index);

        if (!poolDict.ContainsKey(index))
        {
            poolDict.Add(index, new ObjectPool<Weapon>(FactoryMethod, projectile.gameObject, initCount, maxCount, parent));
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

    public Weapon FactoryMethod(GameObject go)
    {
        Weapon weapon = UnityEngine.Object.Instantiate(go).GetComponent<Weapon>();
        weapon.Init();
        weapon.name = go.name;
        // projectile.OnResetData += ResetMonsterBaseData;
        // projectile.OnDeadCallback += OnDead;
        // projectile.OnAttacked += OnCastleAttacked;
        // ResetMonsterBaseData(projectile);
        return weapon;
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