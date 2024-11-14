using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopTutorialPanel : UI_MainBattleTutorialPanel
{
    [SerializeField] private SummonSlot equipmentSummonSlot;
    [SerializeField] private RectTransform buttonParent;

    protected override void Start()
    {
        
    }

    protected override void InitDict()
    {

    }

    public override void Init()
    {
        currentTutorialName = DataBaseManager.instance.Load(Consts.CURRENT_TUTORIAL_NAME, "");
        closeButtonWaitForSeconds = CoroutineUtility.GetWaitForSeconds(closeButtonWaitTime);
        InitDict();

        foreach (var item in GuideManager.instance.tutorialDict.Values)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in GuideManager.instance.guidesDict.Values)
        {
            item.SetActive(false);
        }

        foreach (var item in guideRects)
        {
            item.SetActive(false);
        }
        

        backgroundPanel.SetActive(false);
    }

    public void ShowSummonGuide()
    {
        GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.SHOP_TAP_TOUCHED_GUIDE, false);
        DataBaseManager.instance.Save(Consts.SHOP_TAP_TOUCHED_GUIDE, true);
        Button summonButton = null;
        RectTransform summonButtonRect = null;


        if (CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem) >= 45)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Clones[0]);
            summonButtonRect = Instantiate(Clones[0], Clones[0].position, Quaternion.identity, buttonParent);
            summonButtonRect.sizeDelta = Clones[0].sizeDelta;
            summonButton = summonButtonRect.GetComponent<Button>();
            summonButton.onClick.AddListener(equipmentSummonSlot.LargeSummon);
            GuideManager.instance.guidesDict.Add(Consts.SUMMON_EQUIPMENT_GUIDE, guideRects[0]);
        }
        else
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Clones[1]);
            summonButtonRect = Instantiate(Clones[1], Clones[1].position, Quaternion.identity, buttonParent);
            summonButtonRect.sizeDelta = Clones[1].sizeDelta;
            summonButton = summonButtonRect.GetComponent<Button>();
            summonButton.onClick.AddListener(equipmentSummonSlot.LargeSummon);
            GuideManager.instance.guidesDict.Add(Consts.SUMMON_EQUIPMENT_GUIDE, guideRects[1]);
        }

        GuideManager.instance.tutorialDict.Add(Consts.SUMMON_EQUIPMENT_GUIDE, summonButtonRect);
        GuideManager.instance.AddGuidDict(Consts.SUMMON_EQUIPMENT_GUIDE, (isActive) => ChangeActivateStateClone(Consts.SUMMON_EQUIPMENT_GUIDE, isActive));
        summonButton.onClick.AddListener(() => DataBaseManager.instance.Save(Consts.SUMMON_EQUIPMENT_GUIDE, true));
        summonButton.onClick.AddListener(ChangeToEquipmentGuide);

        StartCoroutine(Wait(Consts.SUMMON_EQUIPMENT_GUIDE));
    }

    protected override IEnumerator Wait(string str)
    {
        yield return null;
        GuideManager.instance.ToggleGuideWithBackgroundPanel(str, true);
    }

    private void ChangeToEquipmentGuide()
    {
        GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.SUMMON_EQUIPMENT_GUIDE, false);
        GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.HERO_TAP_TOUCHED_GUIDE, true);
        UIManager.instance.GetUIElement<UI_BottombarTutorialPanel>().AddEquipTutorial();
    }
}
