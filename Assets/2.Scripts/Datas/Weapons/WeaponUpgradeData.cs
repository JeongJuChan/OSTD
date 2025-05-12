using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct WeaponUpgradeData
{
    public WeaponType weaponType;
    public int level;
    public BigInteger weaponCurrencyCost;
    public BigInteger researchCost;
    public BigInteger currentDamage;
    public BigInteger afterDamage;

    public WeaponUpgradeData(WeaponType weaponType, int level, BigInteger weaponResearchCost, BigInteger researchCost, BigInteger currentDamage, 
        BigInteger afterDamage)
    {
        this.weaponType = weaponType;
        this.level = level;
        this.weaponCurrencyCost = weaponResearchCost;
        this.researchCost = researchCost;
        this.currentDamage = currentDamage;
        this.afterDamage = afterDamage;
    }
}
