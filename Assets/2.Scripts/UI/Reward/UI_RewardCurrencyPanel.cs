using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardCurrencyPanel : MonoBehaviour, IPoolable<UI_RewardCurrencyPanel>
{
    private Action<UI_RewardCurrencyPanel> OnReturnAction;
    [SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI currencyAmountText;

    public void Initialize(Action<UI_RewardCurrencyPanel> returnAction)
    {
        OnReturnAction = returnAction;
    }

    public void ReturnToPool()
    {
        OnReturnAction?.Invoke(this);
    }

    public void UpdateCurrency(Sprite currencySprite, BigInteger amount)
    {
        currencyImage.sprite = currencySprite;
        currencyAmountText.text = amount.ChangeMoney();
    }
}
