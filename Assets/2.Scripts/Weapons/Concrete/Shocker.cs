using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shocker : MeleeTickWeapon
{
    [SerializeField] private ParticleSystem attackEffect;

    [SerializeField] protected Vector2[] animPosArr;


    public override void ApplyDamage()
    {
        if (monsters.Count > 0 && !attackEffect.isPlaying)
        {
            attackEffect.Play();
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
            // firePoint 위치에서 발사체 생성
            ShootingProjectile electricShock = pooler.Pool((int)projectileType, firePoint.position, firePoint.rotation) as ShootingProjectile;
            electricShock.SetShotPower(projectileSpeed);
            electricShock.SetDisableDelayTime(skillDisableTime);
            electricShock.UpdateDamage(weaponData.skillDamage);
            electricShock.Fire();
            yield return skillIntervalSeconds;
        }
    }

    public override void UpdateWeaponData(WeaponData weaponData)
    {
        base.UpdateWeaponData(weaponData);
        int index = weaponData.level - 1;

        if (index >= animators.Length)
        {
            return;
        }

        firePoint.localPosition = animPosArr[index];
    }
}
