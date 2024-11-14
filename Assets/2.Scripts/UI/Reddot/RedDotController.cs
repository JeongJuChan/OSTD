using UnityEngine;
using UnityEngine.UI;

public class RedDotController : MonoBehaviour
{
    public RedDotIDType notificationKey;
    private Image redDotImage;

    private void Awake()
    {
        redDotImage = GetComponent<Image>();
        NotificationManager.instance.OnNotificationChanged += UpdateRedDot;
    }
    /// <summary>
    /// 알림 상태에 따라 레드 닷의 가시성을 업데이트합니다.
    /// </summary>
    /// 
    public bool GetIsReddotEnable()
    {
        return redDotImage.enabled;
    }

    private void UpdateRedDot()
    {
        redDotImage.enabled = NotificationManager.instance.GetNotificationState(notificationKey.ToString());
    }
}