using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComparingEquipmentInfoPanel : MonoBehaviour
{
    [SerializeField] private Image rankImage;
    [SerializeField] private Image equipmentTypeImage;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image statTypeImage;
    [SerializeField] private TextMeshProUGUI statTypeText;
    [SerializeField] protected TextMeshProUGUI currentStatText;

    public void UpdateEquipmentInfos(Sprite equipmentSprite, Sprite rankSprite, string equipmentStr, string rankStr, Color titleColor, 
        BigInteger currentStat)
    {
        UpdateEquipmentSprite(equipmentSprite, rankSprite);
        UpdateEquipmentNameInfo(equipmentStr, rankStr, titleColor);
        UpdateEquipmentStat(currentStat);
    }

    public void UpdateEquipmentStatType(Sprite statTypeSprite, string statTypeStr)
    {
        statTypeImage.sprite = statTypeSprite;
        statTypeText.text = statTypeStr;
    }

    private void UpdateEquipmentSprite(Sprite equipmentSprite, Sprite rankSprite)
    {
        equipmentTypeImage.sprite = equipmentSprite;
        rankImage.sprite = rankSprite;
    }

    private void UpdateEquipmentNameInfo(string equipmentStr, string rankStr, Color color)
    {
        nameText.text = $"{rankStr} {equipmentStr}";
        nameText.color = color;
    }

    private void UpdateEquipmentStat(BigInteger stat)
    {
        currentStatText.text = stat.ChangeMoney();
    }
}
