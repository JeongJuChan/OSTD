using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnergyUpgradePanel : UI_Base
{
    [SerializeField] private UI_EnergyButton upgradeEnergyButton;
    [SerializeField] private TextMeshProUGUI energyPerSecText;
    [SerializeField] private TextMeshProUGUI currencyText;

    public event Action OnAddEnergy;

    public override void Init()
    {
        upgradeEnergyButton.Init();
        upgradeEnergyButton.AddButtonAction(SkillManager.instance.UpgradeEnergyLevel);
        SkillManager.instance.OnUpdateEnergyUI += UpdateUI;
        SkillManager.instance.OnUpdateEnergyUpgradeButtonState += UpdateButtonInteractableState;
        upgradeEnergyButton.SetDisableColor(Consts.DISABLE_COLOR);
    }

    private void UpdateUI(EnergyData energyData)
    {
        energyPerSecText.text = $"{energyData.recoveryAmountPerSec}/s";
        currencyText.text = energyData.cost.ChangeMoney();
    }

    public void UpdateButtonInteractableState(bool isInteractable)
    {
        upgradeEnergyButton.UpdateInteractable(isInteractable);
    }
}
