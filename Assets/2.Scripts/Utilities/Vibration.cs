using UnityEngine;

public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    private static AndroidJavaObject unityActivity;

    static Vibration()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            if (unityActivity != null)
            {
                vibrator = unityActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
        }
    }
#endif

    public static void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator != null)
        {
            using (var vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
            {
                int amplitude = 1; // Lower values represent softer vibration
                AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                vibrator.Call("vibrate", vibrationEffect);
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        // iOS doesn't support intensity control, so fall back to regular vibration
        Handheld.Vibrate();
#else
        Debug.Log("Vibration not supported on this platform.");
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator != null)
        {
            vibrator.Call("cancel");
        }
#endif
    }
}
