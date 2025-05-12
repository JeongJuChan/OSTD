using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Keiwando.BigInteger;

public class RewardResultPanel : UI_Base
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private RewardSlot rewardSlotPrefab;
    [SerializeField] private Transform rewardSlotContainer;
    private Queue<RewardSlot> rewardSlotPool = new Queue<RewardSlot>();
    private Queue<RewardSlot> activeRewardSlot = new Queue<RewardSlot>();
    private int initialPoolSize = 10;
    private CurrencyManager currencyManager;

    public void Initalize()
    {
        currencyManager = CurrencyManager.instance;

        InitializeRewardSlotPool();
    }

    private void InitializeRewardSlotPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            RewardSlot slot = Instantiate(rewardSlotPrefab, rewardSlotContainer);
            slot.gameObject.SetActive(false);
            rewardSlotPool.Enqueue(slot);
        }
    }

    RewardSlot tempRewardSlot;

    public RewardSlot GetRewardSlot(RewardType type, int amount, IRewardInfo info)
    {
        if (rewardSlotPool.Count > 0)
        {
            tempRewardSlot = rewardSlotPool.Dequeue();
            tempRewardSlot.SetValue(type, amount, info);
            activeRewardSlot.Enqueue(tempRewardSlot);
            return tempRewardSlot;
        }
        else
        {
            RewardSlot slot = Instantiate(rewardSlotPrefab, rewardSlotContainer);
            slot.SetValue(type, amount, info);
            activeRewardSlot.Enqueue(slot);
            return slot;
        }
    }

    public RewardSlot GetRewardSlot(RewardType type, BigInteger amount, IRewardInfo info)
    {
        if (rewardSlotPool.Count > 0)
        {
            tempRewardSlot = rewardSlotPool.Dequeue();
            tempRewardSlot.SetValue(type, amount, info);
            activeRewardSlot.Enqueue(tempRewardSlot);
            return tempRewardSlot;
        }
        else
        {
            RewardSlot slot = Instantiate(rewardSlotPrefab, rewardSlotContainer);
            slot.SetValue(type, amount, info);
            activeRewardSlot.Enqueue(slot);
            return slot;
        }
    }

    public void ShowStageClearRewardSlot()
    {
        titleText.text = "스테이지 클리어";
        ShowRewardSlot();
    }

    public void ShowRewardSlot()
    {
        if (activeRewardSlot.Count == 0) return;
        gameObject.SetActive(true);


        foreach (RewardSlot slot in activeRewardSlot)
        {
            if (slot.isCompleted) continue;
            slot.gameObject.SetActive(true);
            if (slot.rewardInfo != null && slot.rewardInfo is EquipmentRewardInfo)
            {
                EquipmentRewardInfo info = slot.rewardInfo as EquipmentRewardInfo;

                // EquipmentManager.instance.UpdateEquipmentCount(EquipmentManager.instance.GetData(info.name), slot.amount.ToInt());
            }

            if (currencyManager.TryUpdateCurrency(slot.type, slot.amount))
            {
                slot.isCompleted = true;
            }
        }

        StartCoroutine(ShowRewardSlotCoroutine());
    }

    public void HideRewardSlot()
    {
        gameObject.SetActive(false);

        while (activeRewardSlot.Count > 0)
        {
            RewardSlot slot = activeRewardSlot.Dequeue();
            slot.gameObject.SetActive(false);
            rewardSlotPool.Enqueue(slot);
        }

        titleText.text = "보상";
    }

    IEnumerator ShowRewardSlotCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        HideRewardSlot();
    }

    public void ReturnRewardSlot(RewardSlot slot)
    {
        slot.gameObject.SetActive(false);
        rewardSlotPool.Enqueue(slot);
    }

    public void ClearRewards()
    {
        while (rewardSlotPool.Count > 0)
        {
            RewardSlot slot = rewardSlotPool.Dequeue();
            slot.gameObject.SetActive(false);
        }
    }
}

