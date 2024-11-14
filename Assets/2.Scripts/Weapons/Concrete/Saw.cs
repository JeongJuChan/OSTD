using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MeleeTickWeapon
{
    public override void Init()
    {
        base.Init();
        GameManager.instance.OnReset += ResetAnimation;
    }

    public override void ApplyDamage()
    {
        if (!GameManager.instance.isGameState)
        {
            return;
        }

        if (monsters.Count > 0)
        {
            UpdateAttackState(true);
        }

        foreach (Monster monster in monsters)
        {
            if (!monster.isDead)
            {
                BattleManager.instance.OnMonsterAttacked(monster, this, monster.GetDamageTextPivot().position, false, weaponData.damage);
            }
        }

        foreach (Monster removingMonster in waitForRemoveMonsters)
        {
            monsters.Remove(removingMonster);
        }

        waitForRemoveMonsters.Clear();
    }

    public override void UseSkill()
    {
        StartCoroutine(CoUseSkill());
    }

    protected override IEnumerator CoUseSkill()
    {
        for (int i = 0; i < skillCount; i++)
        {
            SawProjectile sawProjectile = pooler.Pool((int)projectileType, firePoint.position, firePoint.rotation) as SawProjectile;
            sawProjectile.SetShotPower(projectileSpeed);
            sawProjectile.SetDisableDelayTime(skillDisableTime);
            sawProjectile.UpdateDamage(weaponData.skillDamage);
            int index = weaponData.level - 1;
            index = index >= animators.Length ? animators.Length - 1 : index;
            sawProjectile.UpdateAnimIndex(index);
            sawProjectile.Fire();
            yield return skillIntervalSeconds;
        }
    }

    private void UpdateAttackState(bool isAttacking)
    {
        animator.SetBool(AnimatorParameters.IS_IDLE_HASH, !isAttacking);
        animator.SetBool(AnimatorParameters.IS_ATTACK_HASH, isAttacking);
    }

    private void ResetAnimation()
    {
        if (animator != null)
        {
            UpdateAttackState(false);
        }
    }
}
