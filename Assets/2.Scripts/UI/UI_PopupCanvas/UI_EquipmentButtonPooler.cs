using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentButtonPooler : UI_EquipmentPooler
{
    [SerializeField] private UI_Inventory ui_Inventory;

    protected override void CreateNewUI()
    {
        UI_EquipmentButton ui_EquipmentButton = Instantiate(prefab, transform) as UI_EquipmentButton;
        ui_EquipmentButton.Initialize(ReturnUI);
        ui_EquipmentButton.AddButtonAction(() => ui_Inventory.ShowComparingEquipmentUI(ui_EquipmentButton));
        ReturnUI(ui_EquipmentButton);
    }
}
