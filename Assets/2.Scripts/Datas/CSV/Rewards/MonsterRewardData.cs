using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterRewardData
{
    public int baseGold;
    public int goldProbability;
    public int baseForgeTicket;
    public int forgeTicketProbability;

    public MonsterRewardData(int baseGold, int goldProbability, int baseForgeTicket, int forgeTicketProbability)
    {
        this.baseGold = baseGold;
        this.goldProbability = goldProbability;
        this.baseForgeTicket = baseForgeTicket;
        this.forgeTicketProbability = forgeTicketProbability;
    }
}
