using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CurrencyTextPanel : UI_Base
{
    [SerializeField] private Image currencyImage;
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private TextMeshProUGUI currencyText;

    public override void Init()
    {
        currencyText.text = "0";
        currencyImage.sprite = CurrencyManager.instance.GetCurrency(currencyType).GetIcon();
    }

    public CurrencyType GetCurrencyType()
    {
        return currencyType;
    }

    public void UpdateCurrencyText(BigInteger amount)
    {
        currencyText.text = amount.ChangeMoney();
    }
}
