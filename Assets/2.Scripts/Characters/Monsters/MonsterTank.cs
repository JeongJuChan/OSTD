using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTank : Monster
{
    protected override void Awake()
    {
        base.Awake();
        monsterStateModule.AddMonsterState(MonsterStateType.Attack, new MonsterAttackState(monsterStateModule, this));
        monsterStateModule.AddMonsterState(MonsterStateType.Forward, new MonsterForwardState(monsterStateModule, this));
        monsterStateModule.ChangeState(MonsterStateType.Forward);
    }

    protected override void FixedUpdate() 
    {
        base.FixedUpdate();
        // CheckAttackRay();
    }

    protected override void Move()
    {
        float targetPosX = transform.position.x;
        float targetPosY = transform.position.y;

        float moveSpeed = totalSpeed * movingRate;
        targetPosX += isForward ? -moveSpeed * Time.deltaTime : moveSpeed * Time.deltaTime;
        if (!isHeroDead)
        {
            targetPosX = Mathf.Max(boxManagerTrans.position.x + boxHalfSizeX + circleCollider.radius, targetPosX);
        }

        transform.position = new Vector2(targetPosX, targetPosY);
    }
}
