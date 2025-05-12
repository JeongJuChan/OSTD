using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

public class UI_CompareEquipmentPanel : UI_Base
{
    [SerializeField] private ComparingEquipmentInfoPanel currentEquipmentInfoPanel;
    [SerializeField] private NewEquipmentInfoPanel newEquipmentInfoPanel;

    [SerializeField] private Button disableButton;
    [SerializeField] private UI_EquipButton ui_EquipButton;

    public override void Init()
    {
        base.Init();
        EquipmentManager equipmentManager = EquipmentManager.instance;
        equipmentManager.OnUpdateEquipmentStatType += currentEquipmentInfoPanel.UpdateEquipmentStatType;
        equipmentManager.OnUpdateEquipmentStatType += newEquipmentInfoPanel.UpdateEquipmentStatType;
        equipmentManager.OnUpdateComparingCurrentEquipmentUI += currentEquipmentInfoPanel.UpdateEquipmentInfos;
        equipmentManager.OnUpdateComparingNewEquipmentUI += newEquipmentInfoPanel.UpdateComparingNewEquipmentUI;
        equipmentManager.OnOpenComparisonPanel += OpenUI;
        disableButton.onClick.AddListener(CloseUI);
        ui_EquipButton.Init();
        ui_EquipButton.AddButtonAction(CloseUI);
        CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        disableButton.gameObject.SetActive(true);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        disableButton.gameObject.SetActive(false);
    }
}
