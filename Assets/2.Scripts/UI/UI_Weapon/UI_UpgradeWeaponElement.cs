using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_UpgradeWeaponElement : MonoBehaviour
{
    [SerializeField] private Image titleLabelImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;

    [SerializeField] private UI_WeaponInfoPanel ui_WeaponInfoPanel;

    [SerializeField] private UI_Button upgradeButton;

    [SerializeField] private TextMeshProUGUI unlockText;

    public void Init()
    {
        upgradeButton.Init();
    }

    public void SetUpgradeEvent(UnityAction OnClickButton)
    {
        upgradeButton.AddButtonAction(OnClickButton);
    }

    public void InitWeaponInfo(string weaponName, Sprite weaponSprite)
    {
        weaponNameText.text = weaponName;
        ui_WeaponInfoPanel.SetWeaponSprite(weaponSprite);
    }

    public void InitLockInfo(int level)
    {
        unlockText.text = $"레벨 {level}에서\n해금";
    }

    public void UnlockUpgradeWeaponUI(CurrencyType weaponCurrencyType)
    {
        titleLabelImage.enabled = true;
        ui_WeaponInfoPanel.UnlockWeaponInfo(weaponCurrencyType);
        upgradeButton.gameObject.SetActive(true);
        unlockText.gameObject.SetActive(false);
    }

    public void UpdateWeaponInfo(int level, BigInteger currentDamage, BigInteger nextDamage)
    {
        ui_WeaponInfoPanel.UpdateWeaponInfo(level, currentDamage, nextDamage);
    }

    public void UpdateWeaponCurrencyInfo(BigInteger currentWeaponCurrencyAmount, BigInteger weaponCurrencyCost)
    {
        ui_WeaponInfoPanel.UpdateWeaponCurrencyText(currentWeaponCurrencyAmount, weaponCurrencyCost); 
    }

    public void UpdateResearchCurrencyInfo(BigInteger currentResearchCurrencyAmount, BigInteger researchCurrencyCost)
    {
        ui_WeaponInfoPanel.UpdateResearchCurrencyText(currentResearchCurrencyAmount, researchCurrencyCost);
    }

    public void UpdateUpgradeButtonInteractable(bool isInteractable)
    {
        upgradeButton.UpdateInteractable(isInteractable);
    }
}
