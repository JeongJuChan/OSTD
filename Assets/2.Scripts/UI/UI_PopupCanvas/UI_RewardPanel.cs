using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardPanel : UI_Base
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Image boxImage;
    [SerializeField] private UI_EquipmentPooler ui_EquipmentRewardPanel;
    [SerializeField] private UI_CurrencyRewardPanel ui_CurrencyRewardPanel;

    private bool isOpenable;

    public override void Init()
    {
        base.Init();
        ui_EquipmentRewardPanel.Init();
        ui_CurrencyRewardPanel.Init();

        RewardManager.instance.OnGetReward += AddCurrencyReward;
        RewardManager.instance.OnAddEquipmentUI += AddEquipmentReward;

        closeButton.onClick.AddListener(CloseUI);

        CloseUI();

    }

    private void AddCurrencyReward(CurrencyType currencyType, BigInteger amount)
    {
        ui_CurrencyRewardPanel.AddReward(currencyType, amount);
        isOpenable = true;
        TryOpenUI();
    }

    private void AddEquipmentReward(Sprite equipmentSprite, Sprite rankSprite)
    {
        ui_EquipmentRewardPanel.AddReward(equipmentSprite, rankSprite);
        isOpenable = true;
    }

    public void TryOpenUI()
    {
        if (isOpenable)
        {
            OpenUI();
            isOpenable = false;
        }
    }

    public override void CloseUI()
    {
        base.CloseUI();
        ui_CurrencyRewardPanel.ReturnAllUI();
        ui_EquipmentRewardPanel.ReturnAllUI();
    }
}
