using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CastleHPUIPanel : HPUIPanel
{
    [SerializeField] private TextMeshProUGUI hpText;

    public override void init(IHasHpUI hasHPUI)
    {
        base.init(hasHPUI);
        hpText.gameObject.SetActive(false);
        hpSlider.gameObject.SetActive(false);
    }

    public override void UpdateCurrentHPUI(BigInteger currentHp)
    {
        hpText.gameObject.SetActive(true);
        hpSlider.gameObject.SetActive(true);
        base.UpdateCurrentHPUI(currentHp);
        UpdateHpText();
    }

    public override void UpdateMaxHP(BigInteger maxHP, BigInteger currentHp)
    {
        base.UpdateMaxHP(maxHP, currentHp);
        UpdateHpText();
    }

    public override void ResetUI()
    {
        base.ResetUI();
        UpdateHpText();
    }

    private void UpdateHpText()
    {
        BigInteger front = preHP / Consts.THOUSAND_DIVIDE_VALUE;
        if (front == 0)
        {
            hpText.text = preHP.ToString();
        }
        else
        {
            BigInteger back = preHP % Consts.THOUSAND_DIVIDE_VALUE / Consts.PERCENT_DIVIDE_VALUE;
            hpText.text = $"{front}.{back}k";
        }
    }
}
