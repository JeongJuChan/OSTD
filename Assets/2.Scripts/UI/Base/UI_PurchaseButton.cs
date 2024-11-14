using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;

public class UI_PurchaseButton : UI_Button
{
    [SerializeField] protected TextMeshProUGUI currencyText;
    [SerializeField] protected GameObject currencyImageObject;
    [SerializeField] protected CurrencyType currencyType;

    protected CurrencyManager currencyManager;

    public override void Init()
    {
        base.Init();
        currencyManager = CurrencyManager.instance;
    }

    public void UpdateCost(BigInteger cost)
    {
        currencyText.text = cost.ChangeMoney();
    }

    public void SetMaxText()
    {
        currencyText.text = "Max";
    }
}
