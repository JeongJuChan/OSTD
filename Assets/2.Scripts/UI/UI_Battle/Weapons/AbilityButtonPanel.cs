using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityButtonPanel : MonoBehaviour
{
    [SerializeField] private Image abilityImage;
    [SerializeField] private TextMeshProUGUI weaponTypeText;
    [SerializeField] private TextMeshProUGUI abilityDescriptionText;
    [SerializeField] private UI_Button choosingAbilityButton;

    public void AddChoosingButtonAction(int index, UnityAction closeUI)
    {
        choosingAbilityButton.Init();
        choosingAbilityButton.AddButtonAction(() => WeaponManager.instance.weaponAbilityModule.ChooseWeaponAblility(index));
        choosingAbilityButton.AddButtonAction(closeUI);
    }

    public void UpdateAbilityInfo(Sprite abilitySprite, string weaponTypeStr, string abilityDescriptionStr)
    {
        abilityImage.sprite = abilitySprite;
        weaponTypeText.text = weaponTypeStr;
        abilityDescriptionText.text = abilityDescriptionStr;
    }
}
