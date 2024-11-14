using System.Collections;
using UnityEngine;

public interface ISkillThrow
{
    Vector2 departPos { get; }

    void SetDepartPosition(Vector2 departPos);
    float GetMovingDuration();
    IEnumerator CoMoveToTarget(Transform target);
}