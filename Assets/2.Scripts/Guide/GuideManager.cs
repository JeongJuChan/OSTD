using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : Singleton<GuideManager>
{
    private Dictionary<string, Action<bool>> guideToggleDict = new Dictionary<string, Action<bool>>();

    [field: Header("TutorialPanelWithBackground")]
    public Dictionary<string, RectTransform> tutorialDict { get; private set; } = new Dictionary<string, RectTransform>();
    public Dictionary<string, GameObject> guidesDict { get; private set; } = new Dictionary<string, GameObject>();

    private Queue<string> readiedKeyQueue = new Queue<string>();

    public bool isPopupShowed { get; private set; }

    public void AddGuidDict(string guideKey, Action<bool> OnChangeActiveState)
    {
        if (!guideToggleDict.ContainsKey(guideKey))
        {
            guideToggleDict.Add(guideKey, null);
        }

        guideToggleDict[guideKey] += OnChangeActiveState;
    }

    // 팝업 띄우는 거 먼저 호출
    public void ToggleGuide(string guideKey, bool isActive)
    {
        if (guideToggleDict.ContainsKey(guideKey))
        {
            guideToggleDict[guideKey]?.Invoke(isActive);
        }
    }

    public bool GuideContainsKey(string guideKey)
    {
        return guideToggleDict.ContainsKey(guideKey);
    }

    public void ToggleGuideWithBackgroundPanel(string guideKey, bool isActive)
    {
        Debug.Log($"{guideKey} {isActive}");

        if (guideToggleDict.ContainsKey(guideKey))
        {
            if (isPopupShowed)
            {
                if (isActive)
                {
                    if (!readiedKeyQueue.Contains(guideKey))
                    {
                        readiedKeyQueue.Enqueue(guideKey);
                    }
                }
                else if (readiedKeyQueue.Count > 0)
                {
                    guideToggleDict[guideKey]?.Invoke(isActive);
                    guideToggleDict[readiedKeyQueue.Dequeue()]?.Invoke(true);
                    isPopupShowed = true;
                    return;
                }
                else
                {
                    guideToggleDict[guideKey]?.Invoke(isActive);
                }

            }
            else
            {
                guideToggleDict[guideKey]?.Invoke(isActive);
            }

            isPopupShowed = isActive;
        }
    }

    protected IEnumerator Wait(Action<bool> action)
    {
        yield return null;
        action?.Invoke(true);
    }
}
