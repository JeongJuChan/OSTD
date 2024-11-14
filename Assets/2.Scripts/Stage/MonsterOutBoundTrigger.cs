using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterOutBoundTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag(Consts.MONSTER_TAG))
        {
            if (other.TryGetComponent(out Monster monster))
            {
                monster.ReturnToPool();
            }
        }    
    }
}
