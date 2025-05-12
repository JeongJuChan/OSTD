using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class WeaponUpgradeModule
{
    public event Action<WeaponType, int, BigInteger, BigInteger, BigInteger, BigInteger, BigInteger, BigInteger> OnUpdateWeaponUIUpgraded;
    public event Action<WeaponType> OnUpgradeWeapon;
    private int[] weaponLevels;

    private WeaponResourceDataHandler weaponResourceDataHandler;

    private WeaponType[] weaponTypes;

    private CurrencyManager currencyManager;
    private CurrencyType researchType = CurrencyType.Research;

    public WeaponUpgradeModule()
    {
        currencyManager = CurrencyManager.instance;

        weaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));
        weaponLevels = DataBaseManager.instance.Load(Consts.WEAPON_UPGRADE_LEVELS, new int[weaponTypes.Length - 1]);

        weaponResourceDataHandler = ResourceManager.instance.weapon;

        if (weaponLevels[0] == 0)
        {
            for (int i = 0; i < weaponLevels.Length; i++)
            {
                weaponLevels[i] = 1;
            }
        }
    }

    public void Init()
    {
        UpdateWeaponUIs();
    }

    private void UpdateWeaponUIs()
    {
        for (int i = 1; i < weaponTypes.Length; i++)
        {
            ApplyWeaponUIUpgraded(weaponTypes[i]);
        }
    }

    private void ApplyWeaponUIUpgraded(WeaponType weaponType)
    {
        int level = weaponLevels[(int)weaponType - 1];
        WeaponUpgradeData weaponUpgradeData = GetCurrentWeaponUpgradeData(weaponType);
        BigInteger weaponCurrencyAmount = currencyManager.GetCurrencyValue(EnumUtility.GetCurrencyTypeByWeaponType(weaponType));
        BigInteger researchCurrencyAmount = currencyManager.GetCurrencyValue(researchType);
        OnUpdateWeaponUIUpgraded?.Invoke(weaponType, weaponUpgradeData.level, weaponUpgradeData.currentDamage, weaponUpgradeData.afterDamage,
            weaponCurrencyAmount, weaponUpgradeData.weaponCurrencyCost, researchCurrencyAmount, weaponUpgradeData.researchCost);
    }

    public BigInteger GetDamageUpgraded(WeaponType weaponType, BigInteger originDamage)
    {
        int level = weaponLevels[(int)weaponType - 1];
        if (level == 1)
        {
            return originDamage;
        }

        WeaponUpgradeData currentWeaponUpgradeData = GetWeaponUpgradeData(weaponType, level);
        WeaponUpgradeData beforeWeaponUpgradeData = GetWeaponUpgradeData(weaponType, 1);
        BigInteger totalDamage = originDamage + currentWeaponUpgradeData.currentDamage - beforeWeaponUpgradeData.currentDamage;
        return totalDamage;
    }

    public BigInteger GetSkillDamageUpgraded(WeaponType weaponType, BigInteger damage, BigInteger skillOriginDamage)
    {
        int level = weaponLevels[(int)weaponType - 1];
        if (level == 1)
        {
            return skillOriginDamage;
        }

        WeaponUpgradeData currentWeaponUpgradeData = GetWeaponUpgradeData(weaponType, level);
        WeaponUpgradeData beforeWeaponUpgradeData = GetWeaponUpgradeData(weaponType, level - 1);
        
        BigInteger totalDamage = skillOriginDamage * (damage + (currentWeaponUpgradeData.currentDamage - beforeWeaponUpgradeData.currentDamage)) * 
            Consts.PERCENT_DIVIDE_VALUE / (damage * Consts.PERCENT_DIVIDE_VALUE);
        return totalDamage;
    }

    public WeaponUpgradeData GetCurrentWeaponUpgradeData(WeaponType weaponType)
    {
        int level = weaponLevels[(int)weaponType - 1];
        return GetWeaponUpgradeData(weaponType, level);
    }

    public WeaponUpgradeData GetWeaponUpgradeData(WeaponType weaponType, int level)
    {
        WeaponUpgradeData weaponUpgradeData = weaponResourceDataHandler.GetWeaponUpgradeData(weaponType, level);
        return weaponUpgradeData;
    }

    public void UpgradeWeapon(WeaponType weaponType)
    {
        int level = weaponLevels[(int)weaponType - 1];
        WeaponUpgradeData weaponUpgradeData = weaponResourceDataHandler.GetWeaponUpgradeData(weaponType, level);
        CurrencyType weaponCurrencyType = EnumUtility.GetCurrencyTypeByWeaponType(weaponType);
        BigInteger weaponCurrencyAmount = currencyManager.GetCurrency(weaponCurrencyType).GetCurrencyValue();
        BigInteger researchAmount = currencyManager.GetCurrencyValue(researchType);
        BigInteger weaponCurrencyCost = weaponUpgradeData.weaponCurrencyCost;
        BigInteger researchCost = weaponUpgradeData.researchCost;

        if (weaponCurrencyAmount < weaponUpgradeData.weaponCurrencyCost || researchAmount < weaponUpgradeData.researchCost)
        {
            return;
        }

        currencyManager.TryUpdateCurrency(weaponCurrencyType, -weaponCurrencyCost);
        currencyManager.TryUpdateCurrency(researchType, -researchCost);

        weaponLevels[(int)weaponType - 1]++;
        OnUpgradeWeapon?.Invoke(weaponType);
        DataBaseManager.instance.Save(Consts.WEAPON_UPGRADE_LEVELS, weaponLevels);

        UpdateWeaponUIs();
    }
}
