using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class RangeMonster : Monster
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private Transform shootingPivot;
    [SerializeField] private float shotPowerX = 5f;

    private Transform target;
    private ProjectilePooler projectilePooler;

    private float randomTargetPosX;

    protected override void Awake()
    {
        base.Awake();
        monsterStateModule.AddMonsterState(MonsterStateType.Attack, new RangeAttackState(monsterStateModule, this));
        monsterStateModule.AddMonsterState(MonsterStateType.Forward, new RangeForwardState(monsterStateModule, this));
        monsterStateModule.ChangeState(MonsterStateType.Forward);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ToggleInvincible(true);
        monsterStateModule.ResetState();
    }

    protected override void Move()
    {
        if (target)
        {
            return;
        }

        float targetPosX = transform.position.x;
        float targetPosY = transform.position.y;


        float moveSpeed = totalSpeed * movingRate;
        targetPosX += isForward ? -moveSpeed * Time.deltaTime : moveSpeed * Time.deltaTime;

        if (targetPosX <= randomTargetPosX)
        {
            targetPosX = randomTargetPosX;
            TryFindTarget();
        }
        if (!isHeroDead)
        {
            targetPosX = Mathf.Max(boxManagerTrans.position.x + boxHalfSizeX + circleCollider.radius, targetPosX);
        }

        transform.position = new Vector2(targetPosX, targetPosY);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Reset();
    }

    private void Reset()
    {
        target = null;
    }

    public override void Init()
    {
        base.Init();
        projectilePooler = PoolManager.instance.projectile;
        BoxManager.instance.OnInGameBoxRemoved += TryFindTarget;
    }
    public override void ToggleInvincible(bool isInvincible)
    {
        base.ToggleInvincible(isInvincible);
        if (isInvincible)
        {
            float halfPosX = (boxManagerTrans.position.x + boxHalfSizeX + circleCollider.radius + transform.position.x) * Consts.HALF;
            randomTargetPosX = UnityEngine.Random.Range(halfPosX - 1, halfPosX + 1);
        }
    }

    private void TryFindTarget()
    {
        if (this == null)
        {
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        
        target = BoxManager.instance.GetTargetTransform();
        if (target == null)
        {
            return;
        }

        PlayAttackAnimation(true);
        isMove = false;
    }


    public ProjectileType GetProjectileType()
    {
        return projectileType;
    }

    protected override void OnAttack()
    {
        if (target == null)
        {
            return;
        }

        Shoot();
    }

    private void Shoot()
    {
        MonsterProjectile projectile = projectilePooler.Pool((int)projectileType, shootingPivot.position, Quaternion.identity) as MonsterProjectile;
        projectile.SetTarget(target);
        projectile.SetShotPower(shotPowerX);
        projectile.UpdateDamage(monsterData.damage);
        projectile.Fire();
    }

}
