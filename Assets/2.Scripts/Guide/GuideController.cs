using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
    [SerializeField] protected GameObject[] guides;
    [SerializeField] protected QuestType[] questTypes;
    [SerializeField] protected EventQuestType[] eventQuestTypes;
    [SerializeField] protected int[] guideNumbers;

    protected bool[] isGuidePassed;

    protected QuestManager questManager;
    //protected GuideManager guideManager;

    public void Initialize()
    {
        // SetManagers();
        // AddCallbacks();
        // TODO : 주석 풀기
        //InitGuides();
    }

    // private void SetManagers()
    // {
    //     questManager = QuestManager.instance;
    // }

    protected virtual void AddCallbacks()
    {
        // questManager.OnQuestStarted += TryActivateGuide;
        // questManager.OnQuestAchieved += TryDeactivateGuide;
    }

    private void InitGuides()
    {
        for (int i = 0; i < guides.Length; i++)
        {
            int index = i;
            guides[index].SetActive(false);
        }

        // QuestType curType = questManager.GetQuestType();

        // if (curType != QuestType.Event)
        // {
        //     for (int i = 0; i < questTypes.Length; i++)
        //     {
        //         if (curType != questTypes[i]) continue;

        //         if (questManager.IsQuestOnNumber(questTypes[i])) guides[i].SetActive(true);
        //     }
        // }
        // else
        // {
        //     EventQuestType curEventType = questManager.GetEventQuestType();

        //     for (int i = 0; i < eventQuestTypes.Length; i++)
        //     {
        //         if (questTypes[i] != QuestType.Event) continue;
        //         if (curEventType != eventQuestTypes[i]) continue;

        //         if (questManager.IsQuestOnNumber(eventQuestTypes[i]) && questManager.CheckQuestType(eventQuestTypes[i], guideNumbers[i]))
        //         {
        //             guides[i].SetActive(true);
        //         }
        //     }
        // }
    }

    protected virtual void TryActivateGuide(bool isFirstTime)
    {

        // for (int i = 0; i < guides.Length; i++)
        // {
        //     if (guides[i].activeSelf) continue;

        //     if (questTypes[i] != QuestType.Event)
        //     {
        //         if (questManager.CheckQuestType(questTypes[i]))
        //         {
        //             guides[i].SetActive(true);
        //         }
        //     }
        //     else if (questManager.CheckQuestType(eventQuestTypes[i], guideNumbers[i]))
        //     {
        //         guides[i].SetActive(true);
        //     }
        // }
    }

    private void TryDeactivateGuide(bool isFirstTime)
    {
        // for (int i = 0; i < guides.Length; i++)
        // {
        //     if (!guides[i].activeSelf) continue;

        //     if (questTypes[i] != QuestType.Event)
        //     {
        //         if (questManager.CheckQuestType(questTypes[i])) guides[i].SetActive(false);
        //     }
        //     else if (questManager.CheckQuestType(eventQuestTypes[i], guideNumbers[i]))
        //     {
        //         guides[i].SetActive(false);
        //     }
        // }
    }

    public virtual void RemoveCallbacks()
    {
        // if (questManager == null) return;
        // questManager.OnQuestStarted -= TryActivateGuide;
        // questManager.OnQuestAchieved -= TryDeactivateGuide;
    }

    private void OnDestroy()
    {
        RemoveCallbacks();
    }
}