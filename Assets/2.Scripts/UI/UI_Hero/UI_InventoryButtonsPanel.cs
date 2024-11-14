using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryButtonsPanel : UI_Base
{
    [SerializeField] private Button autoEquipButton;
    [SerializeField] private Button sellDuplicateButton;

    public override void Init()
    {
        base.Init();
        autoEquipButton.onClick.AddListener(EquipmentManager.instance.AutoEquip);
        autoEquipButton.onClick.AddListener(() => UpdateAutoEquipButtonActiveState(false));
        sellDuplicateButton.onClick.AddListener(EquipmentManager.instance.SellDuplicates);
        sellDuplicateButton.onClick.AddListener(() => UpdateSellDuplicateButtonActiveState(false));
        EquipmentManager.instance.OnUpdateAutoEquipButtonInteractable += UpdateAutoEquipButtonActiveState;
        EquipmentManager.instance.OnUpdateSellDupicateButtonInteractable += UpdateSellDuplicateButtonActiveState;
    }

    public void UpdateAutoEquipButtonActiveState(bool isInteractable)
    {
        autoEquipButton.interactable = isInteractable;
        NotificationManager.instance.SetNotification(RedDotIDType.Hero_Equip_Best, isInteractable);
        if (DataBaseManager.instance.ContainsKey(Consts.HERO_TAP_TOUCHED_GUIDE))
        {
            NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Hero, UIManager.instance.GetUIElement<UI_Hero>().GetRedDotActiveState());
        }
    }

    public void UpdateSellDuplicateButtonActiveState(bool isInteractable)
    {
        sellDuplicateButton.interactable = isInteractable;
        NotificationManager.instance.SetNotification(RedDotIDType.Hero_Sell_Duplicates, isInteractable);
        if (DataBaseManager.instance.ContainsKey(Consts.HERO_TAP_TOUCHED_GUIDE))
        {
            NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Hero, UIManager.instance.GetUIElement<UI_Hero>().GetRedDotActiveState());
        }
    }

}
