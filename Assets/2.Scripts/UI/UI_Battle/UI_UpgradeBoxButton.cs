using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;

public class UI_UpgradeBoxButton : UI_PurchaseButton
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Interact Sprite")]
    [SerializeField] private Sprite disableSprite;
    private Sprite offsetSprite;

    #region Initialize
    public override void Init()
    {
        base.Init();
        offsetSprite = button.image.sprite;
        StageManager.instance.OnClearStage += Reset;
    }
    #endregion

    #region Update UI
    public void UpdateBoxDataUI(BoxData boxData, bool isMaxLevel)
    {
        if (isMaxLevel)
        {
            SetMaxLevelState();
        }
        else
        {
            UpdateCost(boxData.cost);
        }

        UpdateLevelText(boxData.level);
        UpdateHpText(boxData.hp);
    }

    private void Reset()
    {
        UpdateInteractable(true);
        SetDisableColor(Consts.DISABLE_COLOR);
        currencyImageObject.SetActive(true);
        button.image.sprite = offsetSprite;
    }

    private void SetMaxLevelState()
    {
        UpdateInteractable(false);
        currencyImageObject.SetActive(false);
        currencyText.text = "Max";
        UpdateSpriteByLevelMax();
    }

    private void UpdateSpriteByLevelMax()
    {
        SetDisableColor(Color.white);
        UpdateInteractable(false);
        button.image.sprite = disableSprite;
    }

    private void UpdateLevelText(int level)
    {
        levelText.text = $"Lv{level}";
    }

    private void UpdateHpText(BigInteger hp)
    {
        BigInteger frontValue = hp / Consts.THOUSAND_DIVIDE_VALUE;
        BigInteger behindValue = hp % Consts.THOUSAND_DIVIDE_VALUE / Consts.PERCENT_DIVIDE_VALUE;
        hpText.text = behindValue == 0 ? $"{frontValue}K" : $"{frontValue}.{behindValue}K";
    }
    #endregion
}
