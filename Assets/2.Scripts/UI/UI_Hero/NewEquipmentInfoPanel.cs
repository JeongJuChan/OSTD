using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

public class NewEquipmentInfoPanel : ComparingEquipmentInfoPanel
{
    [SerializeField] private Image arrowImage;

    public void UpdateComparingNewEquipmentUI(Sprite equipmentSprite, Sprite rankSprite, string equipmentStr, string rankStr, Color titleColor, 
        BigInteger stat, Color arrowColor, Sprite arrowSprite)
    {
        UpdateEquipmentInfos(equipmentSprite, rankSprite, equipmentStr, rankStr, titleColor, stat);
        UpdateInfoChanged(arrowColor, arrowSprite);
    }

    private void UpdateInfoChanged(Color color, Sprite arrowSprite)
    {
        currentStatText.color = color;
        arrowImage.sprite = arrowSprite;
        if (arrowSprite != null)
        {
            arrowImage.color = color;
            arrowImage.gameObject.SetActive(true);
        }
        else
        {
            arrowImage.gameObject.SetActive(false);
        }
    }
}
