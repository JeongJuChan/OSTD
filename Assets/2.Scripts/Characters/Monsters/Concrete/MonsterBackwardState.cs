using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBackwardState : StateBase
{

    public class Params : IState.ParamsAbstract
    {
        public float targetPosX;
    }

    private IState.ParamsAbstract parameters;

    public MonsterBackwardState(MonsterStateModule monsterStateModule, Monster ownerMonster) : base(monsterStateModule, ownerMonster)
    {
    }

    public override void EnterState(IState.ParamsAbstract parameters)
    {
        canTransitionToOtherState = false;

        Monster behindMonster = ownerMonster.FindBehindAndAboveMonster();
        Monster forwardMonster = ownerMonster.FindForwardMonster();
        if (behindMonster == null && forwardMonster == null)
        {
            canTransitionToOtherState = true;
            monsterStateModule.ChangeState(MonsterStateType.Forward);
            return;
        }

        if (ownerMonster.movementStateType == MonsterMovementStateType.Fall)
        {
            this.parameters = parameters;
            return;
        }

        if (parameters is Params backwardParams)
        {
            behindMonster = ownerMonster.FindBehindMonster();

            ownerMonster.StartMoveBackward(backwardParams.targetPosX);

            if (behindMonster != null &&
                (behindMonster.monsterStateModule.GetCurrentStateType() == MonsterStateType.Forward ||
                behindMonster.monsterStateModule.GetCurrentStateType() == MonsterStateType.Backward))
            {
                Params nextMonsterBackwardParams = new Params();
                nextMonsterBackwardParams.targetPosX = backwardParams.targetPosX + ownerMonster.GetColliderWidth();
                behindMonster.monsterStateModule.ChangeState(MonsterStateType.Backward, nextMonsterBackwardParams);
            }
        }
    }

    public override void FixedUpdateState()
    {
        // if (ownerMonster.movementStateType != MonsterMovementStateType.Fall)
        // {
        //     canTransitionToOtherState = true;
        //     EnterState(parameters);
        // }

        if (ownerMonster.movementStateType != MonsterMovementStateType.Backward && ownerMonster.movementStateType != MonsterMovementStateType.Fall)
        {
            Monster behindMonster = ownerMonster.FindBehindAndAboveMonster();
            Monster forwardMonster = ownerMonster.FindForwardMonster();
            if (behindMonster == null || forwardMonster == null)
            {
                canTransitionToOtherState = true;
                monsterStateModule.ChangeState(MonsterStateType.Forward);
            }
        }
    }

    public override void ExitState()
    {
        
    }

    public override MonsterStateType GetStateType()
    {
        return MonsterStateType.Backward;
    }

    
}
