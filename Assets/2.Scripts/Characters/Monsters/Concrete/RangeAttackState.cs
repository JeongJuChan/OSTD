using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackState : MonsterAttackState
{
    public RangeAttackState(MonsterStateModule monsterStateModule, Monster ownerMonster) : base(monsterStateModule, ownerMonster)
    {
    }

    public override void FixedUpdateState()
    {
        if (!HeroManager.instance.hero.gameObject.activeInHierarchy)
        {
            monsterStateModule.ChangeState(MonsterStateType.Forward);
        }
    }
}
