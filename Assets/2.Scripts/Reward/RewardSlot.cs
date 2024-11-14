using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Keiwando.BigInteger;
public class RewardSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI amountText;
    public bool isCompleted = false;
    public RewardType type;
    public BigInteger amount;
    public IRewardInfo rewardInfo;

    // public event Action<RewardSlot> OnRewardSlotCompleted;

    public void SetUI(IRewardInfo rewardInfo, int amount)
    {
        icon.sprite = rewardInfo.GetIcon();
        background.color = rewardInfo.GetBackground();
        amountText.text = amount.ToString();
    }

    public void SetValue(RewardType type, int amount, IRewardInfo info)
    {
        this.type = type;
        this.amount = amount;
        this.rewardInfo = info;

        isCompleted = false;
    }

    public void SetUI(IRewardInfo rewardInfo, BigInteger amount)
    {
        icon.sprite = rewardInfo.GetIcon();
        background.color = rewardInfo.GetBackground();
        amountText.text = amount.ChangeMoney();
    }

    public void SetValue(RewardType type, BigInteger amount, IRewardInfo info)
    {
        this.type = type;
        this.amount = amount;
        this.rewardInfo = info;

        isCompleted = false;
    }
}
