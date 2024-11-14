using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : StateBase
{
    public MonsterAttackState(MonsterStateModule monsterStateModule, Monster ownerMonster) : base(monsterStateModule, ownerMonster)
    {
    }

    public override void EnterState(IState.ParamsAbstract parameters)
    {
        canTransitionToOtherState = false;
        ownerMonster.Stop();
    }

    public override void FixedUpdateState()
    {
        Monster belowMonster = ownerMonster.FindBelowMonster(onlyGrounded: true);
        if (belowMonster != null && belowMonster != ownerMonster && belowMonster.monsterStateModule.GetCurrentStateType() != MonsterStateType.Backward &&
            belowMonster.monsterStateModule.GetCurrentStateType() == MonsterStateType.Attack && belowMonster.GetBackwardable())
        {
            // 아래 몬스터의 상태를 BACKWARD 변경
            MonsterBackwardState.Params backwardParams = new MonsterBackwardState.Params();
            backwardParams.targetPosX = ownerMonster.transform.position.x + ownerMonster.GetColliderWidth();
            belowMonster.monsterStateModule.ChangeState(MonsterStateType.Backward, backwardParams);
        }

        if (!ownerMonster.isGrounded && ownerMonster.movementStateType != MonsterMovementStateType.Fall)
        {
            float targetPosY = ownerMonster.transform.position.y - ownerMonster.GetColliderWidth();
            ownerMonster.Fall(targetPosY);
        }

        if (!ownerMonster.CanAttack() || ownerMonster.movementStateType == MonsterMovementStateType.Idle)
        {
            monsterStateModule.ChangeState(MonsterStateType.Forward);
        }
    }

    public override void ExitState()
    {
        ownerMonster.PlayAttackAnimation(false);
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.Attack;
    }
}
