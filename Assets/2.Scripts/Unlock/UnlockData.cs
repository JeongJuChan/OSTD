using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnlockData
{
    public FeatureType featureType;
    public int count;
    public FeatureID featureID;

    public UnlockData(FeatureType featureType, int count, FeatureID featureID)
    {
        this.featureType = featureType;
        this.count = count;
        this.featureID = featureID;
    }
}
