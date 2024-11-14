using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct RewardData
{
    public RewardType rewardType;
    public BigInteger rewardCount;

    public RewardData(RewardType rewardType, BigInteger rewardCount)
    {
        this.rewardType = rewardType;
        this.rewardCount = rewardCount;
    }
}
