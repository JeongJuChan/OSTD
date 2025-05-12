using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keiwando.BigInteger;
using UnityEngine;

public class UI_Alert : UI_Base
{
    [SerializeField] Transform slotArea;
    [SerializeField] AlertSlot alertSlotPrefab;
    [SerializeField] PowerAlertSlot powerAlertSlotPrefab;

    [SerializeField] Color increaseColor;
    [SerializeField] Color decreaseColor;
    [SerializeField] Sprite[] icons;

    private Queue<AlertSlot> alertSlots = new Queue<AlertSlot>();
    private Queue<AlertSlot> activedAlertSlots = new Queue<AlertSlot>();

    private Queue<PowerAlertSlot> powerAlertSlots = new Queue<PowerAlertSlot>();
    private Queue<PowerAlertSlot> activedPowerAlertSlots = new Queue<PowerAlertSlot>();

    private bool isInitialized;
    private Coroutine powerMessageCoroutine;
    private WaitForSeconds powerMessageDelay = new WaitForSeconds(0.1f);

    public bool isPowerMessageWaiting = false;


    public void Initialize()
    {
        // SetCollections();
        isInitialized = true;
    }

    private void SetCollections()
    {
        alertSlots = new Queue<AlertSlot>();
        activedAlertSlots = new Queue<AlertSlot>();

        powerAlertSlots = new Queue<PowerAlertSlot>();
        activedPowerAlertSlots = new Queue<PowerAlertSlot>();
    }

    public void AlertMessage(string message)
    {
        // Debug.Assert(isInitialized, "UI_Alert is not initialized");

        AlertSlot slot;

        if (alertSlots.Count > 0) slot = alertSlots.Dequeue();
        else
        {
            slot = Instantiate(alertSlotPrefab, slotArea);
            slot.OnAnimEnd += EnqueueAlert;
        }

        slot.SetMessage(message);
        slot.gameObject.SetActive(true);
        activedAlertSlots.Enqueue(slot);

        if (activedAlertSlots.Count > 1) activedAlertSlots.Dequeue().QuickAnim();
    }


    private void EnqueueAlert(AlertSlot slot)
    {
        alertSlots.Enqueue(slot);
        if (activedAlertSlots.Peek() == slot) activedAlertSlots.Dequeue();
        slot.gameObject.SetActive(false);
    }


    public void PowerMessage(BigInteger increase)
    {
        if (isPowerMessageWaiting) return;
        if (!isInitialized || increase == 0 || !GameManager.instance.isInitializing) return;

        if (powerMessageCoroutine != null) StopCoroutine(powerMessageCoroutine);

        powerMessageCoroutine = StartCoroutine(HandlePowerMessageDelay(increase));
    }

    private IEnumerator HandlePowerMessageDelay(BigInteger increase)
    {
        yield return powerMessageDelay;

        PowerAlertSlot slot;
        if (powerAlertSlots.Count > 0) slot = powerAlertSlots.Dequeue();
        else
        {
            slot = Instantiate(powerAlertSlotPrefab, slotArea);
            slot.OnAnimEnd += EnqueuePowerAlert;
        }

        string amount = increase < 0 ? (-increase).ChangeMoney() : increase.ChangeMoney();
        Color color = increase < 0 ? decreaseColor : increaseColor;
        Sprite sprite = icons[increase < 0 ? 0 : 1];

        slot.SetMessage(amount, color, sprite);
        slot.gameObject.SetActive(true);
        activedPowerAlertSlots.Enqueue(slot);

        if (activedPowerAlertSlots.Count > 1) activedPowerAlertSlots.Dequeue().QuickAnim();

        powerMessageCoroutine = null;
    }

    private void EnqueuePowerAlert(PowerAlertSlot slot)
    {
        powerAlertSlots.Enqueue(slot);
        if (activedPowerAlertSlots.Peek() == slot) activedPowerAlertSlots.Dequeue();
        slot.gameObject.SetActive(false);
    }

}
