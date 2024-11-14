using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_LosePanel : UI_Base
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI earnGoldAmount;
    [SerializeField] private UI_DoubleGoldAdsButton ui_DoubleGoldAdsButton;

    [SerializeField] private float doubleGoldAdsButtonWaitTime = 2f;
    [SerializeField] private float closeButtonWaitTime = 1.3f;

    private WaitForSeconds doubleGoldAdsWaitForSeconds;
    private WaitForSeconds closeButtonWaitForSeconds;

    [SerializeField] private UI_PopupCanvas ui_PopupCanvas;

    public override void Init()
    {
        base.Init();
        AddButtonAction(() => GameManager.instance.UpdateGameState(false));
        AddButtonAction(() => HeroManager.instance.ChangeHeroActiveState(true));
        HeroManager.instance.hero.OnFailed += OpenUIDelay;
        AddButtonAction(CloseUI);
        AddButtonAction(RewardManager.instance.GetReward);
        CloseUI();
        ui_DoubleGoldAdsButton.OnGetDoubleGold += closeButton.onClick.Invoke;
        ui_DoubleGoldAdsButton.Init();
        AddButtonAction(AdsManager.instance.TryShowInterstitial);
        ui_DoubleGoldAdsButton.AddButtonAction(AdsManager.instance.TryShowInterstitial);
        GameManager.instance.OnStart += () => UpdateGetConfirmButtonActiveState(false);
        doubleGoldAdsWaitForSeconds = CoroutineUtility.GetWaitForSeconds(doubleGoldAdsButtonWaitTime);
        closeButtonWaitForSeconds = CoroutineUtility.GetWaitForSeconds(closeButtonWaitTime);
    }

    public void AddButtonAction(UnityAction action)
    {
        closeButton.onClick.AddListener(action);
    }

    public void UpdateGoldAmount(BigInteger amount)
    {
        earnGoldAmount.text = amount.ToString();
    }

    private void UpdateGetConfirmButtonActiveState(bool isActive)
    {
        UpdateCloseButtonActiveState(isActive);
        UpdateDoubleGoldAdsButtonActiveState(isActive);
    }

    private void UpdateCloseButtonActiveState(bool isActive)
    {
        closeButton.gameObject.SetActive(isActive);
    }

    private void UpdateDoubleGoldAdsButtonActiveState(bool isActive)
    {
        ui_DoubleGoldAdsButton.gameObject.SetActive(isActive);
    }

    private void OpenUIDelay()
    {
        ui_PopupCanvas.StartCoroutine(CoActivateButtonSequentially());
    }

    public override void OpenUI()
    {
        base.OpenUI();
    }

    private IEnumerator CoActivateButtonSequentially()
    {
        yield return doubleGoldAdsWaitForSeconds;
        UpdatePlayCount();
        OpenUI();
        UpdateDoubleGoldAdsButtonActiveState(true);
        yield return closeButtonWaitForSeconds;
        UpdateCloseButtonActiveState(true);
    }

    private void UpdatePlayCount()
    {
        int playCount = DataBaseManager.instance.Load(Consts.PLAY_COUNT, 0);
        playCount++;
        DataBaseManager.instance.Save(Consts.PLAY_COUNT, playCount);
    }
}
