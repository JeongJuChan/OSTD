using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BoxWeaponPanel : UI_Base
{
    [SerializeField] private SelectWeaponPanel selectWeaponPanel;
    [field: SerializeField] public UI_UpgradeWeaponButtonPanel upgradeWeaponButton { get; private set; }
    [field: SerializeField] public UI_WeaponChoiceButton[] buttons { get; private set; }
    private WeaponResourceDataHandler weaponResourceDataHandler;

    private int boxIndex;

    private Currency currency;

    private WeaponManager weaponManager;

    public override void Init()
    {
        base.Init();
        upgradeWeaponButton.Init();
        weaponManager = WeaponManager.instance;
        weaponManager.OnUpdateCurrentWeaponType += UpdateWeaponType;
        weaponResourceDataHandler = ResourceManager.instance.weapon;

        currency = CurrencyManager.instance.GetCurrency(CurrencyType.Gold);
        currency.OnCurrencyChange += UpdateWeaponChoiceButtonsInteractable;

        upgradeWeaponButton.AddButtonAction(LevelUpWeapon);

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].Init();
            buttons[index].AddButtonAction(() => SetBoxWeapon(index));
            buttons[index].AddButtonAction(() => ChangeCurrencyByChoosingWeapon(index));
        }

        Reset();
    }

    private void UpdateWeaponType(int Index, WeaponType weaponType, WeaponSlotType weaponSlotType)
    {
        buttons[Index].UpdateWeaponSprite(weaponResourceDataHandler.GetWeaponSprite(weaponType));
        buttons[Index].UpdateCost(weaponResourceDataHandler.GetInitWeaponCost((int)weaponSlotType));
    }

    public void Reset()
    {
        UpdateSelectPanelActiveState(true);
    }

    public void UpdateWeaponChoiceButtonsInteractable(BigInteger amount)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].UpdateInteractable(amount >= weaponResourceDataHandler.GetInitWeaponCost((int)weaponManager.GetWeaponSlotType(i)));
        }
    }

    public void SetIndex(int boxIndex)
    {
        this.boxIndex = boxIndex;
    }

    public void SetBoxWeapon(int currentWeaponTypeIndex)
    {
        WeaponManager.instance.ChooseWeapon(boxIndex, currentWeaponTypeIndex);
        GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ADD_WEAPON_GUIDE, false);
        DataBaseManager.instance.Save(Consts.ADD_WEAPON_GUIDE, true);
    }

    public void ChangeCurrencyByChoosingWeapon(int index)
    {
        WeaponSlotType weaponSlotType = weaponManager.GetWeaponSlotType(index);
        currency.TryUpdateCurrency(-weaponResourceDataHandler.GetInitWeaponCost((int)weaponSlotType));
    }

    public void UpdateLevelUpWeaponButton(int level, int upgradeBlockCount, BigInteger damage, BigInteger cost, BigInteger currentCurrency)
    {
        upgradeWeaponButton.UpdateLevel(level);
        upgradeWeaponButton.UpgradeBlockCount(upgradeBlockCount);
        upgradeWeaponButton.UpdateDamage(damage);

        if (upgradeBlockCount == weaponResourceDataHandler.GetUpgradeBlockCountMax() - 1)
        {
            if (level == weaponResourceDataHandler.GetUpgradeLevelMax())
            {
                upgradeWeaponButton.SetMaxText();
                upgradeWeaponButton.UpdateInteractable(false);
                upgradeWeaponButton.UpdateEvolutionActiveState(false);
            }
            else
            {
                upgradeWeaponButton.UpdateEvolutionLevelText(level);
                upgradeWeaponButton.UpdateEvolutionActiveState(true);
            }
        }
        else
        {
            upgradeWeaponButton.UpdateCost(cost);
            upgradeWeaponButton.UpdateInteractable(currentCurrency >= cost);
            upgradeWeaponButton.UpdateEvolutionActiveState(false);
        }

    }

    public void UpdateSelectPanelActiveState(bool isActive)
    {
        selectWeaponPanel.UpdateActiveState(isActive);
        upgradeWeaponButton.UpdateActiveState(!isActive);
    }

    private void LevelUpWeapon()
    {
        WeaponManager.instance.LevelUpWeapon(boxIndex);
    }
}
