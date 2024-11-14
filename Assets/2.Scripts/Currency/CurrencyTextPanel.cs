using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyTextPanel : MonoBehaviour
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private TextMeshProUGUI currencyText;

    protected Currency currency;

    public virtual void Init()
    {
        currency = CurrencyManager.instance.GetCurrency(currencyType);
        CurrencyManager.instance.GetCurrency(currencyType).OnCurrencyChange += UpdateCurrencyText;
        UpdateCurrencyText(currency.GetCurrencyValue());
    }

    public CurrencyType GetCurrencyType()
    {
        return currencyType;
    }

    private void UpdateCurrencyText(BigInteger value)
    {
        currencyText.text = value.ChangeMoney();
    }
}
