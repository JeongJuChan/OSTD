using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public abstract class ParamsAbstract { }
    public bool canTransitionToOtherState { get; }

    void EnterState(ParamsAbstract parameters);
    void FixedUpdateState();
    void UpdateState();
    void ExitState();
    MonsterStateType GetStateType();

}
