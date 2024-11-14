using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class NotificationManager : MonoBehaviourSingleton<NotificationManager>
{
    public event Action OnNotificationChanged;
    public Dictionary<string, bool> notifications = new Dictionary<string, bool>();

    /// <summary>
    /// 주어진 키에 대한 알림 상태를 설정하고 업데이트를 트리거합니다.
    /// </summary>
    /// <param name="key">알림 키입니다.</param>
    /// <param name="state">설정할 상태입니다. true는 활성, false는 비활성입니다.</param>
    public void SetNotification(string key, bool state)
    {
        if (notifications.ContainsKey(key))
        {
            notifications[key] = state;
        }
        else
        {
            notifications.Add(key, state);
        }
        OnNotificationChanged?.Invoke();
    }

    public void SetNotification(RedDotIDType redDotIDType, bool state)
    {
        string redDotID = redDotIDType.ToString();
        if (notifications.ContainsKey(redDotID))
        {
            notifications[redDotID] = state;
        }
        else
        {
            notifications.Add(redDotID, state);
        }
        OnNotificationChanged?.Invoke();
    }

    /// <summary>
    /// 주어진 키에 대한 알림 상태를 검색합니다.
    /// </summary>
    /// <param name="key">알림 키입니다.</param>
    /// <returns>알림의 현재 상태입니다.</returns>
    public bool GetNotificationState(string key)
    {
        if (notifications.ContainsKey(key))
        {
            return notifications[key];
        }
        return false;
    }

    string redDotID;
    public bool GetNotificationState(RedDotIDType redDotIDType)
    {
        this.redDotID = redDotIDType.ToString();
        if (notifications.ContainsKey(redDotID))
        {
            return notifications[redDotID];
        }
        return false;
    }
}
