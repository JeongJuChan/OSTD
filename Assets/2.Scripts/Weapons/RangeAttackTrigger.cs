using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangeAttackTrigger<T> : MonoBehaviour
{
    public HashSet<T> monsters { get; } = new HashSet<T>();
    public HashSet<T> waitForRemoveMonsters { get; } = new HashSet<T>();

    [SerializeField] private Collider2D collider2D;

    public void UpdateColliderActiveState(bool isActive)
    {
        collider2D.enabled = isActive;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Consts.MONSTER_TAG))
        {
            if (other.TryGetComponent(out T monster))
            {
                monsters.Add(monster);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Consts.MONSTER_TAG))
        {
            if (other.TryGetComponent(out T monster))
            {
                waitForRemoveMonsters.Add(monster);
            }
        }
    }

    public void ClearMonsterSets()
    {
        monsters.Clear();
        waitForRemoveMonsters.Clear();
    }
}
