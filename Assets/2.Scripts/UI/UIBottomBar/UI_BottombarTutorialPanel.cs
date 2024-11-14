using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BottombarTutorialPanel : UI_MainBattleTutorialPanel
{
    [SerializeField] private BottomBarController bottomBarController;

    private RectTransform heroGuideClone;
    private Button heroButton;

    private RectTransform shopGuideClone;
    private Button shopButton;


    protected override void Start()
    {
        // InitDict();
    }

    public override void Init()
    {
        currentTutorialName = DataBaseManager.instance.Load(Consts.CURRENT_TUTORIAL_NAME, "");
        closeButtonWaitForSeconds = CoroutineUtility.GetWaitForSeconds(closeButtonWaitTime);

        foreach (var rect in guideRects)
        {
            rect.gameObject.SetActive(false);
        }

        StartCoroutine(WaitSeconds());

        foreach (var item in GuideManager.instance.tutorialDict.Values)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in GuideManager.instance.guidesDict.Values)
        {
            item.SetActive(false);
        }

        backgroundPanel.SetActive(false);
    }

    private IEnumerator WaitSeconds()
    {
        InitDict();
        LayoutRebuilder.ForceRebuildLayoutImmediate(bottomBarController.GetComponent<RectTransform>());
        yield return new WaitForEndOfFrame();
        heroGuideClone.position = Clones[0].position;
        heroGuideClone.sizeDelta = Clones[0].sizeDelta;
        shopGuideClone.position = Clones[1].position;
        shopGuideClone.sizeDelta = Clones[1].sizeDelta;
    }

    protected override void InitDict()
    {
        if (heroGuideClone == null)
        {
            heroGuideClone = Instantiate(Clones[0], Clones[0].position, Quaternion.identity, tutorialClonePanel);
            heroGuideClone.sizeDelta = Clones[0].sizeDelta;
            heroButton = heroGuideClone.GetComponent<Button>();
            heroButton.onClick.AddListener(() => bottomBarController.ToggleCanvas(bottomBarController.uiElements[1], false));
            heroButton.onClick.AddListener(() => DataBaseManager.instance.Save(Consts.HERO_TAP_TOUCHED_GUIDE, true));

            GuideManager.instance.tutorialDict.Add(Consts.HERO_TAP_TOUCHED_GUIDE, heroGuideClone);
            GuideManager.instance.AddGuidDict(Consts.HERO_TAP_TOUCHED_GUIDE, (isActive) => ChangeActivateStateClone(Consts.HERO_TAP_TOUCHED_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.HERO_TAP_TOUCHED_GUIDE, guideRects[0]);
        }
        
        if (shopGuideClone == null)
        {
            shopGuideClone = Instantiate(Clones[1], Clones[1].position, Quaternion.identity, tutorialClonePanel);
            shopGuideClone.sizeDelta = Clones[1].sizeDelta;
            shopButton = shopGuideClone.GetComponent<Button>();
            shopButton.onClick.AddListener(() => bottomBarController.ToggleCanvas(bottomBarController.uiElements[0], false));

            GuideManager.instance.tutorialDict.Add(Consts.SHOP_TAP_TOUCHED_GUIDE, shopGuideClone);
            GuideManager.instance.AddGuidDict(Consts.SHOP_TAP_TOUCHED_GUIDE, (isActive) => ChangeActivateStateClone(Consts.SHOP_TAP_TOUCHED_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.SHOP_TAP_TOUCHED_GUIDE, guideRects[1]);
        }
    }

    public void AddEquipTutorial()
    {
        heroButton.onClick.AddListener(UIManager.instance.GetUIElement<UI_HeroTutorialPanel>().InitAutoEquip);
    }
}
