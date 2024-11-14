using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dragable : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Transform")]
    [SerializeField] protected Transform trans;
    private Vector3 offsetScale;
    private const float DRAG_SCALE = 2f;

    [field: Header("OnDrop")]
    public event Action OnDragStart;
    public event Action OnDragEnd;
    public event Action OnDrag;
    protected Vector2 offsetPos;

    [Header("UI_Bin")]
    private UI_Bin ui_Bin;

    #region Unity Mouse Events
    private void OnMouseDown()
    {
        DragStart();
    }

    private void OnMouseDrag() 
    {
        UpdatePos();
        Drag();
    }

    private void OnMouseUp() 
    {
        DragEnd();
    }
    #endregion

    #region Initialize
    public virtual void Init()
    {
        ui_Bin = UIManager.instance.GetUIElement<UI_Bin>();
    }
    protected abstract void ResetOffsetPos();
    protected void InitOffsetScale()
    {
        offsetScale = trans.localScale;
    }
    #endregion

    #region Drag Events
    protected virtual void DragStart()
    {
        UpdatePos();
        UpdateScale(true);
        ui_Bin.UpdateScaleUpState(true);
        OnDragStart?.Invoke();
        InputManager.instance.SetIsDragging(true);
    }

    protected virtual void Drag()
    {
        OnDrag?.Invoke();
    }

    protected virtual void DragEnd()
    {
        ResetPos();
        UpdateScale(false);
        ui_Bin.UpdateScaleUpState(false);
        OnDragEnd?.Invoke();
        InputManager.instance.SetIsDragging(false);
    }
    #endregion

    #region Update States
    private void UpdatePos()
    {
        Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        inputPos.z = 0;
        trans.position = inputPos;
    }

    private void ResetPos()
    {
        trans.localPosition = offsetPos;
    }

    private void UpdateScale(bool isScaleUp)
    {
        trans.localScale = isScaleUp ? offsetScale * DRAG_SCALE : offsetScale;
    }

    private void UpdateTransparent(bool isTransparent)
    {
        spriteRenderer.color = isTransparent ? Consts.HALF_TRANSPARENT_COLOR : Color.white;
    }
    #endregion
}
