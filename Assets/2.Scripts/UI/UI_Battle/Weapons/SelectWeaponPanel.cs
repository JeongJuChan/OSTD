using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectWeaponPanel : MonoBehaviour
{
    public void Reset()
    {
        UpdateActiveState(true);
    }

    public void UpdateActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
