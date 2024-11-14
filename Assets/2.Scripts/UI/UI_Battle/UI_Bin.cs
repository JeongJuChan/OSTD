using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Bin : UI_PointerUp
{
    [field: Header("Box Event")]
    public event Action<Box> OnRemoveBox;
    public event Action<bool> OnUpdateScaleUp;

    [Header("Transform")]
    [SerializeField] private Transform parentTrans;
    private Vector3 offsetScale;
    private const float SCALE_UP = 1.5f;

    [SerializeField] private TextMeshProUGUI sellingPriceText;

    #region Unity Pointer Events
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        OnUpdateScaleUp?.Invoke(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        OnUpdateScaleUp?.Invoke(false);
    }

    #endregion

    #region Initialize
    public override void Init()
    {
        base.Init();
        offsetScale = parentTrans.localScale;
        UpdateSellingPriceTextActiveState(false);
        GameManager.instance.OnStart += () => ChangeActiveState(false);
        GameManager.instance.OnReset += () => ChangeActiveState(true);
    }
    #endregion

    #region Update Scale
    public void UpdateScaleUpState(bool isScaleUp)
    {
        parentTrans.localScale = isScaleUp ? parentTrans.localScale * SCALE_UP : offsetScale;
    }
    #endregion

    public void TrySellBox(Box box)
    {
        if (enteredObject == null)
        {
            return;
        }

        OnRemoveBox?.Invoke(box);
    }

    public void OnUpdatePriceText(BigInteger amount)
    {
        UpdateSellingPriceTextActiveState(true);

        BigInteger front = amount / Consts.THOUSAND_DIVIDE_VALUE;
        if (front == 0)
        {
            sellingPriceText.text = amount.ChangeMoney();
        }
        else
        {
            BigInteger back = amount % Consts.THOUSAND_DIVIDE_VALUE / Consts.PERCENT_DIVIDE_VALUE;
            sellingPriceText.text = $"{front}.{back}k";
        }
    }

    public void UpdateSellingPriceTextActiveState(bool isActive)
    {
        sellingPriceText.gameObject.SetActive(isActive);
    }

    private void ChangeActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
