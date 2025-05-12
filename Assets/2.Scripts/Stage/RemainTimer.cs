using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemainTimer : MonoBehaviour
{
    [SerializeField] private Slider remainTimeSlider;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private TextMeshProUGUI progressText;

    private int offsetRemainSecond;

    public event Func<int> OnGetRemainSecond;

    public void Init()
    {
        ActivateSelf(false);
    }

    public void UpdateTime(string timeText)
    {
        this.timeText.text = timeText;
        UpdateRemainTimeSlider();
    }

    public void UpdateRemainTimeSlider()
    {
        float ratio = 1 - (float)OnGetRemainSecond.Invoke() / offsetRemainSecond;
        remainTimeSlider.value = ratio;
    }

    public void ActivateSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetOffsetRemainTime(int second)
    {
        offsetRemainSecond = second;
    }

    public void ActivateSlider(bool isActive)
    {
        remainTimeSlider.gameObject.SetActive(isActive);
    }

    public void ActivateTimerText(bool isActive)
    {
        timeText.gameObject.SetActive(isActive);
    }
}
