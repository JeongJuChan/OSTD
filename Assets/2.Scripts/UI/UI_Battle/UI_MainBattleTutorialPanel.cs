using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainBattleTutorialPanel : UI_Base
{
    [SerializeField] protected RectTransform tutorialClonePanel;
    [SerializeField] protected RectTransform[] Clones;
    [SerializeField] protected GameObject backgroundPanel;
    [SerializeField] protected GameObject[] guideRects;

    [SerializeField] private RectTransform[] guidesTargetRect;

    [SerializeField] protected float closeButtonWaitTime = 2f;
    protected WaitForSeconds closeButtonWaitForSeconds;

    protected string currentTutorialName;

    protected Coroutine preCoroutine;

    private RectTransform addBoxRect;
    private UI_AddBoxButton ui_AddBoxButton;
    private RectTransform upgradeEnergyButton;
    private UI_EnergyUpgradePanel ui_UpgradeBoxButton;
    private UI_WeaponChoiceButton ui_WeaponChoiceButtonClone;

    private Queue<(Func<bool>, Action)> tutorialQueue = new Queue<(Func<bool>, Action)>();

    private const int PLAY_COUNT_FOR_GIVING_GOLD = 2;

    private CurrencyType currencyType = CurrencyType.Gold;

    private int addWeaponGoldAmount = 90;

    protected virtual void Start()
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

        backgroundPanel.SetActive(false);


        if (!DataBaseManager.instance.ContainsKey(Consts.ADD_BOX_GUIDE))
        {
            tutorialQueue.Enqueue((() => CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >= ResourceManager.instance.box.GetNewBoxCost(),
                () => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ADD_BOX_GUIDE, true)));
        }

        if (!DataBaseManager.instance.ContainsKey(Consts.ENERGY_ENFORCE_GUIDE))
        {
            tutorialQueue.Enqueue((() => CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >=
                ResourceManager.instance.energy.GetEnergyData(SkillManager.instance.energyLevel).cost,
                () => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ENERGY_ENFORCE_GUIDE, true)));
        }

        if (!DataBaseManager.instance.ContainsKey(Consts.ADD_WEAPON_GUIDE))
        {
            tutorialQueue.Enqueue((() => CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >= addWeaponGoldAmount,
                () => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ADD_WEAPON_GUIDE, true)));
        }

        if (!DataBaseManager.instance.ContainsKey(Consts.FirstOpen))
        {
            guideRects[3].SetActive(true);
            GameManager.instance.OnStart += () => guideRects[3].SetActive(false);
            GameManager.instance.OnStart += () => DataBaseManager.instance.Save(Consts.FirstOpen, true);
        }

        if (!DataBaseManager.instance.ContainsKey(Consts.ENERGY_ENFORCE_GUIDE))
        {
            CurrencyManager.instance.GetCurrency(CurrencyType.Gold).OnCurrencyChange += CheckTutorial;
        }

        GameManager.instance.OnReset += () => CheckTutorial(CurrencyManager.instance.GetCurrencyValue(currencyType));
    }

    private void CheckTutorial(BigInteger amount)
    {
        if (tutorialQueue.Count != 0 && !GuideManager.instance.isPopupShowed)
        {
            if (DataBaseManager.instance.Load(Consts.PLAY_COUNT, 0) == PLAY_COUNT_FOR_GIVING_GOLD)
            {
                BigInteger totalGold = 0;
                BigInteger energyCost = ResourceManager.instance.energy.GetEnergyData(SkillManager.instance.energyLevel).cost;

                if (!DataBaseManager.instance.ContainsKey(Consts.ADD_BOX_GUIDE) && amount < addWeaponGoldAmount)
                {
                    CurrencyManager.instance.TryUpdateCurrency(currencyType, addWeaponGoldAmount + energyCost +
                        ResourceManager.instance.box.GetNewBoxCost() - amount);
                }
                else if (!DataBaseManager.instance.ContainsKey(Consts.ENERGY_ENFORCE_GUIDE) && amount < energyCost)
                {
                    CurrencyManager.instance.TryUpdateCurrency(currencyType, addWeaponGoldAmount + energyCost - amount);
                }
                else if (!DataBaseManager.instance.ContainsKey(Consts.ADD_WEAPON_GUIDE) && amount < addWeaponGoldAmount)
                {
                    CurrencyManager.instance.TryUpdateCurrency(currencyType, amount - addWeaponGoldAmount);
                }
            }

            if (tutorialQueue.Peek().Item1.Invoke())
            {
                tutorialQueue.Dequeue().Item2?.Invoke();
            }
        }

        // if (tutorialQueue.Count == 0)
        // {
        //     if (DataBaseManager.instance.ContainsKey(Consts.ENERGY_ENFORCE_GUIDE) && !DataBaseManager.instance.ContainsKey(Consts.ADD_WEAPON_GUIDE))
        //     {
        //         if (CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >= 90 && DataBaseManager.instance.ContainsKey(Consts.ADD_BOX_GUIDE))
        //         {
        //             // GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ADD_WEAPON_GUIDE, true);
        //         }
        //         else
        //         {
        //             tutorialQueue.Enqueue((() => CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >= 90,
        //                 () => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ADD_WEAPON_GUIDE, true)));
        //         }
        //     }
        // }
        // else
        // {
        //     if (tutorialQueue.Peek().Item1.Invoke())
        //     {
        //         tutorialQueue.Dequeue().Item2?.Invoke();
        //     }
        // }

    }

    protected virtual void InitDict()
    {
        if (addBoxRect == null)
        {
            addBoxRect = Instantiate(Clones[0], Clones[0].position, Quaternion.identity, transform);
            ui_AddBoxButton = addBoxRect.GetComponent<UI_AddBoxButton>();
            ui_AddBoxButton.Init();
            ui_AddBoxButton.AddButtonAction(BoxManager.instance.boxSpawner.AddBox);
            ui_AddBoxButton.UpdateInteractable(true);
            ui_AddBoxButton.AddButtonAction(() => 
                CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, -ResourceManager.instance.box.GetNewBoxCost()));

            upgradeEnergyButton = Instantiate(Clones[1], Clones[1].position, Quaternion.identity, transform);
            ui_UpgradeBoxButton = upgradeEnergyButton.GetComponent<UI_EnergyUpgradePanel>();
            ui_UpgradeBoxButton.Init();
            ui_UpgradeBoxButton.UpdateButtonInteractableState(true);

            GuideManager.instance.AddGuidDict(Consts.ADD_BOX_GUIDE, (isActive) => ChangeActivateStateClone(Consts.ADD_BOX_GUIDE, isActive));
            GuideManager.instance.AddGuidDict(Consts.ENERGY_ENFORCE_GUIDE, (isActive) => UpdateEnergyEnforceState(isActive));

            GuideManager.instance.tutorialDict.Add(Consts.ADD_BOX_GUIDE, addBoxRect);
            GuideManager.instance.tutorialDict.Add(Consts.ENERGY_ENFORCE_GUIDE, upgradeEnergyButton);

            GuideManager.instance.guidesDict.Add(Consts.ADD_BOX_GUIDE, guideRects[0]);
            GuideManager.instance.guidesDict.Add(Consts.ENERGY_ENFORCE_GUIDE, guideRects[1]);
        }
    }

    private void UpdateEnergyEnforceState(bool isActive)
    {
        if (isActive)
        {
            if (DataBaseManager.instance.ContainsKey(Consts.ENERGY_ENFORCE_GUIDE))
            {
                if (tutorialQueue.Count == 0)
                {
                    tutorialQueue.Enqueue((() => CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >= addWeaponGoldAmount,
                        () => GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ADD_WEAPON_GUIDE, true)));
                }
                return;
            }

            BigInteger cost = ResourceManager.instance.energy.GetEnergyData(SkillManager.instance.energyLevel).cost;
            if (CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >= cost)
            {
                ChangeActivateStateClone(Consts.ENERGY_ENFORCE_GUIDE, isActive);
            }
        }
        else
        {
            ChangeActivateStateClone(Consts.ENERGY_ENFORCE_GUIDE, isActive);
        }
    }

    protected void ChangeActivateStateClone(string tutorialName, bool isActive)
    {
        if (preCoroutine != null)
        {
            StopCoroutine(preCoroutine);
        }

        if (GuideManager.instance.tutorialDict.ContainsKey(tutorialName))
        {
            backgroundPanel.SetActive(isActive);
            GuideManager.instance.tutorialDict[tutorialName].gameObject.SetActive(isActive);
            GuideManager.instance.guidesDict[tutorialName].SetActive(isActive);
            if (isActive)
            {
                currentTutorialName = tutorialName;
                if (!gameObject.activeInHierarchy)
                {
                    return;
                }
            }
        }
    }

    public void CloseCurrentObject()
    {
        if (currentTutorialName == null || !GuideManager.instance.tutorialDict.ContainsKey(currentTutorialName))
        {
            return;
        }

        if (preCoroutine != null)
        {
            StopCoroutine(preCoroutine);
        }

        GuideManager.instance.tutorialDict[currentTutorialName].gameObject.SetActive(false);
        backgroundPanel.SetActive(false);
        GuideManager.instance.guidesDict[currentTutorialName].SetActive(false);
        GuideManager.instance.ToggleGuideWithBackgroundPanel(currentTutorialName, false);
        if (DataBaseManager.instance.ContainsKey(Consts.HERO_TAP_TOUCHED_GUIDE_START) && !DataBaseManager.instance.ContainsKey(Consts.SHOP_TAP_TOUCHED_GUIDE))
        {
            GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.SHOP_TAP_TOUCHED_GUIDE, true);
        }
        // DataBaseManager.instance.Save(currentTutorialName, true);
    }

    public void AddWeaponGuide(UI_BoxWeaponPanel ui_WeaponPanel, UI_WeaponChoiceButton ui_WeaponChoiceButton)
    {
        if (GuideManager.instance.tutorialDict.ContainsKey(Consts.ADD_WEAPON_GUIDE) && !DataBaseManager.instance.ContainsKey(Consts.ADD_BOX_GUIDE))
        {
            return;
        }

        StartCoroutine(CoWaitForAddWeapon(ui_WeaponPanel, ui_WeaponChoiceButton));
    }

    private IEnumerator CoWaitForAddWeapon(UI_BoxWeaponPanel ui_WeaponPanel, UI_WeaponChoiceButton ui_WeaponChoiceButton)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(ui_WeaponPanel.GetComponent<RectTransform>());
        yield return new WaitForEndOfFrame();

        if (ui_WeaponChoiceButtonClone == null)
        {
            ui_WeaponChoiceButtonClone = Instantiate(ui_WeaponChoiceButton, ui_WeaponChoiceButton.GetComponent<RectTransform>().position, Quaternion.identity, transform);
            ui_WeaponChoiceButtonClone.Init();

            ui_WeaponChoiceButtonClone.AddButtonAction(() => ui_WeaponPanel.SetBoxWeapon(0));
            ui_WeaponChoiceButtonClone.AddButtonAction(() => ui_WeaponPanel.ChangeCurrencyByChoosingWeapon(0));
            ui_WeaponChoiceButtonClone.gameObject.SetActive(false);
            GuideManager.instance.tutorialDict.Add(Consts.ADD_WEAPON_GUIDE, ui_WeaponChoiceButtonClone.GetComponent<RectTransform>());
            GuideManager.instance.AddGuidDict(Consts.ADD_WEAPON_GUIDE, (isActive) => UpdateAddWeaponUIState(Consts.ADD_WEAPON_GUIDE, isActive));
            GuideManager.instance.guidesDict.Add(Consts.ADD_WEAPON_GUIDE, guideRects[2]);
        }
    }

    private void UpdateAddWeaponUIState(string key, bool isActive)
    {
        if (!DataBaseManager.instance.ContainsKey(Consts.ADD_BOX_GUIDE))
        {
            return;
        }

        if (isActive)
        {
            Box box = BoxManager.instance.boxSpawner.GetBox(0);
            ui_WeaponChoiceButtonClone.transform.position = box.ui_WeaponPanel.buttons[0].transform.position;
        }

        ChangeActivateStateClone(Consts.ADD_WEAPON_GUIDE, isActive);
    }

    protected virtual IEnumerator Wait(string str)
    {
        yield return null;
        GuideManager.instance.ToggleGuideWithBackgroundPanel(str, true);
    }
}
