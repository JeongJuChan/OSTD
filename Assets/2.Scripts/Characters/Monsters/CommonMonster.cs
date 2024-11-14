using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonMonster : Monster
{
    [SerializeField] private GameObject shadow;

    protected override void Awake()
    {
        base.Awake();
        monsterStateModule.AddMonsterState(MonsterStateType.Attack, new MonsterAttackState(monsterStateModule, this));
        monsterStateModule.AddMonsterState(MonsterStateType.Forward, new MonsterForwardState(monsterStateModule, this));
        monsterStateModule.AddMonsterState(MonsterStateType.Backward, new MonsterBackwardState(monsterStateModule, this));
        monsterStateModule.AddMonsterState(MonsterStateType.Jump, new MonsterJumpState(monsterStateModule, this));

        monsterStateModule.ChangeState(MonsterStateType.Forward);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // CheckAttackRay();
    }

    protected override void Update()
    {
        base.Update();
        shadow.SetActive(!isDead && isGrounded);
    }
}
