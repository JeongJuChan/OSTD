using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroTutorialPanel : UI_MainBattleTutorialPanel
{
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private Canvas canvas;

    [SerializeField] private BottomBarController bottomBarController;

    private RectTransform ui_EquipButtonRect;
    private RectTransform ui_TouchequipmentRect;

    private RectTransform ui_compareEquipemntPanel;
    private UI_EquipButton uI_EquipButton;
    private UI_CompareEquipmentPanel ui_CompareEquipmentPanelOrigin;

    private RectTransform ui_equipmentRect;
    private UI_EquipmentButton ui_EquipmentButton;

    private RectTransform ui_levelUpRect;
    private UI_EquipmentLevelUpPanel ui_equipmentLevelUpPanel;

    private RectTransform ui_sellDuplicateButtonRect;
    private Button sellDuplicateButton;

    private RectTransform ui_EquipBestButtonRect;
    private Button equipBestButton;

    public event Action<int, bool> OnHeroTapUnlock;

    protected override void Start()
    {
        currentTutorialName = DataBaseManager.instance.Load(Consts.CURRENT_TUTORIAL_NAME, "");
        closeButtonWaitForSeconds = CoroutineUtility.GetWaitForSeconds(closeButtonWaitTime);

        foreach (var rect in guideRects)
        {
            rect.gameObject.SetActive(false);
        }

        InitDict();

        foreach (var item in GuideManager.instance.tutorialDict.Values)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in GuideManager.instance.guidesDict.Values)
        {
            item.SetActive(false);
        }

        canvas.sortingLayerName = Consts.POPUP_UI_LAYER;


        backgroundPanel.SetActive(false);

        TryShowShopTutorial();
    }

    protected override void InitDict()
    {
        
    }

    public void AddTouchGranadeTutorial()
    {
        if (contentTransform.childCount == 0)
        {
            return;
        }

        GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.HERO_TAP_TOUCHED_GUIDE, false);
        DataBaseManager.instance.Save(Consts.HERO_TAP_TOUCHED_GUIDE, true);

        if (ui_TouchequipmentRect == null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform.GetComponent<RectTransform>());

            ui_EquipButtonRect = contentTransform.GetChild(0).GetComponent<RectTransform>();
            ui_TouchequipmentRect = Instantiate(ui_EquipButtonRect, ui_EquipButtonRect.position, Quaternion.identity, tutorialClonePanel);
            ui_TouchequipmentRect.sizeDelta = new Vector2(225, 225);

            UI_EquipmentButton ui_EquipmentButton = ui_TouchequipmentRect.GetComponent<UI_EquipmentButton>();
            ui_EquipmentButton.AddButtonAction(() => EquipmentManager.instance.CompareEquipments(EquipmentManager.instance.inventory.GetEquipmentStatDataByIndex(0)));
            ui_EquipmentButton.AddButtonAction(InstComparePanel);
            ui_EquipmentButton.AddButtonAction(() => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.GRANADE_TOUCHED_GUIDE, false));

            GuideManager.instance.tutorialDict.Add(Consts.GRANADE_TOUCHED_GUIDE, ui_TouchequipmentRect);
            GuideManager.instance.AddGuidDict(Consts.GRANADE_TOUCHED_GUIDE, (isActive) => ChangeActivateStateClone(Consts.GRANADE_TOUCHED_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.GRANADE_TOUCHED_GUIDE, guideRects[0]);
            StartCoroutine(Wait(Consts.GRANADE_TOUCHED_GUIDE));
        }
    }

    public void InstComparePanel()
    {
        if (ui_compareEquipemntPanel == null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Clones[0]);
            ui_compareEquipemntPanel = Instantiate(Clones[0], Clones[0].position, Quaternion.identity, tutorialClonePanel);
            ui_compareEquipemntPanel.sizeDelta = Clones[0].sizeDelta;
            DataBaseManager.instance.Save(Consts.GRANADE_TOUCHED_GUIDE, true);

            uI_EquipButton = ui_compareEquipemntPanel.GetComponentInChildren<UI_EquipButton>();
            ui_CompareEquipmentPanelOrigin = Clones[0].GetComponent<UI_CompareEquipmentPanel>();
            uI_EquipButton.Init();
            uI_EquipButton.AddButtonAction(EquipmentManager.instance.EquipNewEquipment);
            uI_EquipButton.AddButtonAction(ui_CompareEquipmentPanelOrigin.CloseUI);
            uI_EquipButton.AddButtonAction(InstEquipGranadePanel);
            uI_EquipButton.AddButtonAction(() => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.GRANADE_EQUIPPED_GUIDE, false));

            GuideManager.instance.tutorialDict.Add(Consts.GRANADE_EQUIPPED_GUIDE, ui_compareEquipemntPanel);
            GuideManager.instance.AddGuidDict(Consts.GRANADE_EQUIPPED_GUIDE, (isActive) => ChangeActivateStateClone(Consts.GRANADE_EQUIPPED_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.GRANADE_EQUIPPED_GUIDE, guideRects[1]);
        }

        StartCoroutine(Wait(Consts.GRANADE_EQUIPPED_GUIDE));
    }

    public void InstEquipGranadePanel()
    {
        if (ui_equipmentRect == null)
        {
            ui_equipmentRect = Instantiate(Clones[1], Clones[1].position, Quaternion.identity, tutorialClonePanel);
            DataBaseManager.instance.Save(Consts.GRANADE_EQUIPPED_GUIDE, true);
            ui_EquipmentButton = ui_equipmentRect.GetComponent<UI_EquipmentButton>();
            ui_EquipmentButton.AddButtonAction(() => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.EQUIPPED_GRANADE_TOUCHED_GUIDE, false));
            ui_EquipmentButton.AddButtonAction(InstLevelUpPanel);

            GuideManager.instance.tutorialDict.Add(Consts.EQUIPPED_GRANADE_TOUCHED_GUIDE, ui_equipmentRect);
            GuideManager.instance.AddGuidDict(Consts.EQUIPPED_GRANADE_TOUCHED_GUIDE, (isActive) => ChangeActivateStateClone(Consts.EQUIPPED_GRANADE_TOUCHED_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.EQUIPPED_GRANADE_TOUCHED_GUIDE, guideRects[2]);
        }

        StartCoroutine(Wait(Consts.EQUIPPED_GRANADE_TOUCHED_GUIDE));
    }

    public void InstLevelUpPanel()
    {
        DataBaseManager.instance.Save(Consts.EQUIPPED_GRANADE_TOUCHED_GUIDE, true);
        UIManager.instance.GetUIElement<UI_HeroEquipmentController>().ShowEquipmentInfo(1);

        if (ui_levelUpRect == null)
        {
            ui_levelUpRect = Instantiate(Clones[2], Clones[2].position, Quaternion.identity, tutorialClonePanel);
            ui_equipmentLevelUpPanel = ui_levelUpRect.GetComponent<UI_EquipmentLevelUpPanel>();

            ui_equipmentLevelUpPanel.levelUpButton.Init();
            ui_equipmentLevelUpPanel.levelUpMaxButton.Init();

            EquipmentManager.instance.UpdateCurrentEquipmentIndex(1);

            ui_equipmentLevelUpPanel.levelUpButton.AddButtonAction(
                () => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, false));
            ui_equipmentLevelUpPanel.levelUpButton.AddButtonAction(() => DataBaseManager.instance.Save(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, true));
            ui_equipmentLevelUpPanel.levelUpButton.AddButtonAction(EquipmentManager.instance.LevelUpEquipment);
            ui_equipmentLevelUpPanel.levelUpButton.AddButtonAction(InstSellDuplicateButton);
            ui_equipmentLevelUpPanel.levelUpMaxButton.AddButtonAction(
                () => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, false));
            ui_equipmentLevelUpPanel.levelUpMaxButton.AddButtonAction(() => DataBaseManager.instance.Save(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, true));
            ui_equipmentLevelUpPanel.levelUpMaxButton.AddButtonAction(EquipmentManager.instance.LevelUpMaxEquipment);
            ui_equipmentLevelUpPanel.levelUpMaxButton.AddButtonAction(InstSellDuplicateButton);
            GuideManager.instance.tutorialDict.Add(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, ui_levelUpRect);
            GuideManager.instance.AddGuidDict(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, (isActive) => ChangeActivateStateClone(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, guideRects[3]);
        }

        StartCoroutine(Wait(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE));
    }

    public void InstSellDuplicateButton()
    {
        if (ui_sellDuplicateButtonRect == null)
        {
            ui_sellDuplicateButtonRect = Instantiate(Clones[3], Clones[3].position, Quaternion.identity, tutorialClonePanel);
            DataBaseManager.instance.Save(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE, true);

            sellDuplicateButton = ui_sellDuplicateButtonRect.GetComponent<Button>();
            sellDuplicateButton.interactable = true;
            sellDuplicateButton.onClick.AddListener(EquipmentManager.instance.SellDuplicates);
            sellDuplicateButton.onClick.AddListener(() => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.SELL_DUPLICATES_GUIDE, false));
            sellDuplicateButton.onClick.AddListener(() => DataBaseManager.instance.Save(Consts.SELL_DUPLICATES_GUIDE, true));
            sellDuplicateButton.onClick.AddListener(() => UIManager.instance.GetUIElement<UI_InventoryButtonsPanel>().UpdateSellDuplicateButtonActiveState(false));
            sellDuplicateButton.onClick.AddListener(TryShowShopTutorial);
            

            GuideManager.instance.tutorialDict.Add(Consts.SELL_DUPLICATES_GUIDE, ui_sellDuplicateButtonRect);
            GuideManager.instance.AddGuidDict(Consts.SELL_DUPLICATES_GUIDE, (isActive) => ChangeActivateStateClone(Consts.SELL_DUPLICATES_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.SELL_DUPLICATES_GUIDE, guideRects[4]);
        }

        StartCoroutine(Wait(Consts.SELL_DUPLICATES_GUIDE)); 
    }


    public void InitAutoEquip()
    {
        GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.HERO_TAP_TOUCHED_GUIDE, false);

        if (ui_EquipBestButtonRect == null)
        {
            ui_EquipBestButtonRect = Instantiate(Clones[4], Clones[4].position, Quaternion.identity, tutorialClonePanel);

            equipBestButton = ui_EquipBestButtonRect.GetComponent<Button>();
            equipBestButton.interactable = true;
            equipBestButton.onClick.AddListener(EquipmentManager.instance.AutoEquip);
            equipBestButton.onClick.AddListener(ShowSellingDuplicateTutorialAfterAutoEquip);
            equipBestButton.onClick.AddListener(() => DataBaseManager.instance.Save(Consts.AUTO_EQUIP_GUIDE, true));
            equipBestButton.onClick.AddListener(() => UIManager.instance.GetUIElement<UI_InventoryButtonsPanel>().UpdateAutoEquipButtonActiveState(false));

            GuideManager.instance.tutorialDict.Add(Consts.AUTO_EQUIP_GUIDE, ui_EquipBestButtonRect);
            GuideManager.instance.AddGuidDict(Consts.AUTO_EQUIP_GUIDE, (isActive) => ChangeActivateStateClone(Consts.AUTO_EQUIP_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.AUTO_EQUIP_GUIDE, guideRects[5]);
        }

        StartCoroutine(Wait(Consts.AUTO_EQUIP_GUIDE));
    }

    private void ShowSellingDuplicateTutorialAfterAutoEquip()
    {
        GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.AUTO_EQUIP_GUIDE, false);
        if (ui_sellDuplicateButtonRect != null)
        {
            sellDuplicateButton.onClick.RemoveListener(TryShowShopTutorial);
        }
        InstSellDuplicateButton();
    }

    protected override IEnumerator Wait(string str)
    {
        yield return null;
        GuideManager.instance.ToggleGuideWithBackgroundPanel(str, true);
    }

    public void TryShowShopTutorial()
    {
        if (StageManager.instance.mainStageNum > 1 && DataBaseManager.instance.ContainsKey(Consts.EQUIPPED_GRANADE_LEVELUP_GUIDE) &&
            !DataBaseManager.instance.ContainsKey(Consts.SHOP_TAP_TOUCHED_GUIDE))
        {
            OnHeroTapUnlock?.Invoke(0, false);
            GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.SHOP_TAP_TOUCHED_GUIDE, true);
        }
    }
}
