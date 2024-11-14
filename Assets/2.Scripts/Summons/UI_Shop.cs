using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UI_Shop : UI_BottomElement
{
    [SerializeField] private SummonController controller;
    [SerializeField] private UI_SummonResult resultUI;
    [SerializeField] private UI_SummonInfo infoUI;
    [SerializeField] private GuideController guide;

    [SerializeField] private UI_FreeGemPanel ui_FreeGemPanel;
    [SerializeField] private CurrencyTextPanel[] currencyTextPanels;

    private bool initialized = false;


    public override void OpenUI()
    {
        base.OpenUI();

        if (DataBaseManager.instance.ContainsKey(Consts.SHOP_TAP_TOUCHED_GUIDE))
        {
            NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Shop, GetRedDotActiveState());
        }

        if (DataBaseManager.instance.ContainsKey(Consts.HERO_TAP_TOUCHED_GUIDE_START))
        {
            if (CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem) < 5 || DataBaseManager.instance.ContainsKey(Consts.SUMMON_EQUIPMENT_GUIDE))
            {
                return;
            }

            UIManager.instance.GetUIElement<UI_ShopTutorialPanel>().ShowSummonGuide();
        }

        
    }

    public override void Initialize()
    {
        base.Initialize();
        //NotificationManager.instance.SetNotification(RedDotIDType.ShowSummonButton, CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem) >= 300);
        foreach (CurrencyTextPanel panel in currencyTextPanels)
        {
            panel.Init();
        }

        ui_FreeGemPanel.SetReddotFunc(GetRedDotActiveState);
        ui_FreeGemPanel.Init();

        Hes_Init();

        UIManager.instance.GetUIElement<UI_ShopTutorialPanel>().Init();

        if (DataBaseManager.instance.ContainsKey(Consts.SHOP_TAP_TOUCHED_GUIDE))
        {
            NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Shop, GetRedDotActiveState());
        }
    }

    public override void StartInit()
    {
        controller.InitUnlock();
    }

    private void OnDisable()
    {
        // NotificationManager.instance.SetNotification(RedDotIDType.ShowSummonButton, CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem) >= 300);
    }

    public void Hes_Init()
    {
        if (initialized) return;
        GetElements();
        InitializeFunctions();

        // guide.Initialize();

        initialized = true;
    }

    private void GetElements()
    {
        resultUI = UIManager.instance.GetUIElement<UI_SummonResult>();
        infoUI = UIManager.instance.GetUIElement<UI_SummonInfo>();
    }

    private void InitializeFunctions()
    {
        resultUI.Initialize();
        infoUI.Initialize();

        controller.Initalize();
    }
}
