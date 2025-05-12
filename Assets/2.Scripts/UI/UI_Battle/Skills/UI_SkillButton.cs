using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillButton : UI_Button
{
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image skillImage;
    [SerializeField] private GameObject parentObject;

    public override void Init()
    {
        base.Init();
        SetDisableColor(Consts.DISABLE_COLOR);
    }

    public void UpdateCostText(int cost)
    {
        costText.text = cost.ToString();
    }

    public void UpdateSprite(Sprite sprite)
    {
        skillImage.sprite = sprite;
    }

    public override void OpenUI()
    {
        parentObject.SetActive(true);
    }

    public override void CloseUI()
    {
        parentObject.SetActive(false);
    }

    public override void UpdateInteractable(bool isInteractable)
    {
        base.UpdateInteractable(isInteractable);
        skillImage.color = isInteractable ? Color.white : Consts.DISABLE_COLOR;
    }
}
