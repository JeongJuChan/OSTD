using System;
using System.Collections.Generic;
using UnityEngine;
public class AdsManager : MonoBehaviourSingleton<AdsManager>
{
    private Action<MaxSdk.Reward, MaxSdkBase.AdInfo> pendingRewardAction;
    private string adUnitKey;
    private Dictionary<string, int> adsShownToday;
    private DateTime lastAdShownDate;
    //private string MaxSdkKey = "QqxxU259hFqPcXv2vIq4mtdJVaJ7Dt7G3WBsXWONa76yR9urWDB9M55t5o7ZlHG602Od1bEFcTGNOmxNxsEv1L";
    // #if UNITY_ANDROID && UNITY_EDITOR
    private string adUnitId = "037b40315c1a7eba";
    // #endif
    int retryAttempt;
    private DateTime interstitialStartTime;

    private int offsetInterstitialMinute = 3;
    private int interstitialMinuteAfterReward = 6;
    private int totalSeconds;

    public void Initialize()
    {
        if (DataBaseManager.instance.ContainsKey(Consts.INTERSTITIAL_TIME))
        {
            string interstitialTime = DataBaseManager.instance.Load(Consts.INTERSTITIAL_TIME, "0");
            interstitialStartTime = DateTime.Parse(interstitialTime);
        }


        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            InitializeInterstitialAds();
            InitializeRewardedAds();
        };
            //MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();

        adsShownToday = ES3.Load<Dictionary<string, int>>(Consts.ADS_SHOWN_TODAY_SAVE, new Dictionary<string, int>());
        lastAdShownDate = ES3.Load<DateTime>(Consts.LAST_AD_SHOW_DATE_SAVE, DateTime.Today);
        UpdateAdTracking();
    }
    #region AppLovinMax Mathrd
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += HandleReward;
        // Load the first rewarded ad

        LoadRewardedAd();
    }

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(adUnitId);
    }
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        // Reset retry attempt
        retryAttempt = 0;
    }
    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);
    }
    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }
    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }
    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
    }
    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
    #endregion
    public void ShowRewardedAdByTime(string adUnitKey, Action<MaxSdk.Reward, MaxSdkBase.AdInfo> rewardAction)
    {
        adsShownToday[adUnitKey] = 0;
        UpdateAdTracking();
        this.adUnitKey = adUnitKey;
        pendingRewardAction = rewardAction;
        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
            MaxSdk.ShowRewardedAd(adUnitId);
        }
        else
        {
            Debug.Log("Ad not ready, loading ad...");
            LoadRewardedAd();
        }
    }

    public void ShowRewardedAdByDay(string adUnitKey, Action<MaxSdk.Reward, MaxSdkBase.AdInfo> rewardAction)
    {
        UpdateAdTracking();
        if (!adsShownToday.ContainsKey(adUnitKey))
        {
            adsShownToday[adUnitKey] = 0;
        }
        if (adsShownToday[adUnitKey] < 3)
        {
            this.adUnitKey = adUnitKey;
            pendingRewardAction = rewardAction;
            if (MaxSdk.IsRewardedAdReady(adUnitId))
            {
                MaxSdk.ShowRewardedAd(adUnitId);
            }
            else
            {
                Debug.Log("Ad not ready, loading ad...");
                LoadRewardedAd();
            }
        }
        else
        {
            Debug.Log("Daily limit for this ad reached. No more views today.");
        }
    }

    public int GetAdCount(string adType)
    {
        if (adsShownToday.ContainsKey(adType))
        {
            return adsShownToday[adType];
        }
        return 0;
    }
    private void HandleReward(string adUnitKey, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        if (pendingRewardAction != null)
        {
            DataBaseManager.instance.Save(Consts.IS_RECENTLY_REWARDED, true);
            IncrementAdCount(this.adUnitKey);
            pendingRewardAction.Invoke(reward, adInfo);
            pendingRewardAction = null;
            this.adUnitKey = null;
        }
    }
    private void IncrementAdCount(string adUnitKey)
    {
        adsShownToday[adUnitKey]++;
        lastAdShownDate = DateTime.Today;
        ES3.Save<Dictionary<string, int>>(Consts.ADS_SHOWN_TODAY_SAVE, adsShownToday, ES3.settings);
        ES3.Save<DateTime>(Consts.LAST_AD_SHOW_DATE_SAVE, lastAdShownDate, ES3.settings);

        ES3.StoreCachedFile();
    }
    private void UpdateAdTracking()
    {
        if (lastAdShownDate != DateTime.Today)
        {
            lastAdShownDate = DateTime.Today;
            adsShownToday.Clear();  // Reset the count for each ad unit
            ES3.Save<Dictionary<string, int>>(Consts.ADS_SHOWN_TODAY_SAVE, adsShownToday, ES3.settings);
            ES3.Save<DateTime>(Consts.LAST_AD_SHOW_DATE_SAVE, lastAdShownDate, ES3.settings);

            ES3.StoreCachedFile();
        }
    }

    public void TryShowInterstitial()
    {
        if (GetCanInterstitialShow())
        {
            MaxSdk.ShowInterstitial(adUnitId);
            DataBaseManager.instance.Save(Consts.IS_RECENTLY_REWARDED, false);
            interstitialStartTime = DateTime.Now;
            DataBaseManager.instance.Save(Consts.INTERSTITIAL_TIME, interstitialStartTime.ToString());
        }
    }

    private bool GetCanInterstitialShow()
    {
        if (StageManager.instance.mainStageNum <= 1)
        {
            return false;
        }

        if (interstitialStartTime == null)
        {
            return true;
        }

        TimeSpan timeSpan = DateTime.Now - interstitialStartTime;
        totalSeconds = DataBaseManager.instance.Load(Consts.IS_RECENTLY_REWARDED, false) ?
                Date.GetSecondsByTime(0, interstitialMinuteAfterReward, 0) : Date.GetSecondsByTime(0, offsetInterstitialMinute, 0);

        return timeSpan.TotalSeconds >= totalSeconds;
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(adUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }
}