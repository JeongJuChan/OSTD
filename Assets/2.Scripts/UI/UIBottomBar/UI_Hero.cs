using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hero : UI_BottomElement
{
    [SerializeField] private UI_Inventory ui_Inventory;
    [SerializeField] private UI_HeroStatPanel ui_HeroStatPanel;
    [SerializeField] private UI_CurrencyTextPanel enforcePowderTextPanel;
    [SerializeField] private UI_CurrencyTextPanel researchTextPanel;
    private Inventory inventory;
    [SerializeField] private UI_HeroEquipmentController ui_HeroEquipmentController;
    [SerializeField] private UI_EquipmentLevelUpPanel ui_EquipmentLevelUpPanel;
    [SerializeField] private UI_CompareEquipmentPanel ui_CompareEquipmentPanel;
    [SerializeField] private UI_InventoryButtonsPanel ui_InventoryButtonsPanel;

    public event Action OnUIOpened;

    public override void Initialize()
    {
        base.Initialize();
        ui_Inventory.Init();
        inventory = EquipmentManager.instance.inventory;
        inventory.OnUpdateInventoryUI += ui_Inventory.UpdateUI;
        CurrencyManager currencyManager = CurrencyManager.instance;
        ui_HeroStatPanel.Init();
        Currency enforcePowderCurrency = currencyManager.GetCurrency(enforcePowderTextPanel.GetCurrencyType());
        enforcePowderCurrency.OnCurrencyChange += enforcePowderTextPanel.UpdateCurrencyText;
        enforcePowderTextPanel.Init();
        enforcePowderTextPanel.UpdateCurrencyText(enforcePowderCurrency.GetCurrencyValue());
        Currency researchCurrency = currencyManager.GetCurrency(researchTextPanel.GetCurrencyType());
        researchCurrency.OnCurrencyChange += researchTextPanel.UpdateCurrencyText;
        researchTextPanel.UpdateCurrencyText(researchCurrency.GetCurrencyValue());
        researchTextPanel.Init();
        ui_EquipmentLevelUpPanel.Init();
        ui_HeroEquipmentController.Init();
        ui_CompareEquipmentPanel.Init();
        ui_Inventory.OnUpdateNewEquipment += inventory.UpdateCurrentEquipmentStatData;
        ui_InventoryButtonsPanel.Init();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        inventory.UpdateInventoryUI();
        EquipmentManager.instance.UpdateCurrentEquipment();
        EquipmentManager.instance.UpdateAutoEquipButtonActiveState();
        EquipmentManager.instance.UpdateSellDuplicateButtonActiveState();

        OnUIOpened?.Invoke();

        if (!DataBaseManager.instance.ContainsKey(Consts.GRANADE_TOUCHED_GUIDE))
        {
            UIManager.instance.GetUIElement<UI_HeroTutorialPanel>().AddTouchGranadeTutorial();
        }
    }
}
