using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PowerAlertSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private Image[] lines;
    [SerializeField] private Image icon;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private float waitTime;
    [SerializeField] private float fadeTime;
    [SerializeField] private float height;

    private Coroutine waitCoroutine;
    private Coroutine fadeCoroutine;

    private bool isFadeStarted;

    public event Action<PowerAlertSlot> OnAnimEnd;

    public void SetMessage(string message, Color color, Sprite sprite)
    {
        powerText.text = message;
        powerText.color = color;
        icon.color = color;
        icon.sprite = sprite;
        foreach (Image line in lines)
        {
            line.color = color;
        }
    }

    private void OnEnable()
    {
        ResetSlot();
        waitCoroutine = StartCoroutine(WaitForFade());
    }

    private void ResetSlot()
    {
        group.alpha = 1;
        transform.localPosition = Vector3.zero;
    }

    public void QuickAnim()
    {
        if (isFadeStarted) return;

        StopCoroutine(waitCoroutine);
        StartFade();
    }

    private IEnumerator WaitForFade()
    {
        float currentTime = 0;

        while (currentTime < waitTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        StartFade();
    }

    private void StartFade()
    {
        isFadeStarted = false;

        fadeCoroutine = StartCoroutine(FadeAnim());
    }

    private IEnumerator FadeAnim()
    {
        float currentTime = 0;

        while (currentTime < waitTime)
        {
            float progress = currentTime / fadeTime;
            group.alpha = 1 - progress;
            float yPos = progress * waitTime;
            transform.localPosition = new Vector3(0, yPos, 0);
            currentTime += Time.deltaTime;
            yield return null;
        }
        AnimEnd();
    }

    private void AnimEnd()
    {
        waitCoroutine = null;
        fadeCoroutine = null;
        isFadeStarted = false;

        OnAnimEnd?.Invoke(this);
    }
}
