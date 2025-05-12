using TMPro;
using UnityEngine;

public class RemainTimeViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainTime;

    public void UpdateRemainTime(string remainTimeText)
    {
        remainTime.text = $"남은 시간 : {remainTimeText}";
    }
}