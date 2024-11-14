using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BluePrintElement : CurrencyTextPanel
{
    [SerializeField] private Image currencyImage;

    public override void Init()
    {
        base.Init();
        currencyImage.sprite = currency.GetIcon();
    }
}
