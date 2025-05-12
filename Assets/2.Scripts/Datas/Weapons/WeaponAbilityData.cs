using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WeaponAbilityData
{
    public AbilityType abilityType;
    public ArithmeticStatType arithmeticStatType;
    public int initEvolutionValue;
    public int increasingEvolutionValue;
    public int evolutionCountMax;

    public WeaponAbilityData(AbilityType abilityType, ArithmeticStatType arithmeticStatType, int initEvolutionValue, int increasingEvolutionValue, 
        int evolutionCountMax)
    {
        this.abilityType = abilityType;
        this.arithmeticStatType = arithmeticStatType;
        this.initEvolutionValue = initEvolutionValue;
        this.increasingEvolutionValue = increasingEvolutionValue;
        this.evolutionCountMax = evolutionCountMax;
    }
}
