using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationEventHandler : MonoBehaviour
{
    private Action attackAction;

    #region AttackActionMethods
    public void AddAttackAction(Action attackAction)
    {
        this.attackAction += attackAction;
    }

    public void RemoveAction(Action attackAction)
    {
        this.attackAction -= attackAction;
    }
    #endregion

    #region AnimationEventMethods
    void OnAttack()
    {
        attackAction?.Invoke();
    }
    #endregion
}
