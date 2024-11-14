using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BattleMainPanelGuideController : MonoBehaviour
{
    [SerializeField] private GameObject movingBoxGuide;
    
    public void Init()
    {
        GuideManager.instance.AddGuidDict(Consts.MOVE_BOX_GUIDE, (isActive) => ChangeGuideActiveState(movingBoxGuide, isActive));
        movingBoxGuide.SetActive(false);
        GameManager.instance.OnStart += () => ChangeGuideActiveState(movingBoxGuide, false);
    }

    private void ChangeGuideActiveState(GameObject guide, bool isActive)
    {
        guide.SetActive(isActive);
    }

    
}
