using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipButton : UI_Button
{
    public override void Init()
    {
        base.Init();
        AddButtonAction(EquipmentManager.instance.EquipNewEquipment);
    }
}
