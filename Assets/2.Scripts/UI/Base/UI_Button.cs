using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_Button : UI_Base
{
    protected Button button;

    #region Initialize
    public override void Init()
    {
        base.Init();
        button = GetComponent<Button>();
    }
    #endregion

    #region Button Events
    public virtual void UpdateInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }

    public void AddButtonAction(UnityAction action)
    {
        button.onClick.AddListener(action);
    }

    public void RemoveAction(UnityAction action)
    {
        button.onClick.RemoveListener(action);
    }

    public void RemoveAllActions()
    {
        button.onClick.RemoveAllListeners();
    }

    public virtual void SetDisableColor(Color32 disableColor)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.disabledColor = disableColor;
        button.colors = colorBlock;
    }

    #endregion
}
