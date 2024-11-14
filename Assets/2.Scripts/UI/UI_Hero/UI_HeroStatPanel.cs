using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;

public class UI_HeroStatPanel : UI_Base
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI healthText;

    public override void Init()
    {
        base.Init();
        EquipmentManager.instance.OnUpdateHeroStatUI += UpdateHeroStatUI;
    }

    private void UpdateHeroStatUI(BigInteger damage, BigInteger health)
    {
        damageText.text = damage.ChangeMoney();
        healthText.text = health.ChangeMoney();
    }
}
