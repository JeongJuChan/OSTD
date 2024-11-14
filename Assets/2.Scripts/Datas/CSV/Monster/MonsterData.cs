using System;
using Keiwando.BigInteger;

[Serializable]
public struct MonsterData
{
    public int index;
    public string name;
    public MonsterType monsterType;
    public BigInteger damage;
    public BigInteger health;
    public float speed;

    public MonsterData(int index, string name, MonsterType monsterType, BigInteger damage, BigInteger health, float speed)
    {
        this.index = index;
        this.name = name;
        this.monsterType = monsterType;
        this.damage = damage;
        this.health = health;
        this.speed = speed;
    }
}
