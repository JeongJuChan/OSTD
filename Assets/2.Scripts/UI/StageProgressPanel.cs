using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageProgressPanel : MonoBehaviour
{
    [SerializeField] private Image stageProgressBar;
    [SerializeField] private UI_CurrencyTextPanel ui_CurrencyTextPanel;

    [SerializeField] private Image[] rewardImages;

    private int checkPointMax;

    private float goalPosX;

    #region Initialize
    public void Init()
    {
        checkPointMax = ResourceManager.instance.stage.GetCheckPointMax();
        BoxManager.instance.boxMoveController.OnUpdateBoxPosX += UpdateProgressBar;
        ui_CurrencyTextPanel.Init();
        StageManager.instance.OnUpdateStageRewardUI += UpdateRewardImages;
        StageManager.instance.OnClearStage += Reset;
    }
    #endregion 

    private void Reset()
    {
        ResetRewardImages();
    }

    private void UpdateProgressBar(float currentBoxPosX)
    {
        stageProgressBar.fillAmount = currentBoxPosX / goalPosX;
    }

    // 더해지기 전에 추가
    private void UpdateRewardImages(int checkpointNum)
    {
        int j = 1;
        for (int i = 0; i < rewardImages.Length; i++)
        {
            if (i + j <= checkpointNum)
            {
                rewardImages[i].gameObject.SetActive(false);
                j++;
            }
            else
            {
                return;
            }
        }
    }

    private void ResetRewardImages()
    {
        for (int i = 0; i < rewardImages.Length; i++)
        {
            rewardImages[i].gameObject.SetActive(true);
        }
    }

    public void UpdateActiveState(bool isActive)
    {
        if(isActive)
        {
            goalPosX = StageManager.instance.GetGoalPosX();
        }
        gameObject.SetActive(isActive);
    }
}
