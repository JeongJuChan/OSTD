using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIPanel : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;

    public void UpdateLoadingBar(float ratio)
    {
        loadingSlider.value = ratio;
        loadingText.text = $"로딩 중 : {ratio * Consts.PERCENT_DIVIDE_VALUE: 0.00}%";
    }
}
