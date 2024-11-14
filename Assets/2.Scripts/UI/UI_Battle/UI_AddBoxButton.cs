using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AddBoxButton : UI_PurchaseButton
{
    public override void Init()
    {
        base.Init();
        SetDisableColor(Consts.DISABLE_COLOR);
    }
}
