using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;
using System;
using System.Collections.Generic;

public class PushNotificationManager : MonoBehaviourSingleton<PushNotificationManager>
{
    Dictionary<string, PushNotesDataSO> dataDic;
    Dictionary<string, bool> rewardRecieved;

    public event Action OnApplicationPauseEvent;

    public int hasPermissionChecked
    {
        get { return PlayerPrefs.GetInt("HasPermissionChecked_" + Application.productName, 0); }
        set { PlayerPrefs.SetInt("HasPermissionChecked_" + Application.productName, value); }
    }
#if UNITY_EDITOR
    public bool GetIsPlayerStartInMainScene()
    {
        return dataDic == null;
    }
#endif 

#if UNITY_EDITOR || UNITY_ANDROID
    public void StartInit()
    {
        DontDestroyOnLoad(gameObject);
        Initialize();
    }


    private void Initialize()
    {
        SetCollections();

        // 최초 한 번만 권한 체크
        if (hasPermissionChecked == 0)
        {
            CheckNotificationPermission();
            hasPermissionChecked = 1;
        }

        // 게임시작 모든 알람 지우기
        AndroidNotificationCenter.CancelAllNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        LoadDatas();
    }

    private void SetCollections()
    {
        dataDic = new Dictionary<string, PushNotesDataSO>();
        rewardRecieved = new Dictionary<string, bool>();
    }

    private void LoadDatas()
    {
        PushNotesDataSO[] datas = Resources.LoadAll<PushNotesDataSO>("ScriptableObjects/PushNoteDataSO");

        foreach (PushNotesDataSO data in datas)
        {
            dataDic[data.name] = data;
        }

        LoadRewardRecieved();
    }

    private void LoadRewardRecieved()
    {
        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            rewardRecieved[kvp.Key] = ES3.KeyExists($"PushRewardRecieved_{kvp.Key}") ? ES3.Load<bool>($"PushRewardRecieved_{kvp.Key}") : false;
        }
    }

    private void CheckNotificationPermission()
    {
        Debug.Log($"Permission.HasUserAuthorizedPermission(\"android.permission.POST_NOTIFICATIONS\") {Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS")}");
        // 푸시 알림 권한이 허용되어 있는지 확인
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            // 권한이 허용되지 않은 경우 권한 요청
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // 알림 예약
            if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                OnApplicationPauseEvent?.Invoke();
                SendLocalNotification();
            }

            SaveRewardRecieved();
        }
        else
        {
            // 알림 예약 제거
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.CancelAllScheduledNotifications();
        }
    }

    public void SendLocalNotification(string title, string desc, int minutes)
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Channel",
            Importance = Importance.Default,
            Description = "Description",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        AndroidNotificationCenter.SendNotification(new AndroidNotification(title, desc, DateTime.Now.AddMinutes(minutes)), "channel_id");
    }

    private void SendLocalNotification()
    {
        // Android에서만 사용되는 푸시 채널 설정
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Channel",
            Importance = Importance.Default,
            Description = "Description",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            if (rewardRecieved[kvp.Key]) continue;

            Debug.Log($"Push: {kvp.Key}");

            AndroidNotificationCenter.SendNotification(
            new AndroidNotification(kvp.Value.Title, kvp.Value.Desc, DateTime.Now.AddHours(kvp.Value.PushTime)), "channel_id"); // AddHours(1) : 1시간 후 알림
        }
    }
#endif

    public List<PushNotesDataSO> GetUnrecievedRewardDatas(int hour)
    {
        List<PushNotesDataSO> pushDatas = new List<PushNotesDataSO>();

        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            if (!rewardRecieved[kvp.Key] && kvp.Value.PushTime <= hour)
            {
                pushDatas.Add(kvp.Value);
            }
        }

        return pushDatas;
    }

    public void SetRewardRecieved(string dataName)
    {
        rewardRecieved[dataName] = true;
    }

    private void SaveRewardRecieved()
    {
        foreach (KeyValuePair<string, bool> kvp in rewardRecieved)
        {
            ES3.Save($"PushRewardRecieved_{kvp.Key}", kvp.Value);
        }
    }
}
