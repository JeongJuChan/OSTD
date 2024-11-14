using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class BoxManager : MonoBehaviourSingleton<BoxManager>
{
    [Header("Box UI")]
    private UI_AddBoxButton addBoxButton;

    [Header("Box Pos")]
    private Vector2 unitVec;

    [field: SerializeField] public BoxSpawner boxSpawner { get; private set; }
    [field: SerializeField] public BoxMoveController boxMoveController { get; private set; }

    private BoxResourceDataHandler boxResourceDataHandler;
    private BigInteger addingNewBoxCost;

    private CurrencyManager currencyManager;

    private Hero hero;

    [field: SerializeField] public Transform frameTransform { get; private set; }

    [SerializeField] private GameObject horizontalWallObject;
    [SerializeField] private GameObject verticalWallObject;

    public event Action OnSellBox;

    private InGameBoxHandler inGameBoxHandler;

    public event Action OnInGameBoxRemoved;

    public List<int> boxLevels { get; private set; } = new List<int>();

    [SerializeField] private BoxCollider2D truckCollider;

    [field: SerializeField] public Transform rocketCircleTrans;

    private void Update()
    {
        if (GameManager.instance.isGameState)
        {
            foreach (Box box in boxSpawner.GetReadiedBoxes())
            {
                box.UpdateInGamePositionX(frameTransform.position.x);
            }

            hero.UpdatePosX(frameTransform.position.x);
        }
    }

    
    #region Initialize
    public void Init()
    {
        currencyManager = CurrencyManager.instance;
        boxResourceDataHandler = ResourceManager.instance.box;

        addBoxButton = UIManager.instance.GetUIElement<UI_AddBoxButton>();
        addBoxButton.Init();
        addingNewBoxCost = boxResourceDataHandler.GetNewBoxCost();
        addBoxButton.UpdateCost(addingNewBoxCost);
        addBoxButton.AddButtonAction(() => UpdateGold(-addingNewBoxCost));

        Currency currency = currencyManager.GetCurrency(CurrencyType.Gold);
        BigInteger currencyValue = currency.GetCurrencyValue();
        currency.OnCurrencyChange += UpdateInteractableAddBoxButton;
        UIManager.instance.GetUIElement<UI_Bin>().OnRemoveBox += SellBox;

        AddEvents();
        unitVec = new Vector2(frameTransform.position.x, boxSpawner.boxPrefab.GetUnitSizeY());

        boxMoveController.Init();
        boxMoveController.SetBoxUnitSize(new Vector2(boxSpawner.boxPrefab.GetUnitSizeX(), boxSpawner.boxPrefab.GetUnitSizeY()));

        boxSpawner.OnUpdateHeroPos += UpdatePlayerPos;
        hero = HeroManager.instance.hero;
        boxSpawner.OnBoxCountChanged += boxMoveController.UpdateBoxSize;

        // UpdatePlayerPos(transform.position);
        GameManager.instance.OnStart += () => UpdateGameState(true);
        GameManager.instance.OnReset += () => UpdateGameState(false);
        hero.OnDead += () => UpdateWallActiveState(false);
        hero.OnDead += SetActiveFalseAllBoxes;

        WeaponManager.instance.OnUpdateUpgradeButtonState += UpdateUpgradeWeaponButtonInteractableState;
        WeaponManager.instance.OnUpdateLevelUpWeaponUI += UpdateUpgradeWeaponButtonUI;


        UpdateWallActiveState(false);

        inGameBoxHandler = new InGameBoxHandler();
        inGameBoxHandler.Init();

        WeaponManager.instance.OnUpdateInGameWeapons += UpdateInGameBoxesWeapon;

        StageManager.instance.OnClearStage += ResetBoxes;


        boxLevels = DataBaseManager.instance.Load(Consts.BOX_LEVELS, new List<int>());
        boxSpawner.Init();

        WeaponManager.instance.OnActivateUpgradeWeaponPanel += ActivateUpgradeWeaponPanelByBoxIndex;

        StageManager.instance.OnClearStage += () => UpdateTruckColliderEnableState(false);
        GameManager.instance.OnReset += () => UpdateTruckColliderEnableState(false);
        GameManager.instance.OnStart += () => UpdateTruckColliderEnableState(true);
        UpdateTruckColliderEnableState(false);

        UpdateInteractableAddBoxButton(currencyValue);
    }

    private void AddEvents()
    {
        addBoxButton.AddButtonAction(boxSpawner.AddBox);
        boxSpawner.OnAddBoxState += addBoxButton.UpdateInteractable;
        boxSpawner.OnUpdateBoxPos += SetBoxPos;
    }
    #endregion

    #region Box In Lobby
    public void TryUpdateBoxLastPos(Transform lastPosObjectTrans, int index)
    {
        Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        bool isGoingUp = inputPos.y - lastPosObjectTrans.position.y > 0;
        bool isGoingDown = lastPosObjectTrans.position.y - inputPos.y > 0;

        List<Box> boxes = boxSpawner.GetReadiedBoxes();

        if (isGoingUp && index == boxes.Count - 1)
        {
            return;
        }

        if (isGoingDown && index == 0)
        {
            return;
        }

        float diffY = lastPosObjectTrans.position.y - inputPos.y > 0 ?
            lastPosObjectTrans.position.y - inputPos.y : inputPos.y - lastPosObjectTrans.position.y;
        bool isSwitchable = diffY > Consts.HALF * unitVec.y;

        if (!isSwitchable)
        {
            return;
        }

        if (!DataBaseManager.instance.ContainsKey(Consts.MOVE_BOX_GUIDE))
        {
            GuideManager.instance.ToggleGuide(Consts.MOVE_BOX_GUIDE, false);
            DataBaseManager.instance.Save(Consts.MOVE_BOX_GUIDE, true);
        }

        if (isGoingUp)
        {
            SwitchBoxPos(lastPosObjectTrans, index + 1, boxes);
            SwitchBoxIndex(index, boxes, 1);
            SwitchBoxElement(index, boxes, 1);
        }
        else if (isGoingDown)
        {
            SwitchBoxPos(lastPosObjectTrans, index - 1, boxes);
            SwitchBoxIndex(index, boxes, - 1);
            SwitchBoxElement(index, boxes, - 1);
        }
    }

    public void LevelUpBox(Box box)
    {
        int level = box.GetBoxLevel();
        BigInteger currentCost = boxResourceDataHandler.GetBoxData(level).cost;
        UpdateGold(-currentCost);

        level++;
        BoxData boxData = boxResourceDataHandler.GetBoxData(level);
        box.UpdateBoxData(boxData);
        Sprite boxSprite = boxResourceDataHandler.GetBoxSprite(level);
        box.UpdateBoxSprite(boxSprite);
        SyncBoxLevel(box);
    }

    private void SwitchBoxIndex(int index, List<Box> boxes, int next)
    {
        int tempIndex = boxes[index].index;
        boxes[index].SetIndex(index + next);
        boxes[index + next].SetIndex(tempIndex);
    }

    private void SwitchBoxPos(Transform lastPosObjectTrans, int index, List<Box> boxes)
    {
        Vector2 pos = lastPosObjectTrans.position;
        lastPosObjectTrans.position = boxes[index].transform.position;
        boxes[index].UpdateBoxPos(pos);
        boxes[index].UpdateBoxUIPos();
    }

    private void SwitchBoxElement(int index, List<Box> boxes, int next)
    {
        Box tempBox = boxes[index];
        boxes[index] = boxes[index + next];
        boxes[index + next] = tempBox;

        int tempLevelIndex = boxLevels[index];
        boxLevels[index] = boxLevels[index + next];
        boxLevels[index + next] = tempLevelIndex;

        WeaponManager.instance.SwitchBoxIndex(index, index + next);
        SaveBoxLevels();
    }

    private void SetBoxPos(Transform boxTrans, int boxCount)
    {
        boxTrans.position = new Vector2(unitVec.x, frameTransform.position.y + unitVec.y * Consts.HALF + boxCount * unitVec.y);
    }

    private void UpdateInteractableAddBoxButton(BigInteger amount)
    {
        if (!addBoxButton.gameObject.activeInHierarchy)
        {
            return;
        }

        addBoxButton.UpdateInteractable(addingNewBoxCost <= amount && boxSpawner.GetIsBoxAddable());
    }

    private void UpdatePlayerPos(int boxCount)
    {
        Vector2 pos = frameTransform.position;
        pos.y += unitVec.y * boxCount;
        hero.UpdatePos(pos);
    }
    #endregion

    #region GameState
    private void UpdateGameState(bool isGameState)
    {
        if (isGameState)
        {
            UpdateBoxesUIActiveState(false);
            UpdateWallActiveState(true);
        }
        else
        {
            int boxCount = boxSpawner.readiedBoxes.Count;
            for (int i = 0; i < boxCount; i++)
            {
                Box box = boxSpawner.readiedBoxes[i];
                SetBoxPos(box.transform, i);
                box.Reset();

                Weapon weapon = WeaponManager.instance.GetWeaponByBoxIndex(i);
                if (weapon != null)
                {
                    weapon.gameObject.SetActive(true);
                }
            }

            UpdatePlayerPos(boxCount);
            UpdateWallActiveState(false);
        }
    }

    #endregion

    #region Update Boxes UI
    private void UpdateBoxesUIActiveState(bool isActive)
    {
        foreach (Box box in boxSpawner.GetReadiedBoxes())
        {
            box.UpdateBoxUIActiveState(isActive);
        }
    }
    #endregion

    #region Currency
    public void UpdateGold(BigInteger cost)
    {
        currencyManager.TryUpdateCurrency(CurrencyType.Gold, cost);
    }

    public void SellBox(Box box)
    {
        BigInteger totalGold = GetTotalGoldByBoxIndex(box.index);
        UpdateGold(totalGold);
        WeaponManager.instance.RemoveWeaponByBoxIndex(box.index);
        box.ResetWeaponPanel();
        boxSpawner.RemoveBox(box);
    }
    #endregion

    private void UpdateUpgradeWeaponButtonInteractableState(int boxIndex, BigInteger currency, BigInteger cost)
    {
        if (boxSpawner.readiedBoxes.Count > boxIndex)
        {
            Box box = boxSpawner.readiedBoxes[boxIndex];
            box.UpdateUpgradeWeaponInteractable(currency >= cost);
        }
    }

    private void UpdateUpgradeWeaponButtonUI(int boxIndex, int level, int upgradeBlockCount, BigInteger damage, BigInteger cost, BigInteger currentCurrency)
    {
        if (boxSpawner.readiedBoxes.Count > boxIndex)
        {
            Box box = boxSpawner.readiedBoxes[boxIndex];
            box.UpdateUpgradeWeaponUI(level, upgradeBlockCount, damage, cost, currentCurrency);
        }
    }

    public void AddWeapon(int boxIndex, Weapon weapon)
    {
        Box box = boxSpawner.GetBox(boxIndex);
        box.SetWeapon(weapon);
    }

    public float GetBoxSizeX()
    {
        return boxSpawner.boxPrefab.GetUnitSizeX();
    }

    public float GetBoxSizeY()
    {
        return boxSpawner.boxPrefab.GetUnitSizeY();
    }

    public BigInteger GetTotalGold()
    {
        BigInteger totalGold = 0;

        for (int i = 0; i < boxSpawner.readiedBoxes.Count; i++)
        {
            totalGold += GetTotalGoldByBoxIndex(i);
        }

        return totalGold;
    }

    public BigInteger GetTotalGoldByBoxIndex(int i)
    {
        BigInteger totalGold = 0;
        Box box = boxSpawner.readiedBoxes[i];

        if (box.isDead)
        {
            return totalGold;
        }

        int boxLevel = box.GetBoxLevel();
        totalGold += GetBoxTotalGoldByLevel(boxLevel);
        totalGold += WeaponManager.instance.GetWeaponTotalGold(i);
        return totalGold;
    }

    private BigInteger GetBoxTotalGoldByLevel(int level)
    {
        BigInteger totalGold = 0;

        for (int i = 0; i < level; i++)
        {
            totalGold += i == 0 ? boxResourceDataHandler.GetNewBoxCost() : boxResourceDataHandler.GetBoxData(i).cost;
        }

        return totalGold;
    }

    public void RemoveAllBoxes()
    {
        boxSpawner.RemoveAllActiveBoxes();
    }

    private void ResetBoxes()
    {
        BoxData boxData = boxResourceDataHandler.GetBoxData(1);
        Sprite boxSprite = boxResourceDataHandler.GetBoxSprite(1);

        List<Box> unreadiedBoxses = boxSpawner.GetUnreadiedBoxes();
        List<Box> readiedBoxes = boxSpawner.GetReadiedBoxes();


        for (int i = 0; i < unreadiedBoxses.Count; i++)
        {
            Box box = unreadiedBoxses[i];
            box.UpdateBoxData(boxData);
            box.UpdateBoxSprite(boxSprite);
            WeaponManager.instance.RemoveWeaponByBoxIndex(box.index);
            box.ResetWeaponPanel();
            box.SetIndex(unreadiedBoxses.Count - i - 1);
        }

        for (int i = 0; i < readiedBoxes.Count; i++)
        {
            Box box = readiedBoxes[i];
            box.UpdateBoxData(boxData);
            box.UpdateBoxSprite(boxSprite);
            WeaponManager.instance.RemoveWeaponByBoxIndex(box.index);
            box.ResetWeaponPanel();
            box.SetIndex(readiedBoxes.Count - i - 1);
        }

        WeaponManager.instance.ResetWeapon();
    }

    private void SetActiveFalseAllBoxes()
    {
        foreach (Box box in boxSpawner.readiedBoxes)
        {
            if (box.gameObject.activeInHierarchy)
            {
                box.Die(true);
            }
        }
    }

    private void UpdateWallActiveState(bool isActive)
    {
        horizontalWallObject.SetActive(isActive);
        verticalWallObject.SetActive(isActive);
    }

    public Transform GetTargetTransform()
    {
        List<Box> boxes = inGameBoxHandler.GetInGameBoxes();
        int boxCount = boxes.Count;
        if (boxCount == 0)
        {
            return hero.transform;
        }

        int index = UnityEngine.Random.Range(0, boxCount + 1);

        if (index < boxCount)
        {
            Box box = boxes[index];
            return box.transform;
        }
        else
        {
            return hero.transform; 
        }
    }

    public void RemoveInGameBox(Box box)
    {
        inGameBoxHandler.RemoveBoxWithWeapon(box);
        OnInGameBoxRemoved?.Invoke();
    }

    private void UpdateInGameBoxesWeapon()
    {
        inGameBoxHandler.AddBoxes(boxSpawner.readiedBoxes);
    }

    public void SaveBoxLevels()
    {
        DataBaseManager.instance.Save(Consts.BOX_LEVELS, boxLevels);
    }

    public void AddBoxLevel()
    {
        boxLevels.Add(1);
    }

    public void RemoveBoxLevelByIndex(int index)
    {
        if (boxLevels.Count > index)
        {
            boxLevels.RemoveAt(index);
        }
    }

    private void SyncBoxLevel(Box box)
    {
        boxLevels[box.index] = box.GetBoxLevel();
        SaveBoxLevels();
    }

    public void ActivateUpgradeWeaponPanelByBoxIndex(int boxIndex)
    {
        boxSpawner.readiedBoxes[boxIndex].ActivateUpgradeWeaponPanel();
    }

    public void UpdateBoxesDragable(bool isDragable)
    {
        foreach (Box box in boxSpawner.readiedBoxes)
        {
            box.ChangeDragableState(isDragable);
        }
    }

    private void UpdateTruckColliderEnableState(bool isEnable)
    {
        truckCollider.enabled = isEnable;
    }

    public void InitBoxWeapons()
    {
        foreach (Box box in boxSpawner.readiedBoxes)
        {
            box.ui_WeaponPanel.UpdateWeaponChoiceButtonsInteractable(currencyManager.GetCurrencyValue(CurrencyType.Gold));
        }
        
    }
}
