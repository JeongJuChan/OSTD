using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DragableBox : Dragable
{
    [Header("Highlight Object")]
    [SerializeField] private GameObject highlightObject;
    [SerializeField] private GameObject lastPosObject;

    [SerializeField] private SortingGroup sortingGroup;

    [SerializeField] private Rigidbody2D dragRigid;

    public IDamageable damagable { get; private set; }

    public event Action OnUpdateSprite;

    #region Initialize
    public override void Init()
    {
        base.Init();
        InitOffsetScale();
        highlightObject.SetActive(false);
    }
    #endregion

    #region Reset
    public void Reset()
    {
        ResetOffsetPos();
        gameObject.SetActive(true);

        if (dragRigid == null)
        {
            dragRigid = gameObject.AddComponent<Rigidbody2D>();
        }
        dragRigid.isKinematic = true;
        dragRigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void InGameSetting()
    {
        Destroy(dragRigid);
    }

    public void SetDamageable(IDamageable damagable)
    {
        this.damagable = damagable;
    }

    protected override void ResetOffsetPos()
    {
        offsetPos = trans.localPosition;
    }
    #endregion

    #region Drag Events
    protected override void DragStart()
    {
        base.DragStart();
        UpdateDragObjectsActiveState(true);
        UpdateOrderByDragState(true);
    }

    protected override void DragEnd()
    {
        base.DragEnd();
        UpdateDragObjectsActiveState(false);
        UpdateOrderByDragState(false);
    }

    private void UpdateDragObjectsActiveState(bool isActive)
    {
        lastPosObject.SetActive(isActive);
        highlightObject.SetActive(isActive);
    }

    private void UpdateOrderByDragState(bool isDragging)
    {
        sortingGroup.sortingLayerName = isDragging ? Consts.POPUP_UI_LAYER : Consts.PLAYER_LAYER;
    }
    #endregion

    public void UpdateSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        OnUpdateSprite?.Invoke();
    }
}
