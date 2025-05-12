using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillButtonPanel : UI_Base
{
    [SerializeField] private UI_SkillButton[] ui_SkillButtons;
    [SerializeField] private CanvasGroup[] skillCanvasGroup;
    [SerializeField] private Guide[] guides;

    public override void Init()
    {
        SkillManager.instance.OnUpdateSkillButtonInteractable += UpdateSkillButtonInteractable;
        SkillManager.instance.OnUpdateSkillCostUI += UpdateSkillCostUI;
        SkillManager.instance.OnUpdateUsingSkillUI += UpdateSkillSprite;
        SkillManager.instance.OnUpdateSkillUIActiveState += UpdateActiveState;

        for (int i = 0; i < ui_SkillButtons.Length; i++)
        {
            int index = i;
            ui_SkillButtons[i].Init();
            UpdateSkillButtonInteractable(i, false);
            ui_SkillButtons[i].AddButtonAction(() => SkillManager.instance.UseSkill(index));
            ui_SkillButtons[i].AddButtonAction(() => GuideOff(index));
            GuideManager.instance.AddGuidDict(SkillManager.instance.guideStrArr[index], guides[index].ChangeActiveState);
        }

        foreach (var item in guides)
        {
            item.ChangeActiveState(false);
        }
    }

    private void UpdateSkillButtonInteractable(int index, bool isInteractable)
    {
        ui_SkillButtons[index].UpdateInteractable(isInteractable);
    }

    private void UpdateSkillCostUI(int index, int cost)
    {
        ui_SkillButtons[index].UpdateCostText(cost);
    }

    private void UpdateSkillSprite(int index, SkillType skillType)
    {
        ui_SkillButtons[index].UpdateSprite(ResourceManager.instance.skill.GetSkillSprite(skillType));
    }

    private void UpdateActiveState(int index, bool isActive)
    {
        if (isActive)
        {
            // ui_SkillButtons[index].OpenUI();
            if (index != 0)
            {
                skillCanvasGroup[index - 1].alpha = 1;
            }
        }
        else
        {
            // ui_SkillButtons[index].CloseUI();
            if (index != 0)
            {
                skillCanvasGroup[index - 1].alpha = 0;
            }
        }
    }

    private void GuideOff(int index)
    {
        string guideStr = SkillManager.instance.guideStrArr[index];
        if (!DataBaseManager.instance.ContainsKey(guideStr))
        {
            DataBaseManager.instance.Save(guideStr, true);
            GuideManager.instance.ToggleGuide(guideStr, false);
        }
    }
}
