using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using Unity.Collections;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [field: Header("Box Pool")]
    [field: SerializeField] public Box boxPrefab { get; private set; }
    private int boxCountMax;
    private List<Box> unReadiedBoxes = new List<Box>();
    [field: SerializeField] public List<Box> readiedBoxes { get; private set; } = new List<Box>();

    [field: Header("Box Event")]
    public event Action<bool> OnAddBoxState;
    public event Action<Transform, int> OnUpdateBoxPos;
    public event Action<int> OnUpdateHeroPos;
    public event Action<int> OnBoxCountChanged;


    private BoxResourceDataHandler boxResourceDataHandler;

    #region Initialize
    public void Init()
    {
        boxResourceDataHandler = ResourceManager.instance.box;

        boxCountMax = boxResourceDataHandler.GetBoxMaxCount();

        UI_Battle ui_Battle = UIManager.instance.GetUIElement<UI_Battle>();
        UI_UpgradeBoxButton ui_UpgradeBoxButton = Resources.Load<UI_UpgradeBoxButton>("UI/Box/UI_UpgradeBoxButton");
        UI_BoxWeaponPanel uI_UpgradeWeaponPanel = Resources.Load<UI_BoxWeaponPanel>("UI/Box/UI_BoxWeaponPanel");

        List<int> boxLevels = BoxManager.instance.boxLevels;

        for (int i = 0; i < boxCountMax; i++)
        {
            Box box = InitBox(i, ui_UpgradeBoxButton, uI_UpgradeWeaponPanel, ui_Battle.upgradeBoxButtonParent, ui_Battle.weaponPanelParent);
            RemoveBoxExcludeSave(box);
        }

        int boxLevelCount = boxLevels.Count;
        if (boxLevelCount > 0)
        {
            for (int i = 0; i < boxLevelCount; i++)
            {
                AddBoxExcludeSave();
                int level = boxLevels[i];
                Box box = readiedBoxes[i];
                box.UpdateBoxData(boxResourceDataHandler.GetBoxData(level));
                Sprite boxSprite = boxResourceDataHandler.GetBoxSprite(level);
                box.UpdateBoxSprite(boxSprite);
            }
        }
    }

    private Box InitBox(int i, UI_UpgradeBoxButton ui_UpgradeBoxButtonSource, UI_BoxWeaponPanel ui_weaponPanelSource, 
        Transform upgradeBoxButtonParent, Transform weaponPanelParent)
    {
        Box box = CreateBox();
        UI_UpgradeBoxButton ui_UpgradeBoxButton = Instantiate(ui_UpgradeBoxButtonSource, upgradeBoxButtonParent);
        UI_BoxWeaponPanel ui_WeaponPanel = Instantiate(ui_weaponPanelSource, weaponPanelParent);
        ui_UpgradeBoxButton.Init();
        ui_WeaponPanel.Init();
        box.OnLevelUpBox += BoxManager.instance.LevelUpBox;
        box.SetBoxUI(ui_UpgradeBoxButton, ui_WeaponPanel);
        int boxIndex = boxCountMax - 1 - i;
        box.SetIndex(boxIndex);
        if (boxIndex == 0)
        {
            UIManager.instance.GetUIElement<UI_Battle>().GetComponentInChildren<UI_MainBattleTutorialPanel>().AddWeaponGuide(ui_WeaponPanel, ui_WeaponPanel.buttons[0]);
        }
        return box;
    }
    #endregion

    #region BoxPooling
    public void AddBox()
    {
        AddBoxExcludeSave();

        BoxManager.instance.SaveBoxLevels();

        // TODO: HardCoding
        if (readiedBoxes.Count >= 4)
        {
            if (!DataBaseManager.instance.ContainsKey(Consts.MOVE_BOX_GUIDE))
            {
                GuideManager.instance.ToggleGuide(Consts.MOVE_BOX_GUIDE, true);
            }
        }

        if (!DataBaseManager.instance.ContainsKey(Consts.ADD_BOX_GUIDE))
        {
            GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ADD_BOX_GUIDE, false);
            DataBaseManager.instance.Save(Consts.ADD_BOX_GUIDE, true);
        }
    }

    private void AddBoxExcludeSave()
    {
        Box box = GetBox();
        box.gameObject.SetActive(true);
        OnUpdateBoxPos?.Invoke(box.transform, readiedBoxes.Count);
        box.UpdateBoxOffsetPos();
        box.Reset();
        

        unReadiedBoxes.Remove(box);
        readiedBoxes.Add(box);
        if (readiedBoxes.Count > BoxManager.instance.boxLevels.Count)
        {
            BoxManager.instance.AddBoxLevel();
        }

        WeaponManager.instance.AddWeaponData(box.index);

        if (readiedBoxes.Count >= boxCountMax)
        {
            OnAddBoxState?.Invoke(false);
        }

        OnUpdateHeroPos?.Invoke(readiedBoxes.Count);
        OnBoxCountChanged?.Invoke(readiedBoxes.Count);
    }

    public List<Box> GetReadiedBoxes()
    {
        return readiedBoxes;
    }

    public List<Box> GetUnreadiedBoxes()
    {
        return unReadiedBoxes;
    }

    private void RemoveBoxExcludeSave(Box box)
    {
        OnAddBoxState?.Invoke(true);
        box.gameObject.SetActive(false);

        int removeIndex = readiedBoxes.IndexOf(box);
        if (removeIndex != -1)
        {
            BoxManager.instance.RemoveBoxLevelByIndex(removeIndex);
        }
        box.UpdateBoxData(boxResourceDataHandler.GetBoxData(1));
        box.UpdateBoxSprite(boxResourceDataHandler.GetBoxSprite(1));

        if (removeIndex != -1)
        {
            readiedBoxes.Remove(box);
            for (int i = removeIndex; i < readiedBoxes.Count; i++)
            {
                OnUpdateBoxPos?.Invoke(readiedBoxes[i].transform, i);
            }
            WeaponManager.instance.RemoveWeaponByBoxIndex(box.index);
        }

        unReadiedBoxes.Add(box);
        OnUpdateHeroPos?.Invoke(readiedBoxes.Count);
        OnBoxCountChanged?.Invoke(readiedBoxes.Count);
    }

    public void RemoveBox(Box box)
    {
        RemoveBoxExcludeSave(box);
        BoxManager.instance.SaveBoxLevels();
    }

    public void RemoveAllActiveBoxes()
    {
        Queue<Box> boxes = new Queue<Box>(readiedBoxes);
        while (boxes.Count > 0)
        {
            Box box = boxes.Dequeue();
            RemoveBox(box);
        }
    }

    public Box GetBox(int boxIndex)
    {
        if (readiedBoxes.Count > boxIndex)
        {
            return readiedBoxes[boxIndex];
        }

        return default;
    }

    public bool GetIsBoxAddable()
    {
        return boxCountMax > readiedBoxes.Count;
    }

    private Box GetBox()
    {
        return unReadiedBoxes[unReadiedBoxes.Count - 1];
    }

    private Box CreateBox()
    {
        Box box = Instantiate(boxPrefab, transform);
        box.Init();
        UIManager.instance.GetUIElement<UI_Battle>().OnClickGameStart += box.SetInGame;
        return box;
    }
    #endregion
}
