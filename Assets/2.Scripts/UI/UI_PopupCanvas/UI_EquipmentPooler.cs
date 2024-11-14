using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentPooler : UI_Pooler<UI_Equipment>
{
    public override void Init()
    {
        base.Init();
    }

    private void Reset()
    {
        ReturnAllUI();
    }

    public void AddReward(Sprite equipmentSprite, Sprite rankSprite)
    {
        UI_Equipment ui_Equipment = GetUI();
        ui_Equipment.UpdateUI(equipmentSprite, rankSprite);
    }
}
