using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopupCanvas : UI_Base
{
    [SerializeField] private UI_LosePanel ui_LosePanel;
    [SerializeField] private UI_RewardPanel ui_RewardPanel;
    [SerializeField] private UI_StageClearPanel ui_StageClearPopup;

    public override void Init()
    {
        base.Init();
        ui_LosePanel.Init();
        ui_RewardPanel.Init();
        ui_StageClearPopup.Init();
    }
}
