using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private GameObject weaponStatPanel;
    [SerializeField] private TextMeshProUGUI currentStatText;
    [SerializeField] private TextMeshProUGUI nextStatText;
    [SerializeField] private GameObject lockIconObject;

    [SerializeField] private GameObject currencyPanel;
    [SerializeField] private ChangeableCurrencyPanel weaponCurrencyPanel;
    [SerializeField] private ChangeableCurrencyPanel researchCurrencyPanel;

    public void UnlockWeaponInfo(CurrencyType weaponCurrencyType)
    {
        levelText.gameObject.SetActive(true);
        weaponImage.color = Color.white;
        weaponStatPanel.SetActive(true);
        lockIconObject.SetActive(false);
        currencyPanel.SetActive(true);
        weaponCurrencyPanel.UpdateCurrencySprite(CurrencyManager.instance.GetCurrency(weaponCurrencyType).GetIcon());
        researchCurrencyPanel.UpdateCurrencySprite(CurrencyManager.instance.GetCurrency(CurrencyType.Research).GetIcon());
    }

    public void SetWeaponSprite(Sprite weaponSprite)
    {
        weaponImage.sprite = weaponSprite;
    }

    public void UpdateWeaponInfo(int level, BigInteger currentStat, BigInteger nextStat)
    {
        levelText.text = $"Lv{level}";
        currentStatText.text = currentStat.ToString();
        nextStatText.text = nextStat.ToString();
    }

    public void UpdateWeaponCurrencyText(BigInteger currentCurrency, BigInteger cost)
    {
        weaponCurrencyPanel.UpdateCurrency(currentCurrency, cost);
    }

    public void UpdateResearchCurrencyText(BigInteger currentCurrency, BigInteger cost)
    {
        researchCurrencyPanel.UpdateCurrency(currentCurrency, cost);
    }
}
