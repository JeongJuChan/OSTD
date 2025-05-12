using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeableCurrencyPanel : MonoBehaviour
{
    [SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI currencyText;

    public void UpdateCurrency(BigInteger currentCurrency, BigInteger cost)
    {
        currencyText.text = $"{currentCurrency}/{cost}";
    }

    public void UpdateCurrencySprite(Sprite currencySprite)
    {
        if (currencySprite == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            currencyImage.sprite = currencySprite;
        }
    }
}
