using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler
{
    public event Action OnActiveTargetStateChanged;

    private List<Monster> activeTargets = new List<Monster>();
    private List<Monster> inActiveTargets = new List<Monster>();

    #region TargetListMethods
    public List<Monster> GetActiveTargets()
    {
        return activeTargets;
    }

    public List<Monster> GetInActiveTargets()
    {
        return inActiveTargets;
    }

    public bool GetAreActiveTargetsExist()
    {
        return activeTargets.Count > 0;
    }

    public void AddActiveTarget(Monster monster)
    {
        bool isActiveStateChanged = activeTargets.Count == 0;

        monster.ToggleInvincible(false);
        activeTargets.Add(monster);
        monster.OnRemoveTargetAction += RemoveActiveTarget;

        RemoveInActiveTarget(monster);

        if (isActiveStateChanged)
        {
            OnActiveTargetStateChanged?.Invoke();
        }
    }

    public void RemoveActiveTarget(Monster monster)
    {
        activeTargets.Remove(monster);
        if (activeTargets.Count == 0)
        {
            OnActiveTargetStateChanged?.Invoke();
        }

        AddInActiveTarget(monster);
    }

    public void RemoveAllTarget()
    {
        Stack<Monster> monsters = new Stack<Monster>(GetActiveTargets());

        int count = inActiveTargets.Count;
        for (int i = 0; i < count; i++)
        {
            monsters.Push(inActiveTargets[i]);
        }

        KillTargets(monsters);

        activeTargets.Clear();
        inActiveTargets.Clear();
    }

    private void KillTargets(Stack<Monster> monsters)
    {
        while (monsters.Count != 0)
        {
            if (monsters.Pop().TryGetComponent(out IDie die))
            {
                die.Die(true);
            }
        }
    }

    public void AddInActiveTarget(Monster monster)
    {
        inActiveTargets.Add(monster);
    }

    private void RemoveInActiveTarget(Monster monster)
    {
        inActiveTargets.Remove(monster);
    }
    #endregion
}