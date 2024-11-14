using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentEquipmentTitlePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI equipmentTypeText;

    public void UpdateTitleText(int level, EquipmentType equipmentType)
    {
        levelText.text = $"Lv.{level}";
        equipmentTypeText.text = $"{EnumUtility.GetEquipmentTypeKR(equipmentType)}슬롯";
    }
}
