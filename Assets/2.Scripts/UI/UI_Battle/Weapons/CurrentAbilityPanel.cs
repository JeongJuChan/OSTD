using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentAbilityPanel : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private AbilitySlotPanel abilitySlotPanel;

    public void UpdateCurrentHavingAbilityPanel(Sprite weaponIcon, List<Sprite> abilitySprites)
    {
        weaponImage.sprite = weaponIcon;
        abilitySlotPanel.SetSprites(abilitySprites);
    }
}
