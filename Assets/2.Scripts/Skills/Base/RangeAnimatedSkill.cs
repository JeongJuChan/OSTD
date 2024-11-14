using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RangeAnimatedSkill : AnimatedSkill, ISkillRange
{
    //private TargetInRangeTrigger targetInRangeTrigger;

    private List<Monster> targets = new List<Monster>();

    private float skillDamagePercent;

    private bool isVibrated;

    private BigInteger damage;

    protected override void Awake()
    {
        base.Awake();
        //targetInRangeTrigger = GetComponentInChildren<TargetInRangeTrigger>();
        //targetInRangeTrigger.OnTargetAdded += AddTarget;
    }

    public override void Use(SkillData skillData, Monster monster, BigInteger damage)
    {
        if (monster == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        this.damage = damage;

        targets.Clear();

        // isVibrated = skillData.isVibrated;


        // skillDamagePercent = skillData.skillUpgradableData.damagePerecent;

        // // transform.position = monster.GetSkillPivot().position;

        // OnDamageTarget?.Invoke(skillData.skillUpgradableData.damagePerecent / Consts.PERCENT_DIVIDE_VALUE, monster,
        //     monster.GetDamageTextPivot().position, skillData.isVibrated, damage);
    }

    public void AddTarget(Monster monster)
    {
        if (!targets.Contains(monster))
        {
            targets.Add(monster);
            OnDamageTarget(skillDamagePercent / Consts.PERCENT_DIVIDE_VALUE, monster, monster.GetDamageTextPivot().position, isVibrated, damage);
        }
    }
}
