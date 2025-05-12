using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct EnergyData
{
    public int level;
    public float recoveryAmountPerSec;
    public BigInteger cost;

    public EnergyData(int level, float recoveryAmountPerSec, BigInteger cost)
    {
        this.level = level;
        this.recoveryAmountPerSec = recoveryAmountPerSec;
        this.cost = cost;
    }
}
