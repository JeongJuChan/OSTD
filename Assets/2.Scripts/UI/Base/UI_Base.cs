using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    public event Action<UI_Base> OnOpenUI;
    public event Action<UI_Base> OnCloseUI;

    public bool notDestoryedOnLoad;
    
    public RectTransform rect { get; private set; }

    public virtual void Init()
    {
        rect = GetComponent<RectTransform>();
    }

    protected virtual void InitCamera()
    {
        if (TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }
    }

    public virtual void OpenUI()
    {
        OnOpenUI?.Invoke(this);

        gameObject.SetActive(true);
    }

    public virtual void CloseUI()
    {
        OnCloseUI?.Invoke(this);

        gameObject.SetActive(false);
    }
}
