using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] private CommonMonster monster;

    private bool isAttackState;

    // private void OnTriggerEnter2D(Collider2D other) 
    // {
    //     if (!isAttackState && other.gameObject.tag == Consts.HERO_TAG)
    //     {
    //         monster.UpdateAttackState(other);
    //         isAttackState = true;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other) 
    // {
    //     if (other.gameObject.tag == Consts.HERO_TAG)
    //     {
    //         monster.UpdateAttackState(null);
    //         isAttackState = false;
    //     }
    // }
}
