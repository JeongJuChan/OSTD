using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Popup : UI_Base
{
    protected Action<bool> OnChangePopupState;

    public virtual void Initialize()
    {
        OnChangePopupState += UIManager.instance.ChangeIsPopupOpened;
    }

    public override void OpenUI()
    {
        base.OpenUI();
        OnChangePopupState?.Invoke(true);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        OnChangePopupState?.Invoke(false);
    }
}
