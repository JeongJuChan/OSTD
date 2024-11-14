using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterJumpState : StateBase
{
    public MonsterJumpState(MonsterStateModule monsterStateModule, Monster ownerMonster) : base(monsterStateModule, ownerMonster)
    {
    }

    public override void EnterState(IState.ParamsAbstract parameters)
    {
        Monster forwardMonster = ownerMonster.FindForwardMonsterWithoutJumpOrFall();
        if (forwardMonster)
        {
            float jumpTargetY = forwardMonster.transform.position.y + forwardMonster.GetColliderWidth();
            ownerMonster.JumpTo(jumpTargetY);
        }

        canTransitionToOtherState = false;
    }

    public override void UpdateState()
    {
        if (ownerMonster.movementStateType != MonsterMovementStateType.Jump)
        {
            canTransitionToOtherState = true;
            monsterStateModule.ChangeState(MonsterStateType.Forward);
        }
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.Jump;
    }
}
