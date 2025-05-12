using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBoxHandler
{
    private List<Box> inGameBoxes = new List<Box>();
    private Dictionary<WeaponType, int> weaponCountDict = new Dictionary<WeaponType, int>();

    private WeaponManager weaponManager;
    private SkillManager skillManager;

    public void Init()
    {
        GameManager.instance.OnReset += Reset;
        weaponManager = WeaponManager.instance;
        skillManager = SkillManager.instance;
    }

    private void Reset()
    {
        weaponCountDict.Clear();
    }

    public void AddBoxes(List<Box> boxes)
    {
        inGameBoxes = new List<Box>(boxes);

        for (int i = 0; i < boxes.Count; i++)
        {
            WeaponType weaponType = weaponManager.GetWeaponTypeByBoxIndex(i);

            if (weaponType == WeaponType.None)
            {
                continue;
            }
            
            if (!weaponCountDict.ContainsKey(weaponType))
            {
                weaponCountDict.Add(weaponType, 0);
            }

            weaponCountDict[weaponType]++;
        }
    }

    public void RemoveBoxWithWeapon(Box box)
    {
        Weapon weapon = weaponManager.GetWeaponByBoxIndex(box.index);
        if (weapon == null)
        {
            return;
        }

        weapon.gameObject.SetActive(false);
        WeaponType weaponType = weapon.GetWeaponType();
        if (weaponCountDict.ContainsKey(weaponType))
        {
            weaponCountDict[weaponType]--;
            skillManager.UpdateSkillDict(weaponType, weapon, false);
            if (weaponCountDict[weaponType] == 0)
            {
                skillManager.DeActiveSkillUIByWeaponType(weaponType);
            }
        }

        inGameBoxes.Remove(box);
    }

    public List<Box> GetInGameBoxes()
    {
        return inGameBoxes;
    }
}
