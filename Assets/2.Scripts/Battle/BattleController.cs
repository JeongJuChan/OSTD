using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    private FieldMonsterController fieldMonsterController;
    //private CastleClan castleClan;

    // private MonsterSpawner monsterSpawner;

    private TargetHandler targetHandler;

    #region UnityMethods


    private void Start()
    {
        AddEvents();
    }
    #endregion
    #region InitMethods
    public void Init()
    {
        // monsterSpawner = FindAnyObjectByType<MonsterSpawner>();
        if (targetHandler == null)
        {
            targetHandler = new TargetHandler();
        }
        //castleClan.GetComponentInParent<Castle>().OnDead += targetHandler.RemoveAllTarget;
        // FindAnyObjectByType<StageController>().OnClearRoutineStage += targetHandler.RemoveAllTarget;
        // FindAnyObjectByType<PlaceEventSwitcher>().OnClearMonsters += targetHandler.RemoveAllTarget;
        //FindAnyObjectByType<SkillManager>().OnGetActiveTargets += targetHandler.GetActiveTargets;
        //MainPanelSkillUIController mainPanelSkillUIController = FindAnyObjectByType<MainPanelSkillUIController>(FindObjectsInactive.Include);
        //mainPanelSkillUIController.OnGetActiveTargetsExist += targetHandler.GetAreActiveTargetsExist;
        //mainPanelSkillUIController.Init();
        //castleClan.SetHeroesGetTargetFunc(targetHandler.GetActiveTargets);
        // monsterSpawner.OnSpawned += targetHandler.AddInActiveTarget;


    }
    #endregion
    #region EventHandleMethods
    private void AddEvents()
    {
        if (targetHandler == null)
        {
            targetHandler = new TargetHandler();
        }
        fieldMonsterController.OnTargetAdded += targetHandler.AddActiveTarget;
        fieldMonsterController.OnTargetRemoved += targetHandler.RemoveActiveTarget;
        targetHandler.OnActiveTargetStateChanged += OnActiveTargetStateChanged;
    }

    #endregion
    #region ActiveTargetMethods
    private void OnActiveTargetStateChanged()
    {
        
    }
    #endregion

#if UNITY_EDITOR
    public void RemoveAllTarget()
    {
        targetHandler.RemoveAllTarget();
    }
#endif
}
