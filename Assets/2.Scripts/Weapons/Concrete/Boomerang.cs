using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MeleeTickWeapon
{
    private float distanceMaxX;
    private float cameraQuaterSize;
    private float minPosY;
    private float boxSizeY;
    private float distanceMaxY;

    public override void Init()
    {
        base.Init();
        GameManager.instance.OnReset += ResetAnimation;
        cameraQuaterSize = Camera.main.orthographicSize * Consts.HALF * Consts.HALF;
        distanceMaxX = BoxManager.instance.GetBoxSizeX() * Consts.HALF + Camera.main.orthographicSize * Consts.HALF;
        boxSizeY = BoxManager.instance.GetBoxSizeY();
        minPosY = BoxManager.instance.transform.position.y - boxSizeY * Consts.HALF;
        distanceMaxY = boxSizeY + boxSizeY * Consts.HALF;
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
            BoomerangProjectile boomerangProjectile = pooler.Pool((int)projectileType, firePoint.position, firePoint.rotation) as BoomerangProjectile;
            bool isUpper = i % 2 != 0;

            float middleDistanceX = distanceMaxX - cameraQuaterSize;
            float ranDistanceY = UnityEngine.Random.Range(boxSizeY * Consts.HALF, distanceMaxY);
            float firstDistanceY = isUpper ? ranDistanceY : -ranDistanceY;
            firstDistanceY = firstDistanceY < minPosY ? minPosY : firstDistanceY;

            ranDistanceY = UnityEngine.Random.Range(boxSizeY * Consts.HALF, distanceMaxY);
            float secondDistanceY = isUpper ? -ranDistanceY : ranDistanceY;
            secondDistanceY = secondDistanceY < minPosY ? minPosY : secondDistanceY;

            boomerangProjectile.SetMiddlePosX(middleDistanceX);
            boomerangProjectile.SetShooterTransform(transform);
            boomerangProjectile.SetFirstDistanceY(firstDistanceY);
            boomerangProjectile.SetSecondDistance(secondDistanceY);
            boomerangProjectile.SetDistanceMaxX(distanceMaxX);

            boomerangProjectile.SetShotPower(projectileSpeed);
            boomerangProjectile.SetDisableDelayTime(skillDisableTime);
            boomerangProjectile.UpdateDamage(weaponData.skillDamage);
            int index = weaponData.level - 1;
            index = index >= animators.Length ? animators.Length - 1 : index;
            boomerangProjectile.UpdateAnimIndex(index);
            boomerangProjectile.Fire();

            if (isUpper)
            {
                yield return skillIntervalSeconds;
            }
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
