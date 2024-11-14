using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeWeaponButtonPanel : UI_PurchaseButton
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject levelProgressObject;
    [SerializeField] private Image[] progressImages;
    [SerializeField] private GameObject damageTitleObject;
    [SerializeField] private TextMeshProUGUI damageText;

    [SerializeField] private GameObject evolutionTextObject;
    [SerializeField] private TextMeshProUGUI evolutionLevelText;

    public override void Init()
    {
        base.Init();
        SetDisableColor(Consts.DISABLE_COLOR);
    }

    public void UpdateActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void Reset()
    {
        UpdateActiveState(false);
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Lv.{level}";
    }

    public void UpgradeBlockCount(int upgradeBlockCount)
    {
        for (int i = 0; i < progressImages.Length; i++)
        {
            progressImages[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < upgradeBlockCount; i++)
        {
            progressImages[i].gameObject.SetActive(true);
        }
    }

    public void UpdateDamage(BigInteger damage)
    {
        damageText.text = damage.ChangeMoney();
    }

    public void UpdateEvolutionActiveState(bool isActive)
    {
        evolutionTextObject.SetActive(isActive);
        evolutionLevelText.gameObject.SetActive(isActive);
        levelText.gameObject.SetActive(!isActive);
        levelProgressObject.SetActive(!isActive);
        damageTitleObject.SetActive(!isActive);
        damageText.gameObject.SetActive(!isActive);
    }

    public void UpdateEvolutionLevelText(int level)
    {
        evolutionLevelText.text = $"Lv{level} > Lv{level + 1}";
    }
}
