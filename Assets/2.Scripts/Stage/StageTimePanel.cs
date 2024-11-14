using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageTimePanel : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TextMeshProUGUI timeText;

    public event Action OnTimeOver;

    private int minutes;
    private int seconds;

    private Coroutine preCoroutine;

    private WaitForSeconds waitForSeconds;

    private const int SECOND_UNIT = 1;

    private float offsetTime;

    private void Awake()
    {
        waitForSeconds = CoroutineUtility.GetWaitForSeconds(SECOND_UNIT);
        ActiveSelf(false);
    }

    public void StartTimer(int time)
    {
        StopTimer();

        ActiveSelf(true);

        minutes = time / Consts.MAX_MINUTE;
        seconds = time % Consts.MAX_MINUTE;

        offsetTime = time;

        preCoroutine = StartCoroutine(CoStartTimer());
    }

    public void StopTimer()
    {
        if (preCoroutine != null)
        {
            StopCoroutine(preCoroutine);
        }

        if (gameObject.activeInHierarchy)
        {
            ActiveSelf(false);
        }
    }

    private IEnumerator CoStartTimer()
    {
        bool isEnded = minutes == 0 && seconds == 0;

        while (!isEnded)
        {
            int currentTime = minutes * Consts.MAX_SECOND + seconds;
            UpdateTimeText();
            UpdateTimeSlider(currentTime);
            yield return waitForSeconds;
            seconds--;
            if (seconds < 0 && minutes > 0)
            {
                minutes--;
                seconds += Consts.MAX_SECOND;
            }

            isEnded = minutes == 0 && seconds == 0;
        }

        OnTimeOver?.Invoke();
        ActiveSelf(false);
    }

    private void UpdateTimeSlider(int currentTime)
    {
        timeSlider.value = currentTime / offsetTime;
    }

    private void UpdateTimeText()
    {
        string minutesStr = minutes / 10 > 0 ? minutes.ToString() : $"0{minutes}";
        string secondsStr = seconds / 10 > 0 ? seconds.ToString() : $"0{seconds}";
        timeText.text = $"{minutesStr}:{secondsStr}";
    }

    private void ActiveSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
