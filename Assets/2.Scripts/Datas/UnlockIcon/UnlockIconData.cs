using System;
using UnityEngine;

[Serializable]
public struct UnlockIconData
{
    public FeatureID featureID;
    public string iconName;
    public Sprite sprite;

    public UnlockIconData(FeatureID featureID, string iconName, Sprite sprite)
    {
        this.featureID = featureID;
        this.iconName = iconName;
        this.sprite = sprite;
    }
}
