using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageInfoText;
    [SerializeField] private TextMeshProUGUI difficultyText;

    public void Init()
    {
        StageManager.instance.OnUpdateStageUI += UpdateUI;
    }

    public void UpdateActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void UpdateUI(int difficultyNum, int mainStageNum, string stageName)
    {
        UpdateStageInfoUI(difficultyNum, mainStageNum, stageName);
        UpdateDifficultyUI(difficultyNum, mainStageNum);
    }

    private void UpdateStageInfoUI(int difficultyNum, int mainStageNum, string stageName)
    {
        stageInfoText.text = $"{(difficultyNum - 1) * Consts.STAGE_DIVIDE_VALUE + mainStageNum}.{stageName}";
    }

    private void UpdateDifficultyUI(int difficultyNum, int mainStageNum)
    {
        difficultyText.text = $"난이도 {difficultyNum}.{mainStageNum}";
    }
}
