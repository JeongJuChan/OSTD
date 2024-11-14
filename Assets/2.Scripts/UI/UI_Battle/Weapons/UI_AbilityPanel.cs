using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AbilityPanel : UI_Popup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private CurrentAbilityPanel currentAbilityPanel;
    [SerializeField] private TextMeshProUGUI applyInfoText;
    [SerializeField] private AbilityChoicePanel abilityChoicePanel;
    [field: SerializeField] public UI_Button rerollButton { get; private set; }

    public override void Init()
    {
        rerollButton.Init();
        rerollButton.AddButtonAction(rerollButton.CloseUI);
        abilityChoicePanel.Init(CloseUI);
        CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        rerollButton.OpenUI();
    }

    public void UpdateCurrentHavingAbilityUI(string weaponTypeKR, Sprite weaponIcon, List<Sprite> abilitySprites)
    {
        titleText.text = $"{weaponTypeKR} 능력";
        currentAbilityPanel.UpdateCurrentHavingAbilityPanel(weaponIcon, abilitySprites);
    }

    public void UpdateChooseableAbilityUI(int index, string weaponTypeKR, Sprite abilitySprite, string abilityDescription)
    {
        abilityChoicePanel.UpdateAbilityButtonPanel(index, abilitySprite, weaponTypeKR, abilityDescription);
    }
}
