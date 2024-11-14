using System.Collections.Generic;
using UnityEngine;

public class MonsterStateModule
{
    private IState currentState;
    private readonly Dictionary<MonsterStateType, IState> stateByTypeDict = new Dictionary<MonsterStateType, IState>();
    public Animator animator { get; protected set; }

    public void AddMonsterState(MonsterStateType monsterStateType, StateBase stateBase)
    {
        stateByTypeDict.Add(monsterStateType, stateBase);
    }

    public void Update() 
    {
        currentState.UpdateState();
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void ChangeState(MonsterStateType newStateType, IState.ParamsAbstract parameters = null)
    {
        if (!CanTransition(newStateType))
        {
            return;
        }

        if (currentState != null)
        {
            currentState.ExitState();
        }

        if (stateByTypeDict.TryGetValue(newStateType, out IState newState))
        {
            newState.EnterState(parameters);
            currentState = newState;
        }
    }

    public void ResetState(IState.ParamsAbstract parameters = null)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        if (stateByTypeDict.TryGetValue(MonsterStateType.Forward, out IState newState))
        {
            newState.EnterState(parameters);
            currentState = newState;
        }
    }

    private bool CanTransition(MonsterStateType newStateType)
    {
        if (currentState == null || currentState.canTransitionToOtherState)
        {
            return true;
        }

        if (newStateType == MonsterStateType.Attack || newStateType == MonsterStateType.Backward)
        {
            return true;
        }

        return false;
    }

    public MonsterStateType GetCurrentStateType()
    {
        return currentState.GetStateType();
    }
}
