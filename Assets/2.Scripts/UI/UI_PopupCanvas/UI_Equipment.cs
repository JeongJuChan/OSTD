using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Equipment : MonoBehaviour, IPoolable<UI_Equipment>
{
    private Action<UI_Equipment> OnReturnAction;

    [SerializeField] private Image equipmentImage;
    [SerializeField] private Image rankImage;

    public void Initialize(Action<UI_Equipment> returnAction)
    {
        OnReturnAction = returnAction;
    }

    public void ReturnToPool()
    {
        OnReturnAction?.Invoke(this);
    }

    public void UpdateUI(Sprite equipmentSprite, Sprite rankSprite)
    {
        equipmentImage.sprite = equipmentSprite;
        rankImage.sprite = rankSprite;
    }
}
