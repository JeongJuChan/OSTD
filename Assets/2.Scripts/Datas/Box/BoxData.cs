using System;
using Keiwando.BigInteger;

[Serializable]
public struct BoxData
{
    public int level;
    public BigInteger hp;
    public BigInteger cost;

    public BoxData(int level, BigInteger hp, BigInteger cost)
    {
        this.level = level;
        this.hp = hp;
        this.cost = cost;
    }
}
