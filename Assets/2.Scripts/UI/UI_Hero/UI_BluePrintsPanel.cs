using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BluePrintsPanel : UI_Base
{
    [SerializeField] private BluePrintElement[] bluePrintElements;
    [SerializeField] private ScrollRect inventoryView;

    private float offsetPosY; 

    public override void Init()
    {
        base.Init();
        inventoryView.onValueChanged.AddListener(UpdatePosY);
        foreach (BluePrintElement bluePrintElement in bluePrintElements)
        {
            bluePrintElement.Init();
        }
    }

    public void SetPos(float bluePrintHeight)
    {
        Vector2 vec = rect.anchoredPosition;
        vec.y = -bluePrintHeight;
        rect.anchoredPosition = vec;
        offsetPosY = vec.y;
    }

    private void UpdatePosY(Vector2 deltaVec)
    {
        Vector2 anchoredPos = rect.anchoredPosition;
        anchoredPos.y = offsetPosY * deltaVec.y; ;
        rect.anchoredPosition = anchoredPos;
    }
}
