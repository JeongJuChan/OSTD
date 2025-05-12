using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnergyButton : UI_Button
{
    [SerializeField] private Image[] buttonImages;
    private Color32 disableColor;


    public override void SetDisableColor(Color32 disableColor)
    {
        this.disableColor = disableColor;
    }

    public override void UpdateInteractable(bool isInteractable)
    {
        base.UpdateInteractable(isInteractable);
        foreach (Image image in buttonImages)
        {
            image.color = isInteractable ? Color.white : disableColor;
        }
    }
}
