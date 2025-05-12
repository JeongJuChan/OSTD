using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterRewardIncrementData
{
    public int goldIncrement;
    public int goldIncrementPercent;
    public int forgeTicketIncrement;
    public int forgeTicketIncrementPercent;

    public MonsterRewardIncrementData(int goldIncrement, int goldIncrementPercent, int forgeTicketIncrement, int forgeTicketIncrementPercent)
    {
        this.goldIncrement = goldIncrement;
        this.goldIncrementPercent = goldIncrementPercent;
        this.forgeTicketIncrement = forgeTicketIncrement;
        this.forgeTicketIncrementPercent = forgeTicketIncrementPercent;
    }
}
