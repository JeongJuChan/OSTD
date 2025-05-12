using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DailyQuestBar : UI_CurrencyTextPanel
{
    [SerializeField] private TextMeshProUGUI dailyQuestNumberText;
    [SerializeField] private TextMeshProUGUI dailyQuestCurrentInfoText;
    [SerializeField] private GameObject glowObject;

    public void UpdateDailyQuestNumberUI(int currentNumber, int totalCount)
    {
        dailyQuestNumberText.text = $"Daily {currentNumber}/{totalCount}";
    }

    public void UpdateDailyQuestCurrentInfoText(BigInteger currentAmount, BigInteger goalAmount)
    {
        dailyQuestCurrentInfoText.text = $"{currentAmount}/{goalAmount}";
    }

    public void ChangeGlowActiveState(bool isActive)
    {
        glowObject.SetActive(isActive);
    }
}
