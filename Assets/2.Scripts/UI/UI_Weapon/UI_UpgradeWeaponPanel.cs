using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Events;

public class UI_UpgradeWeaponPanel : UI_Base
{
    [SerializeField] private UI_UpgradeWeaponElement[] ui_UpgradeWeaponElements;

    public override void Init()
    {
        WeaponType[] weaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));
        for (int i = 1; i < weaponTypes.Length; i++)
        {
            WeaponType weaponType = weaponTypes[i];

            UI_UpgradeWeaponElement ui_UpgradeWeaponElement = ui_UpgradeWeaponElements[i - 1];
            string weaponName = EnumUtility.GetWeaponTypeKR(weaponType);
            ui_UpgradeWeaponElement.Init();

            ui_UpgradeWeaponElement.InitWeaponInfo(weaponName, ResourceManager.instance.weapon.GetWeaponSprite(weaponType));

            ui_UpgradeWeaponElement.InitLockInfo(ResourceManager.instance.weapon.GetWeaponUnlockStage(weaponType));
        }
    }

    public void InitWeaponInfo(WeaponType weaponType, string weaponName, Sprite sprite, int level)
    {
        UI_UpgradeWeaponElement ui_UpgradeWeaponElement = ui_UpgradeWeaponElements[(int)weaponType - 1];
        ui_UpgradeWeaponElement.InitWeaponInfo(weaponName, sprite);
        ui_UpgradeWeaponElement.InitLockInfo(level);
    }

    public void UnlockWeaponUI(WeaponType weaponType, CurrencyType weaponCurrencyType)
    {
        UI_UpgradeWeaponElement ui_UpgradeWeaponElement = ui_UpgradeWeaponElements[(int)weaponType - 1];
        ui_UpgradeWeaponElement.UnlockUpgradeWeaponUI(weaponCurrencyType);
    }

    public void UpdateWeaponUI(WeaponType weaponType, int level, BigInteger beforeDamage, BigInteger afterDamage, BigInteger weaponCurrencyAmount, 
        BigInteger weaponCurrencyCost, BigInteger researchAmount, BigInteger researchCost)
    {
        UI_UpgradeWeaponElement ui_UpgradeWeaponElement = ui_UpgradeWeaponElements[(int)weaponType - 1];
        ui_UpgradeWeaponElement.UpdateWeaponInfo(level, beforeDamage, afterDamage);
        UpdateCurrencyUI(weaponType, weaponCurrencyAmount, weaponCurrencyCost, researchAmount, researchCost);
    }

    public void UpdateCurrencyUI(WeaponType weaponType, BigInteger weaponCurrencyAmount, BigInteger weaponCurrencyCost, BigInteger researchAmount, 
        BigInteger researchCost)
    {
        UI_UpgradeWeaponElement ui_UpgradeWeaponElement = ui_UpgradeWeaponElements[(int)weaponType - 1];
        ui_UpgradeWeaponElement.UpdateWeaponCurrencyInfo(weaponCurrencyAmount, weaponCurrencyCost);
        ui_UpgradeWeaponElement.UpdateResearchCurrencyInfo(researchAmount, researchCost);
        ui_UpgradeWeaponElement.UpdateUpgradeButtonInteractable(weaponCurrencyAmount >= weaponCurrencyCost && researchAmount >= researchCost);
    }

    public void SetUpgradeEvent(WeaponType weaponType, UnityAction OnUpgradeEvent)
    {
        UI_UpgradeWeaponElement ui_UpgradeWeaponElement = ui_UpgradeWeaponElements[(int)weaponType - 1];
        ui_UpgradeWeaponElement.SetUpgradeEvent(OnUpgradeEvent);
    }
}
