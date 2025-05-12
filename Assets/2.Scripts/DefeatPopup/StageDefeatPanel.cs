using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageDefeatPanel : UI_Popup, UIInitNeeded
{
    [SerializeField] private Button backgroundButton;
    [SerializeField] private RectTransform defeatMainPanelRectTransform;
    [SerializeField] private RectTransform closeTextRectTransform;
    [SerializeField] private Button goToForgeButton;
    [SerializeField] private Button goToSummonColleagueButton;
    [SerializeField] private Button goToCastleLevelUpButton;

    [SerializeField] private DefeatLockElement[] defeatLockElements;

    private const float DEFEAT_MODIFIED_VALUE = 0.15f;
    private const float CLOSE_MODIFIED_VALUE = 0.7f;

    public event Action OnForgeSlot;

    public override void Init()
    {
        base.Init();
        
        foreach (DefeatLockElement defeatLockElement in defeatLockElements)
        {
            defeatLockElement.InitUnlock();
        } 

        //FindAnyObjectByType<Castle>().OnDead += OpenPanel;
        InitPos();
        backgroundButton.onClick.AddListener(OnPointerDown);
        gameObject.SetActive(false);
        goToForgeButton.onClick.AddListener(ForgeSlot);
        UI_BottomElement summonElement = UIManager.instance.GetUIElement<UI_Shop>();

        goToSummonColleagueButton.onClick.AddListener(() => summonElement.openUI?.Invoke());
        goToSummonColleagueButton.onClick.AddListener(OnPointerDown);
        //goToCastleLevelUpButton.onClick.AddListener(() => UIManager.instance.GetUIElement<UI_Castle>().openUI?.Invoke());
        goToCastleLevelUpButton.onClick.AddListener(OnPointerDown);

        Initialize();
    }

    private void InitPos()
    {
        float height = Screen.height;
        Vector2 defeatPos = defeatMainPanelRectTransform.anchoredPosition;
        defeatPos.y = -height * DEFEAT_MODIFIED_VALUE;
        defeatMainPanelRectTransform.anchoredPosition = defeatPos;
        Vector2 closePos = closeTextRectTransform.anchoredPosition;
        closePos.y = -height * CLOSE_MODIFIED_VALUE;
        closeTextRectTransform.anchoredPosition = closePos;
    }

    private void OpenPanel()
    {
        //if (DialogManager.instance.IsBackgroundActivate())
        //{
        //    return;
        //}
        gameObject.SetActive(true);
    }

    private void OnPointerDown()
    {
        gameObject.SetActive(false);
        OnChangePopupState?.Invoke(false);
    }

    private void ForgeSlot()
    {
        OnPointerDown();
        OnForgeSlot?.Invoke();
    }
}
