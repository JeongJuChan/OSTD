using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterForwardState : StateBase
{
    public MonsterForwardState(MonsterStateModule monsterStateModule, Monster ownerMonster) : base(monsterStateModule, ownerMonster)
    {
    }

    public override void EnterState(IState.ParamsAbstract parameters)
    {
        canTransitionToOtherState = true;
        ownerMonster.StartMoveForward();
    }

    public override void FixedUpdateState()
    {
        if (ownerMonster.CanAttack())
        {
            monsterStateModule.ChangeState(MonsterStateType.Attack);
        }
        else
        {
            if (ownerMonster.isGrounded == false)
            {
                Monster belowMonster = ownerMonster.FindBelowMonster();
                if (!belowMonster || !belowMonster.FindForwardMonsterWithoutJumpOrFall() || !belowMonster.FindBehindMonster())
                {
                    float targetPosY = ownerMonster.transform.position.y - ownerMonster.GetColliderWidth();
                    ownerMonster.Fall(targetPosY);
                }
            }

            Monster forwardMonster = ownerMonster.FindForwardMonsterWithoutJumpOrFall();
            if (forwardMonster)
            {
                monsterStateModule.ChangeState(MonsterStateType.Jump);
            }
        }
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.Forward;
    }
}
