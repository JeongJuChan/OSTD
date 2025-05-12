using System;
using Keiwando.BigInteger;

[Serializable]
public struct WeaponData
{
    public WeaponType weaponType;
    public int level;
    public int upgradeBlockCount;
    public BigInteger damage;
    public BigInteger goldCost;
    public BigInteger skillDamage;

    public WeaponData(WeaponType weaponType, int level, int upgradeBlockCount, BigInteger damage, BigInteger goldCost, BigInteger skillDamage)
    {
        this.weaponType = weaponType;
        this.level = level;
        this.upgradeBlockCount = upgradeBlockCount;
        this.damage = damage;
        this.goldCost = goldCost;
        this.skillDamage = skillDamage;
    }
}
