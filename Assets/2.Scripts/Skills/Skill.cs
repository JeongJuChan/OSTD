using Keiwando.BigInteger;
using System;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public Action<float, IDamageable, Vector2, bool, BigInteger> OnDamageTarget;
    public abstract void Use(SkillData skillData, Monster monster, BigInteger damage);
    protected int index = -1;
    public void SetIndex(int index)
    {
        this.index = index;
    }
}
