using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObjectPooler : IPooler<Monster>
{
    private Dictionary<int, ObjectPool<Monster>> poolDict = new Dictionary<int, ObjectPool<Monster>>();
    private HashSet<Monster> activeMonsterSet = new HashSet<Monster>();

    private Transform parent;

    private Action<Vector2> OnDead;

    public MonsterObjectPooler(Transform parent, Action<Vector2> OnDead)
    {
        this.parent = parent;
        this.OnDead = OnDead;
        UIManager.instance.GetUIElement<UI_LosePanel>().AddButtonAction(() => PushAllActiveMonsters(false));
    }

    #region IPoolerInterfaceMethods
    public Monster Pool(int prefabIndex, Vector3 position, Quaternion quaternion)
    {
        Monster monster = poolDict[prefabIndex].Pool(position, quaternion);
        activeMonsterSet.Add(monster);
        monster.gameObject.SetActive(true);
        return monster;
    }

    public void AddPoolInfo(int index, int initCount, int maxCount)
    {
        Monster monster = ResourceManager.instance.monster.GetResource(index);

        if (!poolDict.ContainsKey(index))
        {
            poolDict.Add(index, new ObjectPool<Monster>(FactoryMethod, monster.gameObject, initCount, maxCount, parent));
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

    public Monster FactoryMethod(GameObject go)
    {
        Monster monster = UnityEngine.Object.Instantiate(go).GetComponent<Monster>();
        monster.name = go.name;
        monster.OnDeadCallback += OnDead;
        monster.OnRemoveTargetAction += BattleManager.instance.TryRemoveMonsterInRangeTickWeaponTarget;
        monster.OnTryResetTarget += HeroManager.instance.hero.ResetTarget;
        monster.OnTryResetTarget += WeaponManager.instance.ResetTarget;
        HeroManager.instance.hero.OnDead += () => monster.UpdateIsHeroDead(true);
        HeroManager.instance.hero.OnReset += () => monster.UpdateIsHeroDead(false);
        ResetMonsterBaseData(monster);
        monster.Init();
        if (monster.TryGetComponent(out ClimbingController climbingController))
        {
            climbingController.Init();
        }
        return monster;
    }

    public void PushAllActiveMonsters(bool isCausedByBattle)
    {
        foreach (Monster monster in activeMonsterSet)
        {
            monster.Die(isCausedByBattle);
        }
    }

    public void UpdatePoolCount(int index, int maxCountMod)
    {
        poolDict[index].UpdateMaxCount(maxCountMod);
    }
    #endregion

    private void ResetMonsterBaseData(Monster monsterBase)
    {
        // MonsterData monsterBaseData = OnGetMonsterDataFunc.Invoke(monster.index);
        // monster.SetMonsterBaseData(monsterBaseData);
    }

    public void UpdatePoolInfo(int index, int initCount, int maxCount)
    {
        Monster monster = ResourceManager.instance.monster.GetResource(index);
        poolDict[index].DestroyAll();
        poolDict[index] = new ObjectPool<Monster>(FactoryMethod, monster.gameObject, initCount, maxCount, parent);
    }

}
