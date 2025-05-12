using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInfoPanel : MonoBehaviour
{
    [SerializeField] private Image rankImage;
    [SerializeField] private Image equipmentTypeImage;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image statTypeImage;
    [SerializeField] private TextMeshProUGUI statTypeText;
    [SerializeField] private TextMeshProUGUI currentStatText;
    [SerializeField] private TextMeshProUGUI nextStatText;

    [SerializeField] private TextMeshProUGUI descriptionText;

    public void UpdateEquipmentInfos(Sprite equipmentSprite, Sprite rankSprite, string equipmentStr, string rankStr, Sprite statTypeSprite, 
        string statTypeStr, BigInteger currentStat, BigInteger nextStat, string description)
    {
        UpdateEquipmentSprite(equipmentSprite, rankSprite);
        UpdateEquipmentNameInfo(equipmentStr, rankStr);
        UpdateEquipmentStatInfo(statTypeSprite, statTypeStr, currentStat, nextStat);
        UpdateDescription(description);
    }

    private void UpdateEquipmentSprite(Sprite equipmentSprite, Sprite rankSprite)
    {
        equipmentTypeImage.sprite = equipmentSprite;
        rankImage.sprite = rankSprite;
    }

    private void UpdateEquipmentNameInfo(string equipmentStr, string rankStr)
    {
        nameText.text = $"{rankStr} {equipmentStr}";
    }

    private void UpdateEquipmentStatInfo(Sprite statTypeSprite, string statTypeStr, BigInteger currentStat, BigInteger nextStat)
    {
        statTypeImage.sprite = statTypeSprite;
        statTypeText.text = statTypeStr;
        currentStatText.text = currentStat.ChangeMoney();
        nextStatText.text = nextStat != null ? nextStat.ChangeMoney() : "시트 필요";
    }

    private void UpdateDescription(string description)
    {
        descriptionText.text = description;
    }
}
