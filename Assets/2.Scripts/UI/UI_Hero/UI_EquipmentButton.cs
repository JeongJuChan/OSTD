using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_EquipmentButton : UI_Equipment
{
    [SerializeField] private Button showInfoButton;
    [field: SerializeField] public RedDotController redDotController { get; private set; }

    public void AddButtonAction(UnityAction action)
    {
        showInfoButton.onClick.AddListener(action);
    }

    public void RemoveButtonAction(UnityAction action)
    {
        showInfoButton.onClick.RemoveListener(action);
    }
}
