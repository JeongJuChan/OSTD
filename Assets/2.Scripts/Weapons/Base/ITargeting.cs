using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargeting
{
    Transform target { get; }
    void SetTarget(Monster targetMonster);
    void SetTargetMonster();
}
