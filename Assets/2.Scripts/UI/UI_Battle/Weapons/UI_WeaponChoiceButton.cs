using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponChoiceButton : UI_PurchaseButton
{
    [SerializeField] private Image weaponImage;

    public override void Init()
    {
        base.Init();
        SetDisableColor(Consts.DISABLE_COLOR);
    }

    public void UpdateWeaponSprite(Sprite sprite)
    {
        weaponImage.sprite = sprite;
    }
}
