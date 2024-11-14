using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMonsterController
{
    public event Action<Monster> OnTargetAdded;
    public event Action<Monster> OnTargetRemoved;

    #region FieldTargetEvent
    public void AddTarget(Monster monster)
    {
        monster.OnRemoveTargetAction += RemoveTarget;
        monster.ToggleInvincible(false);
        OnTargetAdded?.Invoke(monster);
    }

    public void RemoveTarget(Monster monster)
    {
        monster.OnRemoveTargetAction -= RemoveTarget;
        OnTargetRemoved?.Invoke(monster);
    }
    #endregion
}
