using Keiwando.BigInteger;
using System.Collections;
using UnityEngine;

public class ThunderStrike : AnimatedSkill
{ 
    public override void Use(SkillData skillData, Monster monster, BigInteger damage)
    {
        if (monster == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // transform.position = monster.GetSkillPivot().position;

        // OnDamageTarget(skillData.skillUpgradableData.damagePerecent / Consts.PERCENT_DIVIDE_VALUE, monster, 
        //     monster.GetDamageTextPivot().position, skillData.isVibrated, damage);
    }
}
