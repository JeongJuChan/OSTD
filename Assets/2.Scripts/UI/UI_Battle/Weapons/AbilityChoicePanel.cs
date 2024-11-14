using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityChoicePanel : MonoBehaviour
{
    [SerializeField] private AbilityButtonPanel[] abilityButtonPanels;

    public void Init(UnityAction closeUI)
    {
        for (int i = 0; i < abilityButtonPanels.Length; i++)
        {
            abilityButtonPanels[i].AddChoosingButtonAction(i, closeUI);
        } 
    }

    public void UpdateAbilityButtonPanel(int index, Sprite abilitySprite, string weaponTypeStr, string abilityDescription)
    {
        abilityButtonPanels[index].UpdateAbilityInfo(abilitySprite, weaponTypeStr, abilityDescription);
    }
}
