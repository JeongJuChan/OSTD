using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct HeroStatData
{
    public BigInteger damage;
    public BigInteger health;

    public HeroStatData(BigInteger damage, BigInteger health)
    {
        this.damage = damage;
        this.health = health;
    }
}
