using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGameEnergyPanel : UI_Base
{
    [SerializeField] private TextMeshProUGUI energyAmountText;
    [SerializeField] private Slider energySlider;


    public void UpdateEnergy(int amount)
    {
        energyAmountText.text = amount.ToString();
    }

    public void UpdateEnergySlider(float ratio)
    {
        energySlider.value = ratio;
    }
}
