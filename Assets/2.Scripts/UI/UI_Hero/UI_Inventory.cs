using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private UI_EquipmentPooler ui_EquipmentPooler;
    [SerializeField] private UI_BluePrintsPanel bluePrintsPanel;

    public event Action<int> OnUpdateNewEquipment;

    private Vector2 cellSize;
    private RectOffset rectOffset;

    private int divideValue;

    private float spacingY;

    [SerializeField] private RectTransform bluePrintPanelRect;

    private float cellSizeUnit;

    [SerializeField] private RectTransform contentRectTransform;

    private EquipmentResourceDataHandler equipmentResourceDataHandler;
    private RankDataHandler rankDataHandler;

    private List<UI_Equipment> ui_equipments = new List<UI_Equipment>();

    private Queue<UI_Equipment> removingQueue = new Queue<UI_Equipment>();

    public override void Init()
    {
        base.Init();
        equipmentResourceDataHandler = ResourceManager.instance.equipment;
        rankDataHandler = ResourceManager.instance.rank;
        cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;
        spacingY = spacing.y;
        cellSizeUnit = cellSize.x + spacing.x;
        divideValue = (int)(rect.rect.width / (cellSize.x + spacing.x));
        rectOffset = gridLayoutGroup.padding;
        bluePrintsPanel.Init();
    }

    public void UpdateUI(List<EquipmentStatData> equipmentStatDatas)
    {
        int equipmentCount = equipmentStatDatas.Count;
        UpdateInventoryUISize(equipmentCount);
        UpdateEquipmentCount(equipmentCount);
        SortUI(equipmentStatDatas);
    }

    public void ShowComparingEquipmentUI(UI_Equipment ui_Equipment)
    {
        int index = ui_equipments.IndexOf(ui_Equipment);
        OnUpdateNewEquipment?.Invoke(index);
    }

    private void UpdateEquipmentCount(int inventoryEquipmentCount)
    {
        int ui_EquipmentCount = ui_equipments.Count;

        if (inventoryEquipmentCount - ui_EquipmentCount > 0)
        {
            for (int i = 0; i < inventoryEquipmentCount - ui_EquipmentCount; i++)
            {
                UI_Equipment ui_Equipment = ui_EquipmentPooler.GetUI();
                ui_equipments.Add(ui_Equipment);
            }
        }
        else
        {
            int removeCount = ui_EquipmentCount - inventoryEquipmentCount;
            int startIndex = ui_EquipmentCount - removeCount;
            for (int i = ui_equipments.Count - 1; i >= ui_equipments.Count - removeCount; i--)
            {
                ui_equipments[i].ReturnToPool();
            }

            ui_equipments.RemoveRange(startIndex, removeCount);
        }
    }

    private void UpdateInventoryUISize(int equipmentCount)
    {
        if (equipmentCount == 0)
        {
            Vector2 offsetSize = contentRectTransform.sizeDelta;
            offsetSize.y = bluePrintPanelRect.sizeDelta.y + rectOffset.vertical;
            contentRectTransform.sizeDelta = offsetSize;
            return;
        }

        int count = equipmentCount / divideValue;
        count += equipmentCount % divideValue == 0 ? 0 : 1;

        float bluePrintHeight = count * cellSizeUnit + rectOffset.vertical - spacingY;
        float height = bluePrintHeight + bluePrintPanelRect.sizeDelta.y;
        Vector2 size = contentRectTransform.sizeDelta;
        size.y = height;
        contentRectTransform.sizeDelta = size;
        bluePrintsPanel.SetPos(bluePrintHeight);
    }

    private void SortUI(List<EquipmentStatData> equipmentStatDatas)
    {
        EquipmentType equipmentType = EquipmentType.None;
        Rank rank = Rank.None;
        Sprite equipmentSprite = null;
        Sprite rankSprite = null;
        for (int i = 0; i < ui_equipments.Count; i++)
        {
            UI_Equipment ui_Equipment = ui_equipments[i];
            EquipmentStatData equipmentStatData = equipmentStatDatas[i];
            if (equipmentType != equipmentStatData.equipmentType || rank != equipmentStatData.rank)
            {
                equipmentSprite = equipmentResourceDataHandler.GetEquipmentSprite(equipmentStatData.equipmentType, equipmentStatData.rank);
                rankSprite = rankDataHandler.GetRankBackgroundSprite(equipmentStatData.rank);
            }

            ui_Equipment.UpdateUI(equipmentSprite, rankSprite);
        }
    }

    public UI_EquipmentButton GetUIEquipmentButtonByIndex(int index)
    {
        if (ui_equipments.Count > index)
        {
            return ui_equipments[index] as UI_EquipmentButton;
        }

        return null;
    }
}
