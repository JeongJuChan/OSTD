using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTrigger : MonoBehaviour
{
    public event Action<MonsterBase> OnMonsterAttacked;

    [SerializeField] private Collider2D collider2D;

    public void UpdateColliderActiveState(bool isActive)
    {
        collider2D.enabled = isActive; 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag(Consts.MONSTER_TAG))
        {
            if (other.TryGetComponent(out MonsterBase monster))
            {
                OnMonsterAttacked?.Invoke(monster);
            } 
        }
    }
}
