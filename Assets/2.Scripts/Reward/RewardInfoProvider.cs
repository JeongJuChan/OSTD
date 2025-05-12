using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardInfoProvider
{
    private Dictionary<RewardType, IRewardInfo> rewardInfos;

    public RewardInfoProvider()
    {
        rewardInfos = new Dictionary<RewardType, IRewardInfo>
        {
            { RewardType.Gold, new GoldRewardInfo() },
            // Add other rewards similarly
        };
    }

    public IRewardInfo GetRewardInfo(RewardType type)
    {
        if (!rewardInfos.ContainsKey(type)) return null;
        return rewardInfos[type];
    }
}

