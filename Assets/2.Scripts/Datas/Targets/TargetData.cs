using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TargetData
{
    public IDamageable damagable;
    public Transform targetTransform;

    public TargetData(IDamageable damagable, Transform targetTransform)
    {
        this.damagable = damagable;
        this.targetTransform = targetTransform;
    }
}
