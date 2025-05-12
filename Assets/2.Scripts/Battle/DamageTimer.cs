using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class DamageTimer
{
    private Dictionary<BigInteger, float> damageTextTimeDict = new Dictionary<BigInteger, float>();
    private Dictionary<ITickAttack, Dictionary<IDamageable, float>> rangeDamageTickTimeDict = new Dictionary<ITickAttack, Dictionary<IDamageable, float>>();

    private float damageTextInterval = 0.5f;
    // 30 프레임 기준
    private float tickDamageUnit = 0.05f;

    public void TryRemoveMonsterInRangeTickWeaponTarget(IDamageable damagable)
    {
        HashSet<ITickAttack> tickAttacks = new HashSet<ITickAttack>();
        foreach (var timeDict in rangeDamageTickTimeDict)
        {
            if (timeDict.Value.ContainsKey(damagable))
            {
                tickAttacks.Add(timeDict.Key);
            }
        }

        foreach (ITickAttack tickAttack in tickAttacks)
        {
            rangeDamageTickTimeDict[tickAttack].Remove(damagable);
        }
    }

    public bool GetTickDamagePossible(ITickAttack tickAttack, IDamageable monster)
    {
        TryAddRangeTickTimeDict(tickAttack, monster);

        float elapsedTime = Time.time - rangeDamageTickTimeDict[tickAttack][monster];
        if (elapsedTime >= tickDamageUnit)
        {
            rangeDamageTickTimeDict[tickAttack][monster] = Time.time;
            return true;
        }

        return false;
    }

    private void TryAddRangeTickTimeDict(ITickAttack tickAttack, IDamageable monster)
    {
        if (!rangeDamageTickTimeDict.ContainsKey(tickAttack))
        {
            rangeDamageTickTimeDict.Add(tickAttack, new Dictionary<IDamageable, float>());
        }

        if (!rangeDamageTickTimeDict[tickAttack].ContainsKey(monster))
        {
            rangeDamageTickTimeDict[tickAttack].Add(monster, 0f);
        }
    }

    private void TryAddDamageTextTimeDict(BigInteger damage)
    {
        if (!damageTextTimeDict.ContainsKey(damage))
        {
            damageTextTimeDict.Add(damage, 0f);
        }
    }

    public bool GetTextPossible(BigInteger damage)
    {
        TryAddDamageTextTimeDict(damage);
        float elapsedTime = Time.time - damageTextTimeDict[damage];
        if (elapsedTime >= damageTextInterval)
        {
            damageTextTimeDict[damage] = Time.time;
            return true;
        }

        return false;
    }

    public void Init()
    {
        GameManager.instance.OnReset += Reset;
    }

    private void Reset()
    {
        damageTextTimeDict.Clear();
        rangeDamageTickTimeDict.Clear();
    }

}
