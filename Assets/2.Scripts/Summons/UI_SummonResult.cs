using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;

public class UI_SummonResult : UI_Base
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Transform slotArea;
    [SerializeField] private GameObject slotPrefab;

    [SerializeField] private Button smallSummonBtn;
    [SerializeField] private Button largeSummonBtn;
    [SerializeField] private Button adsSummonBtn;

    [SerializeField] private Image smallSummonImage;
    [SerializeField] private Image largeSummonImage;
    [SerializeField] private Image adsSummonImage;

    [SerializeField] private TextMeshProUGUI smallTitle;
    [SerializeField] private TextMeshProUGUI largeTitle;
    [SerializeField] private TextMeshProUGUI adsTitle;
    [SerializeField] private TextMeshProUGUI smallPriceText;
    [SerializeField] private TextMeshProUGUI largePriceText;
    [SerializeField] private TextMeshProUGUI adsPriceText;

    [SerializeField] private Color bgNormalColor;
    [SerializeField] private Color bgDisabledColor;
    [SerializeField] private Color titleNormalColor;
    [SerializeField] private Color titleDisabledColor;
    [SerializeField] private Color priceNormalColor;
    [SerializeField] private Color priceDisabledColor;

    private SummonType currentType;

    [SerializeField] private List<SummonResultSlot> slotPoolDic;
    [SerializeField] private List<SummonResultSlot> activatedSlots;
    private int activatedIndex;

    private Action smallSummon;
    private Action largeSummon;
    private Action adsSummon;

    private Action<CurrencyType, int> OnSummonSmall;
    private Action<CurrencyType, int> OnSummonLarge;

    private Action currentSummon;

    private int currentQuantityType;

    private SummonUnitInfo smallInfo;
    private SummonUnitInfo largeInfo;

    private CurrencyType smallCurrencyType = CurrencyType.Gem;
    private CurrencyType largeCurrencyType = CurrencyType.Gem;

    private BigInteger smallPrice;
    private BigInteger largePrice;

    private UIAnimations uIAnimations;

    private WaitForSeconds waitForOpenUI = new WaitForSeconds(0.2f);
    private WaitForSeconds waitForNextSlot = new WaitForSeconds(0.005f);
    private WaitForSeconds waitForwaitTimeForNextSlot = new WaitForSeconds(0.1f);
    private WaitForSeconds waitForCloseBtn = new WaitForSeconds(0.4f);
    private WaitForSeconds waitForShowUI = new WaitForSeconds(0.4f);

    private bool isShowingSummonAnimState;

    Coroutine showingSlot;
    Coroutine redoSummon;

    private CurrencyType preCurrencyType;

    public void Initialize()
    {
        AddCallbacks();
        SetCollections();
        LoadSkipState();
    }

    private void LoadSkipState()
    {
        if (ES3.KeyExists(Consts.IS_SHOWING_SUMMON_ANIM_STATE, ES3.settings))
        {
            isShowingSummonAnimState = ES3.Load<bool>(Consts.IS_SHOWING_SUMMON_ANIM_STATE, true, ES3.settings);
        }
        else
        {
            isShowingSummonAnimState = true;
            UpdateSkipSummonState();
        }
    }

    private void AddCallbacks()
    {
        closeBtn.onClick.AddListener(CloseUI);

        smallSummonBtn.onClick.AddListener(() => Summon(1));
        largeSummonBtn.onClick.AddListener(() => Summon(2));
        adsSummonBtn.onClick.AddListener(() => Summon(3));
    }

    private void SetCollections()
    {
        slotPoolDic = new List<SummonResultSlot>();
        activatedSlots = new List<SummonResultSlot>();
        uIAnimations = UIAnimations.instance;
    }

    public void SetResultUI(SummonType type, Action smallSummon, Action largeSummon, Action adsSummon)
    {
        currentType = type;

        this.smallSummon = smallSummon;
        this.largeSummon = largeSummon;
        this.adsSummon = adsSummon;

        activatedIndex = 0;
    }

    public void SetResultUI(SummonType type, Action<CurrencyType, int> smallSummon, Action<CurrencyType, int> largeSummon)
    {
        currentType = type;

        OnSummonSmall = smallSummon;
        OnSummonLarge = largeSummon;

        activatedIndex = 0;
    }

    public void SetButtonInfo(CurrencyType currencyType, SummonUnitInfo small, SummonUnitInfo large)
    {
        smallInfo = small;
        largeInfo = large;

        smallTitle.text = $"{small.quantity}회";
        largeTitle.text = $"{large.quantity}회";

        smallPriceText.text = $"{small.price}";
        largePriceText.text = $"{large.price}";

        BigInteger currentValue = CurrencyManager.instance.GetCurrencyValue(currencyType);
        Sprite currencySprite = CurrencyManager.instance.GetCurrency(currencyType).GetIcon();
        smallSummonImage.sprite = currencySprite;
        largeSummonImage.sprite = currencySprite;
        UpdateButtonAvailability(currentValue);
    }


    private void UpdateButtonAvailability(BigInteger value)
    {
        bool isSmallAvailable = value >= smallInfo.price;
        bool isLargeAvailable = value >= largeInfo.price;

        smallSummonBtn.enabled = isSmallAvailable;
        smallSummonBtn.image.color = (isSmallAvailable) ? bgNormalColor : bgDisabledColor;
        smallTitle.color = (isSmallAvailable) ? titleNormalColor : titleDisabledColor;
        smallPriceText.color = (isSmallAvailable) ? priceNormalColor : priceDisabledColor;

        largeSummonBtn.enabled = isLargeAvailable;
        largeSummonBtn.image.color = (isLargeAvailable) ? bgNormalColor : bgDisabledColor;
        largeTitle.color = (isLargeAvailable) ? titleNormalColor : titleDisabledColor;
        largePriceText.color = (isLargeAvailable) ? priceNormalColor : priceDisabledColor;
    }

    public void AddSlot(ISummonable item)
    {
        SummonResultSlot slot;

        if (slotPoolDic.Count > activatedIndex) slot = slotPoolDic[activatedIndex];
        else
        {
            slot = Instantiate(slotPrefab, slotArea).GetComponent<SummonResultSlot>();
            slot.InitSlot(uIAnimations);
            slotPoolDic.Add(slot);
        }

        slot.ResetSlot();
        slot.SetSlotInfo(item);
        activatedSlots.Add(slot);

        activatedIndex++;
    }

    public void ShowSlots()
    {
        if (!gameObject.activeSelf) OpenUI();

        UpdateAdsUI();

        if (showingSlot != null) StopCoroutine(showingSlot);
        showingSlot = StartCoroutine(ShowSlotsWithAnimation());
    }

    private void UpdateAdsUI()
    {
        bool isAdsAvailable = AdsManager.instance.GetAdCount($"{currentType}Summon") < 3;

        adsSummonBtn.gameObject.SetActive(isAdsAvailable && CurrencyManager.instance.GetCurrencyValue(largeCurrencyType) < largePrice); 
        adsPriceText.text = $"{3 - AdsManager.instance.GetAdCount($"{currentType}Summon")} / 3";
    }

    public void UpdateSummonInfo(CurrencyType smallCurrencyType, CurrencyType largeCurrencyType, bool isSmallAvailable,
        bool isLargeAvailable, BigInteger smallPrice, BigInteger largePrice, SummonUnitInfo smallInfo, SummonUnitInfo largeInfo)
    {
        SetInfo(smallInfo, largeInfo);

        this.largeCurrencyType = largeCurrencyType;
        this.largePrice = largePrice;
        this.smallCurrencyType = smallCurrencyType;
        this.smallPrice = smallPrice;

        smallTitle.text = $"{smallInfo.quantity}회";
        smallPriceText.text = $"{smallPrice}";
        largeTitle.text = $"{largeInfo.quantity}회";
        largePriceText.text = $"{largePrice}";

        smallSummonBtn.enabled = isSmallAvailable;
        smallSummonBtn.image.color = (isSmallAvailable) ? bgNormalColor : bgDisabledColor;
        smallTitle.color = (isSmallAvailable) ? titleNormalColor : titleDisabledColor;
        smallPriceText.color = (isSmallAvailable) ? priceNormalColor : priceDisabledColor;
        smallSummonImage.sprite = CurrencyManager.instance.GetCurrency(smallCurrencyType).GetIcon();

        largeSummonBtn.enabled = isLargeAvailable;
        largeSummonBtn.image.color = (isLargeAvailable) ? bgNormalColor : bgDisabledColor;
        largeTitle.color = (isLargeAvailable) ? titleNormalColor : titleDisabledColor;
        largePriceText.color = (isLargeAvailable) ? priceNormalColor : priceDisabledColor;
        largeSummonImage.sprite = CurrencyManager.instance.GetCurrency(largeCurrencyType).GetIcon();
    }

    IEnumerator ShowSlotsWithAnimation()
    {
        yield return waitForOpenUI;
        ActivateSummonButtons(true);

        for (int i = 0; i < activatedSlots.Count; i++)
        {
            if (i == activatedSlots.Count - 1)
            {
                activatedSlots[i].ShowSlot(!isShowingSummonAnimState);
                yield return waitForwaitTimeForNextSlot;
            }
            else
            {
                yield return activatedSlots[i].ShowSlot(!isShowingSummonAnimState) ? waitForwaitTimeForNextSlot : waitForNextSlot;
            }
        }

        yield return waitForCloseBtn;

        // if (currentQuantityType == 1)
        // {
        //     OnSummonSmall?.Invoke(smallCurrencyType, int.Parse(smallPrice.ToString()));
        // }
        // else if (currentQuantityType == 2)
        // {
        //     OnSummonLarge?.Invoke(largeCurrencyType, int.Parse(largePrice.ToString()));
        // }
    }

    public void ActivateSummonButtons(bool isActive)
    {
        closeBtn.gameObject.SetActive(isActive);
        smallSummonBtn.gameObject.SetActive(isActive);
        largeSummonBtn.gameObject.SetActive(isActive);
    }

    private void UpdateSkipSummonState()
    {
        isShowingSummonAnimState = !isShowingSummonAnimState;
        ES3.Save<bool>(Consts.IS_SHOWING_SUMMON_ANIM_STATE, isShowingSummonAnimState, ES3.settings);
        ES3.StoreCachedFile();
        //if (!skipAnim) currentSummon = null;
    }

    private void Summon(int type)
    {
        currentQuantityType = type;
        Action summon = null;
        BigInteger currentPrice = 0;
        CurrencyType currencyType = CurrencyType.Gem;
        ClearSlots();

        if (redoSummon != null) StopCoroutine(redoSummon);

        switch (type)
        {
            case 1:
                //OnSummonSmall?.Invoke(smallCurrencyType, int.Parse(smallPrice.ToString()));
                currentPrice = smallPrice;
                currencyType = smallCurrencyType;
                summon = () => OnSummonSmall?.Invoke(currencyType, int.Parse(currentPrice.ToString()));
                /*summon = smallSummon;
                currentPrice = smallInfo.price;*/
                break;
            case 2:
                //OnSummonLarge?.Invoke(largeCurrencyType, int.Parse(largePrice.ToString()));
                currentPrice = largePrice;
                currencyType = largeCurrencyType;
                summon = () => OnSummonLarge?.Invoke(currencyType, int.Parse(currentPrice.ToString()));
                /*summon = largeSummon;
                currentPrice = largeInfo.price;*/
                break;
            case 3:
                AdsManager.instance.ShowRewardedAdByDay($"{currentType}Summon", (reward, adInfo) =>
                {
                    adsSummon?.Invoke();
                    FirebaseAnalytics.LogEvent($"Reward_shop_equip_{AdsManager.instance.GetAdCount($"{currentType}Summon")}");
                    UpdateAdsUI();
                });
                break;
        }
        if (summon == null) return;

        currentSummon = summon;
        redoSummon = StartCoroutine(RedoSummon(summon));
    }

    IEnumerator RedoSummon(Action action)
    {
        ClearSlots();
        yield return waitForNextSlot;

        action();
    }

    private void OnDisable()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        ClearSlots();

        currentSummon = null;

        ActivateSummonButtons(false);
    }

    public void ClearSlots()
    {
        if (activatedSlots == null || activatedSlots.Count == 0) return;

        foreach (SummonResultSlot slot in activatedSlots)
        {
            slot.ResetSlot();
        }

        activatedSlots.Clear();

        ActivateSummonButtons(false);
    }

    private void RemoveCallbacks()
    {
        // currencyManager.GetCurrency(Enums.CurrencyType.Gem).OnCurrencyChange -= UpdateButtonAvailability;
    }

    private void OnDestroy()
    {
        RemoveCallbacks();
    }

    public void SetInfo(SummonUnitInfo smallInfo, SummonUnitInfo largeInfo)
    {
        this.smallInfo = smallInfo;
        this.largeInfo = largeInfo;
    }

    public void SetCurrentQuantityType(int quantityNum)
    {
        currentQuantityType = quantityNum;
    }
}