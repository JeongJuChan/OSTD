using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Battle : UI_BottomElement
{
    [SerializeField] private StageInfoPanel stageInfoPanel;
    [SerializeField] private StageProgressPanel stageProgressPanel;
    [SerializeField] private CurrencyTextPanel goldTextPanel;
    [SerializeField] private CurrencyTextPanel gemTextPanel;
    [SerializeField] private UI_Bin ui_Bin;
    [SerializeField] private Button startButton;
    [SerializeField] private UI_SkillButtonPanel ui_skillButtonPanel;
    [SerializeField] private UI_EnergyUpgradePanel ui_EnergyUpgradePanel;
    [SerializeField] private UI_EnergyAdsPanel ui_EnergyAdsPanel;
    [SerializeField] private UI_BattleMainPanelGuideController ui_BattleMainPanelGuideController;

    [SerializeField] private GameObject mainBottomPanel;
    [SerializeField] private GameObject lobbyButtonPanel;
    [SerializeField] private GameObject inGameButtonPanel;

    [SerializeField] private UI_BonusGoldPanel ui_BonusGoldPanel;

    [field: SerializeField] public UI_AbilityPanel ui_AbilityPanel { get; private set; }

    [field: Header("BoxUI Parent")]
    [field: SerializeField] public Transform upgradeBoxButtonParent { get; private set; }
    [field: SerializeField] public Transform weaponPanelParent { get; private set; }

    public event Action OnClickGameStart;

    #region Initialize
    public override void Initialize()
    {
        base.Initialize();
        stageInfoPanel.Init();
        stageProgressPanel.Init();
        goldTextPanel.Init();
        gemTextPanel.Init();
        ui_Bin.Init();
        openUI?.Invoke();
        startButton.onClick.AddListener(ClickStartButton);
        ui_skillButtonPanel.Init();
        UpdateButtonPanelActiveState(false);

        ui_BonusGoldPanel.Init();
        ui_EnergyUpgradePanel.Init();
        ui_EnergyAdsPanel.Init();
        ui_BattleMainPanelGuideController.Init();

        GameManager.instance.OnReset += () => UpdateButtonPanelActiveState(false);
        OnClickGameStart += () => HeroManager.instance.hero.UpdateKinematicState(false);
        GameManager.instance.OnStart += () => UpdateGoldTextActiveState(false);
        GameManager.instance.OnReset += () => UpdateGoldTextActiveState(true);
    }

    #endregion

    #region GameState
    private void ClickStartButton()
    {
        OnClickGameStart?.Invoke();
        UpdateButtonPanelActiveState(true);
    }

    private void UpdateButtonPanelActiveState(bool isGameState)
    {
        stageProgressPanel.UpdateActiveState(isGameState);
        inGameButtonPanel.SetActive(isGameState);

        lobbyButtonPanel.SetActive(!isGameState);
        mainBottomPanel.SetActive(!isGameState);
        stageInfoPanel.UpdateActiveState(!isGameState);
    }

    public override void OpenUI()
    {
        base.OpenUI();
        BoxManager.instance.UpdateBoxesDragable(true);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        BoxManager.instance.UpdateBoxesDragable(false);
    }

    private void UpdateGoldTextActiveState(bool isActive)
    {
        goldTextPanel.gameObject.SetActive(isActive);
    }
    #endregion
}
