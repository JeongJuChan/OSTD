using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UI_Skill : UI_Base
{
    [SerializeField] private UI_Button skillButton;

    public override void Init()
    {
        skillButton.Init();
        skillButton.SetDisableColor(Consts.DISABLE_COLOR);
    }

    public void SetSkillAction(UnityAction skillAction)
    {
        skillButton.RemoveAllActions();
        skillButton.AddButtonAction(skillAction);
    }

    public void UpdateSkillButtonInteractableState(bool isInteractable)
    {
        skillButton.UpdateInteractable(isInteractable);
    }
}
