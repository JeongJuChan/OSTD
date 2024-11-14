using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentLevelUpPanel : UI_Base
{
    [SerializeField] private Button closeButton;
    [SerializeField] private CurrentEquipmentTitlePanel currentEquipmentTitlePanel;
    [SerializeField] private EquipmentInfoPanel equipmentInfoPanel;
    [SerializeField] private ChangeableCurrencyPanel bluePrintCurrencyPanel;
    [SerializeField] private ChangeableCurrencyPanel enforcePowderCurrencyPanel;

    [SerializeField] private UI_HeroEquipmentController ui_HeroEquipmentController;

    [field: SerializeField] public UI_Button levelUpMaxButton { get; private set; }
    [field: SerializeField] public UI_Button levelUpButton { get; private set; }

    [SerializeField] private Canvas popupCanvas;

    private CurrencyManager currencyManager;

    public override void Init()
    {
        base.Init();
        currencyManager = CurrencyManager.instance;
        EquipmentManager equipmentManager = EquipmentManager.instance;

        enforcePowderCurrencyPanel.UpdateCurrencySprite(currencyManager.GetCurrency(CurrencyType.EnforcePowder).GetIcon());
        ui_HeroEquipmentController.OnUpdateEquipmentInfo += equipmentInfoPanel.UpdateEquipmentInfos;
        ui_HeroEquipmentController.OnUpdateEquipPanelTitleText += currentEquipmentTitlePanel.UpdateTitleText;

        ui_HeroEquipmentController.OnUpdateFirstCurrencyCost += bluePrintCurrencyPanel.UpdateCurrency;
        ui_HeroEquipmentController.OnOpenCurrentChosenEquipment += OpenUI;
        ui_HeroEquipmentController.OnUpdateSecondCurrencyCost += enforcePowderCurrencyPanel.UpdateCurrency;
        ui_HeroEquipmentController.OnUpdateFirstCurrencySprite += bluePrintCurrencyPanel.UpdateCurrencySprite;
        ui_HeroEquipmentController.OnUpdateSecondCurrencySprite += enforcePowderCurrencyPanel.UpdateCurrencySprite;
        ui_HeroEquipmentController.OnUpdateLevelUpButtonsInteractable += OnUpdateLevelUpButtonsInteractable;

        popupCanvas.sortingLayerName = Consts.POPUP_UI_LAYER;
        closeButton.onClick.AddListener(CloseUI);
        CloseUI();
        levelUpMaxButton.Init();
        levelUpButton.Init();

        levelUpButton.AddButtonAction(() => equipmentManager.LevelUpEquipment());
        levelUpMaxButton.AddButtonAction(() => equipmentManager.LevelUpMaxEquipment());
    }

    public override void OpenUI()
    {
        closeButton.gameObject.SetActive(true);
        base.OpenUI();
    }

    public override void CloseUI()
    {
        closeButton.gameObject.SetActive(false);
        base.CloseUI();
    }

    private void OnUpdateLevelUpButtonsInteractable(bool isInteractable)
    {
        levelUpMaxButton.UpdateInteractable(isInteractable);
        levelUpButton.UpdateInteractable(isInteractable);
    }
}
