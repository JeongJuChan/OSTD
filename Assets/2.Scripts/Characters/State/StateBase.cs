using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class StateBase : IState
{
    protected MonsterStateModule monsterStateModule;
    protected Monster ownerMonster;
    public bool canTransitionToOtherState { get; protected set; }

    public StateBase(MonsterStateModule monsterStateModule, Monster ownerMonster)
    {
        this.monsterStateModule = monsterStateModule;
        this.ownerMonster = ownerMonster;
    }


    public virtual void EnterState(IState.ParamsAbstract parameters)
    {
        
    }

    public virtual void FixedUpdateState()
    {
        
    }

    public virtual void UpdateState()
    {

    }

    public virtual void ExitState()
    {

    }

    public abstract MonsterStateType GetStateType();
}
