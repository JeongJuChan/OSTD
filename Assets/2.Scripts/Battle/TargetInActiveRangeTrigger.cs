using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetInActiveRangeTrigger : TargetInRangeTrigger
{
    #region ColliderTransformSettingMethods

    public void SetColliderSizeY(float sizeY)
    {
        boxCollider.size = new Vector2(boxCollider.size.x, sizeY);
    }


    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    #endregion
}
