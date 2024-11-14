using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Weapon : UI_BottomElement
{
       [field: SerializeField] public UI_UpgradeWeaponPanel ui_UpgradeWeaponPanel { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
        ui_UpgradeWeaponPanel.Init();
    }
}
