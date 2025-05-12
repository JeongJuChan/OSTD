using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageClearPanel : UI_Base
{
    [SerializeField] private UI_CurrencyTextPanel goldTextPanel;
    [SerializeField] private UI_CurrencyTextPanel enforceTextPanel;
    [SerializeField] private Button claimButton;

    public override void Init()
    {
        base.Init();
        StageManager.instance.OnClearStage += OpenUI;
        StageManager.instance.OnUpdateClearCurrencyUI += UpdateText;
        claimButton.onClick.AddListener(StageManager.instance.ClearStage);
        claimButton.onClick.AddListener(CloseUI);
        goldTextPanel.Init();
        enforceTextPanel.Init();
        CloseUI();
    }

    private void UpdateText(BigInteger goldAmount, BigInteger enforceAmount)
    {
        goldTextPanel.UpdateCurrencyText(goldAmount);
        enforceTextPanel.UpdateCurrencyText(enforceAmount);
    }
}
