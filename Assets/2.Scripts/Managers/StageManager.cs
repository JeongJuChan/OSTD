using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using Firebase.Analytics;

public class StageManager : MonoBehaviourSingleton<StageManager>
{
    private Dictionary<MonsterType, int> monsterTypeDict;
    private Dictionary<MonsterType, MonsterSpawnTimer> spawnTimerDict;

    [Header("Layers")]
    [SerializeField] private Transform[] layerTransforms;
    [SerializeField] private string[] layerMaskStrArr;
    [SerializeField] private int[] layerMasks;
    private float[] groundsPosY;

    private int difficultyNum = 1;
    public int mainStageNum { get; private set; } = 1;
    private int checkpointNum = 1;

    [SerializeField] private float monsterSpawnPosXMod = 3f;

    private List<MonsterType> monsterTypes;

    [SerializeField] private float monsterArriveDuration = 3f;
    [SerializeField] private float towerArriveDuration = 14f;
    private float monsterOffsetPosX;

    private StageData currentStageData;

    private float boxMoverUnitPos;

    [SerializeField] private MonsterBasement monsterBasementPrefab;
    [SerializeField] private MonsterBasement[] monsterBasements;
    [SerializeField] private int hpUnit = 100;

    public event Action<int, int, string> OnUpdateStageUI;
    public event Action<int, int> OnUpdateWeaponTypes;
    public event Action<int> OnUpdateStageRewardUI;
    public event Action<int, int> OnUpdateReward;
    public event Action OnClearStage;
    public event Action<BigInteger, BigInteger> OnUpdateClearCurrencyUI;
    public event Action<Sprite, Sprite> OnUpdateBackgroundSprite;

    private GameManager gameManager;

    public StageInGameDataHandler stageInGameDataHandler { get; private set; }

    [Header("Monster Kill Gold")]
    private int monsterKillGold;

    private int checkPointMax;

    public bool isClearPopupShowed { get; private set; } = false;

    private BoxMoveController boxMoveController;

    private MonsterObjectPooler monsterObjectPooler;

    private int preCheckpointNum = 0;

    private MonsterResourceHandler monsterResourceHandler;

    private float targetInRangePosX;

    private float goalPosX;

    private StageResourceDataHandler stageResourceDataHandler;

    private float stageElapsedTime;
    private float secondMonsterInstIntervalMultiplication;
    private float thirdMonsterInstIntervalMultiplication;
    private float firstWaveDuration;
    private float secondWaveDuration;
    private bool isFirstWaveFinished;
    private bool isSecondWaveFinished;

    public event Action<int, bool> OnUnlockShop;

    [SerializeField] private int stageRepeatValue = 3;

    private void Update()
    {
        if (GameManager.instance.isGameState && !isClearPopupShowed)
        {
            TrySpawnMonsters();
        }
    }

    #region Initialize
    public void Init()
    {
        difficultyNum = DataBaseManager.instance.Load(Consts.CURRENT_DIFFICULTY, difficultyNum);
        mainStageNum = DataBaseManager.instance.Load(Consts.MAIN_STAGE_NUM, 1);
        preCheckpointNum = DataBaseManager.instance.Load($"{mainStageNum}_{Consts.STAGE_PRE_CHECK_POINT_NUM}", 0);
        OnUpdateStageRewardUI?.Invoke(preCheckpointNum);

        stageResourceDataHandler = ResourceManager.instance.stage;

        gameManager = GameManager.instance;

        gameManager.OnReset += ResetStage;

        monsterObjectPooler = PoolManager.instance.monster;

        UI_LosePanel ui_LosePanel = UIManager.instance.GetUIElement<UI_LosePanel>();

        stageInGameDataHandler = new StageInGameDataHandler();
        stageInGameDataHandler.OnUpdateCurrencyUI += ui_LosePanel.UpdateGoldAmount;
        stageInGameDataHandler.OnUpdateCurrencyUI += UIManager.instance.GetUIElement<UI_InGameGoldCurrencyText>().UpdateCurrencyText;

        stageInGameDataHandler.SetLosePanelRect(ui_LosePanel.GetComponent<RectTransform>().position);

        stageInGameDataHandler.OnUpdateHighestGold += UIManager.instance.GetUIElement<UI_BonusGoldPanel>().UpdateInGameHightestGold;
        stageInGameDataHandler.Init();
        gameManager.OnReset += stageInGameDataHandler.Reset;
        stageInGameDataHandler.Reset();



        monsterKillGold = ResourceManager.instance.stage.GetMonsterKillGold();

        for (int i = 0; i < layerMaskStrArr.Length; i++)
        {
            layerMasks[i] = LayerMask.NameToLayer(layerMaskStrArr[i]);
        }

        checkPointMax = stageResourceDataHandler.GetCheckPointMax();

        monsterBasements = new MonsterBasement[checkPointMax];

        boxMoverUnitPos = BoxManager.instance.boxMoveController.currentSpeed * towerArriveDuration;
        Vector3 screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        float colliderSizeX = boxMoverUnitPos * checkPointMax * 2 + screenSize.x;

        groundsPosY = new float[layerTransforms.Length];

        for (int i = 0; i < layerTransforms.Length; i++)
        {
            BoxCollider2D collider2D = layerTransforms[i].GetComponent<BoxCollider2D>();
            Vector2 colliderSize = collider2D.size;
            colliderSize.x = colliderSizeX;
            collider2D.size = colliderSize;

            Vector2 layerPos = layerTransforms[i].position;
            layerPos.x += colliderSizeX * Consts.HALF * Consts.HALF - screenSize.x * Consts.HALF;
            layerTransforms[i].position = layerPos;

            float height = colliderSize.y;
            float offsetY = collider2D.offset.y;
            groundsPosY[i] = layerPos.y + offsetY + height * Consts.HALF;
        }

        //Load StageNum
        monsterTypeDict = new Dictionary<MonsterType, int>();
        spawnTimerDict = new Dictionary<MonsterType, MonsterSpawnTimer>();

        currentStageData = ResourceManager.instance.stage.GetStageData(mainStageNum, checkpointNum);

        monsterTypes = new List<MonsterType>();
        monsterTypes.AddRange((MonsterType[])Enum.GetValues(typeof(MonsterType)));

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));

        monsterOffsetPosX = worldPos.x - BoxManager.instance.transform.position.x;

        monsterResourceHandler = ResourceManager.instance.monster;

        UpdateWaveDuration();

        for (int i = 0; i < currentStageData.monsterIndexes.Length; i++)
        {
            int index = currentStageData.monsterIndexes[i];
            Monster monster = monsterResourceHandler.GetResource(index);

            MonsterType monsterType = monsterResourceHandler.GetMonsterData(index).monsterType;

            if (monsterType == MonsterType.Range || monsterType == MonsterType.Flying)
            {
                RangeMonster rangeMonster = monster as RangeMonster;
                ProjectileType projectileType = rangeMonster.GetProjectileType();
                PoolManager.instance.projectile.AddPoolInfo((int)projectileType, monsterResourceHandler.monsterProjectileInitCount,
                    monsterResourceHandler.monsterProjectileMaxCount);
            }

            int monsterTypeIndex = monsterTypes.IndexOf(monsterType) - 1;
            bool isIntervalZero = currentStageData.monsterInstIntervals[monsterTypeIndex] == 0f;

            if (isIntervalZero)
            {
                continue;
            }

            if (!monsterTypeDict.ContainsKey(monsterType))
            {
                monsterTypeDict.Add(monsterType, index);
                spawnTimerDict.Add(monsterType, new MonsterSpawnTimer());
            }
            else
            {
                monsterObjectPooler.RemovePoolInfo(index);
                monsterTypeDict[monsterType] = index;
            }

            spawnTimerDict[monsterType].UpdateInterval(currentStageData.monsterInstIntervals[monsterTypeIndex]);

            // TODO: 하드 코딩 바꿀 것
            monsterObjectPooler.AddPoolInfo(index, monsterResourceHandler.monsterInitCount, monsterResourceHandler.monsterMaxCount);
        }

        int currentMainStageNum = mainStageNum;
        if (mainStageNum % Consts.STAGE_DIVIDE_VALUE != 0)
        {
            difficultyNum = mainStageNum / Consts.STAGE_DIVIDE_VALUE + 1;
            currentMainStageNum = mainStageNum % Consts.STAGE_DIVIDE_VALUE;
        }
        else
        {
            difficultyNum = mainStageNum / Consts.STAGE_DIVIDE_VALUE;
            currentMainStageNum = Consts.STAGE_DIVIDE_VALUE;
        }

        OnUpdateStageUI?.Invoke(difficultyNum, currentMainStageNum, currentStageData.stageName);
        OnUpdateWeaponTypes?.Invoke(difficultyNum, mainStageNum);

        boxMoveController = BoxManager.instance.boxMoveController;

        OnClearStage += RewardManager.instance.GetReward;

        SetMonsterBasementPosX(BattleManager.instance.targetInRangeTrigger.GetOffsetPosX());

        // TODO : Remove Hard Coding
        currentMainStageNum = mainStageNum % stageRepeatValue;
        currentMainStageNum = currentMainStageNum == 0 ? 1 : currentMainStageNum;
        (Sprite, Sprite) backgroundImages = stageResourceDataHandler.GetBackgroundSprites(currentMainStageNum);
        OnUpdateBackgroundSprite?.Invoke(backgroundImages.Item1, backgroundImages.Item2);

        BoxManager.instance.InitBoxWeapons();
    }

    private void InitMonsterBasements()
    {
        BigInteger basementGoldUnit = ResourceManager.instance.stage.GetGoldByEnemyBaseHealth();

        Vector2 boxManagerPos = BoxManager.instance.transform.position;


        for (int i = 1; i <= checkPointMax; i++)
        {
            float posX = boxManagerPos.x + targetInRangePosX + boxMoverUnitPos * i;
            monsterBasements[i - 1] = Instantiate(monsterBasementPrefab, new Vector3(posX, boxManagerPos.y, 0), Quaternion.identity, transform);
            // TODO: Set Index from Data
            MonsterBasement monsterBasement = monsterBasements[i - 1];
            monsterBasement.OnUpdateInGameCurrency += stageInGameDataHandler.UpdateCurrency;
            monsterBasement.SetGoldEarningUnitByHp(basementGoldUnit, hpUnit);
            monsterBasement.OnReturnAction += DeactivateMonsterBasement;
            monsterBasement.OnForceReturnAction += ForceToDeactivateMonsterBasement;
            monsterBasement.Init();

            if (i == checkPointMax)
            {
                goalPosX = posX - targetInRangePosX;
            }
        }

        UpdateMonsterBasement();
    }

    public void SetMonsterBasementPosX(float targetInRangePosX)
    {
        this.targetInRangePosX = targetInRangePosX;
        InitMonsterBasements();
    }
    #endregion

    #region Spawn Monsters
    private void TrySpawnMonsters()
    {
        int count = 0;
        UpdateWaveInterval();

        foreach (var item in spawnTimerDict)
        {
            if (item.Value.UpdateElapsedTime())
            {
                int index = UnityEngine.Random.Range(0, layerTransforms.Length);
                float monsterPosX = BoxManager.instance.transform.position.x + monsterOffsetPosX + monsterSpawnPosXMod;
                Monster monster = monsterObjectPooler.Pool(monsterTypeDict[item.Key],
                    new Vector3(monsterPosX, layerTransforms[index].position.y, 0), Quaternion.identity);
                monster.SetLayer(index);
                monster.SetMonsterBaseData(ResourceManager.instance.monster.GetMonsterData(monsterTypeDict[item.Key]));
                monster.gameObject.layer = layerMasks[index];
                monster.UpdateGroundPosY(groundsPosY[index]);
                monster.UpdateSpeed(monsterPosX, BoxManager.instance.transform.position.x, monsterArriveDuration);
            }

            count++;
        }
    }

    private void UpdateWaveInterval()
    {
        if (isSecondWaveFinished)
        {
            return;
        }

        stageElapsedTime += Time.deltaTime;

        if (!isFirstWaveFinished)
        {
            if (stageElapsedTime >= firstWaveDuration)
            {
                foreach (var timer in spawnTimerDict.Values)
                {
                    stageElapsedTime -= firstWaveDuration;
                    isFirstWaveFinished = true;
                    timer.MultiplyInterval(secondMonsterInstIntervalMultiplication);
                }
            }
        }
        else if (!isSecondWaveFinished)
        {
            if (stageElapsedTime >= secondWaveDuration)
            {
                foreach (var timer in spawnTimerDict.Values)
                {
                    stageElapsedTime -= secondWaveDuration;
                    isSecondWaveFinished = true;
                    timer.MultiplyInterval(thirdMonsterInstIntervalMultiplication);
                }
            }
        }
    }
    #endregion

    public float GetGoalPosX()
    {
        return goalPosX;
    }


    private void DeactivateMonsterBasement(MonsterBase monsterBasement)
    {
        monsterBasement.gameObject.SetActive(false);
        OnUpdateStageRewardUI?.Invoke(checkpointNum);

        WeaponManager.instance.TryFindTarget();
        HeroManager.instance.hero.ResetTarget(monsterBasement);

        if (preCheckpointNum < checkpointNum)
        {
            if (!DataBaseManager.instance.ContainsKey(Consts.EQUIPMENT_ENFORCE_REWARDED) && mainStageNum == 1 && checkpointNum == 3)
            {
                RewardManager.instance.AddTutorialGranadeRewards();
                DataBaseManager.instance.Save(Consts.EQUIPMENT_ENFORCE_REWARDED, true);
            }
            preCheckpointNum = checkpointNum;
            DataBaseManager.instance.Save($"{mainStageNum}_{Consts.STAGE_PRE_CHECK_POINT_NUM}", preCheckpointNum);
            OnUpdateReward?.Invoke(mainStageNum, checkpointNum);
        }

        if (preCheckpointNum == checkPointMax)
        {
            isClearPopupShowed = true;
            BigInteger sellingGold = BoxManager.instance.GetTotalGold();
            stageInGameDataHandler.UpdateCurrency(sellingGold);
            UpdateClearRewardUI();
            monsterObjectPooler.PushAllActiveMonsters(true);

            preCheckpointNum = 0;
            DataBaseManager.instance.Save($"{mainStageNum}_{Consts.STAGE_PRE_CHECK_POINT_NUM}", preCheckpointNum);
            OnUnlockShop?.Invoke(0, false);
            DataBaseManager.instance.Save(Consts.HERO_TAP_TOUCHED_GUIDE_START, true); 
            OnClearStage?.Invoke();
            BoxManager.instance.RemoveAllBoxes();
            CurrencyManager.instance.ResetCurrency(CurrencyType.Gold);
            BigInteger backpackGold = EquipmentManager.instance.GetBackpackGold();
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, backpackGold);
            return;
        }

        checkpointNum++;
        UpdateCurrentStageInfo();


        stageElapsedTime = 0f;
        isFirstWaveFinished = false;
        isSecondWaveFinished = false;
    }

    private void UpdateCurrentStageInfo()
    {
        monsterObjectPooler.PushAllActiveMonsters(true);
        currentStageData = ResourceManager.instance.stage.GetStageData(mainStageNum, checkpointNum);
        UpdateWaveDuration();

        for (int i = 0; i < currentStageData.monsterIndexes.Length; i++)
        {
            int index = currentStageData.monsterIndexes[i];

            MonsterType monsterType = monsterResourceHandler.GetMonsterData(index).monsterType;

            int monsterTypeIndex = monsterTypes.IndexOf(monsterType) - 1;
            bool isIntervalZero = currentStageData.monsterInstIntervals[monsterTypeIndex] == 0f;

            if (!monsterTypeDict.ContainsKey(monsterType))
            {
                monsterTypeDict.Add(monsterType, index);
                spawnTimerDict.Add(monsterType, new MonsterSpawnTimer());
            }

            spawnTimerDict[monsterType].UpdateInterval(currentStageData.monsterInstIntervals[monsterTypeIndex]);

            // TODO: 하드 코딩 바꿀 것
            monsterObjectPooler.AddPoolInfo(index, monsterResourceHandler.monsterInitCount, monsterResourceHandler.monsterMaxCount);
            OnUpdateStageUI?.Invoke(difficultyNum, mainStageNum, currentStageData.stageName);
        }
    }

    private void UpdateWaveDuration()
    {
        secondMonsterInstIntervalMultiplication = currentStageData.secondMonsterInstIntervalMultiplication;
        thirdMonsterInstIntervalMultiplication = currentStageData.thirdMonsterInstIntervalMultiplication;
        firstWaveDuration = currentStageData.firstWaveDuration;
        secondWaveDuration = currentStageData.secondWaveDuration;
    }

    private void ForceToDeactivateMonsterBasement(MonsterBasement monsterBasement)
    {
        monsterBasement.gameObject.SetActive(false);
        monsterObjectPooler.PushAllActiveMonsters(false);
    }

    private void ResetStage()
    {
        foreach (var monsterBasement in monsterBasements)
        {
            monsterBasement.Die(false);
            monsterBasement.gameObject.SetActive(true);
        }

        stageElapsedTime = 0f;
        isFirstWaveFinished = false;
        isSecondWaveFinished = false;
        checkpointNum = 1;
        UpdateCurrentStageInfo();
    }

    public MonsterBase GetMonsterBasement()
    {
        MonsterBasement monsterBasement = null;

        if (boxMoveController.isMonsterBasementEncountered)
        {
            monsterBasement = monsterBasements[checkpointNum - 1];
        }

        return monsterBasement;
    }

    public void EarnGold()
    {
        BigInteger gold = UnityEngine.Random.Range(1, monsterKillGold + 1);
        stageInGameDataHandler.UpdateCurrency(gold);
    }

    public void ClearStage()
    {
        for (int i = 0; i < currentStageData.monsterIndexes.Length; i++)
        {
            int index = currentStageData.monsterIndexes[i];
            Monster monster = monsterResourceHandler.GetResource(index);
            monsterObjectPooler.RemovePoolInfo(index);
            MonsterType monsterType = monsterResourceHandler.GetMonsterData(index).monsterType;
            monsterTypeDict[monsterType] = index;

            if (monsterType == MonsterType.Range || monsterType == MonsterType.Flying)
            {
                RangeMonster rangeMonster = monster as RangeMonster;
                ProjectileType projectileType = rangeMonster.GetProjectileType();
                PoolManager.instance.projectile.RemovePoolInfo((int)projectileType);
                
            }

            monsterTypeDict.Remove(monsterType);
            spawnTimerDict.Remove(monsterType);
        }

        isClearPopupShowed = false;
        mainStageNum++;
        checkpointNum = 1;

        FirebaseAnalytics.LogEvent($"current_stage_{mainStageNum}");


        int currentMainStageNum = mainStageNum;
        if (mainStageNum % Consts.STAGE_DIVIDE_VALUE != 0)
        {
            difficultyNum = mainStageNum / Consts.STAGE_DIVIDE_VALUE + 1;
            currentMainStageNum = mainStageNum % Consts.STAGE_DIVIDE_VALUE;
        }
        else
        {
            currentMainStageNum = Consts.STAGE_DIVIDE_VALUE;
        }

        DataBaseManager.instance.Save(Consts.MAIN_STAGE_NUM, mainStageNum);
        DataBaseManager.instance.Save(Consts.CURRENT_DIFFICULTY, difficultyNum);

        currentStageData = stageResourceDataHandler.GetStageData(mainStageNum, checkpointNum);
        if (currentStageData.stageNum == 0)
        {
            currentStageData = stageResourceDataHandler.GetStageData(mainStageNum, checkpointNum);
        }
        OnUpdateWeaponTypes?.Invoke(difficultyNum, mainStageNum);
        OnUpdateStageUI?.Invoke(difficultyNum, currentMainStageNum, currentStageData.stageName);

        stageInGameDataHandler.ClearReset();

        gameManager.UpdateGameState(false);
        UpdateMonsterBasement();

        for (int i = 0; i < currentStageData.monsterIndexes.Length; i++)
        {
            int index = currentStageData.monsterIndexes[i];
            Monster monster = monsterResourceHandler.GetResource(index);

            MonsterType monsterType = monsterResourceHandler.GetMonsterData(index).monsterType;

            if (monsterType == MonsterType.Range || monsterType == MonsterType.Flying)
            {
                RangeMonster rangeMonster = monster as RangeMonster;
                ProjectileType projectileType = rangeMonster.GetProjectileType();
                PoolManager.instance.projectile.AddPoolInfo((int)projectileType, monsterResourceHandler.monsterProjectileInitCount,
                    monsterResourceHandler.monsterProjectileMaxCount);
            }

            int monsterTypeIndex = monsterTypes.IndexOf(monsterType) - 1;
            bool isIntervalZero = currentStageData.monsterInstIntervals[monsterTypeIndex] == 0f;

            if (isIntervalZero)
            {
                continue;
            }

            if (!monsterTypeDict.ContainsKey(monsterType))
            {
                monsterTypeDict.Add(monsterType, index);
            }

            monsterTypeDict[monsterType] = index;

            if (!spawnTimerDict.ContainsKey(monsterType))
            {
                spawnTimerDict.Add(monsterType, new MonsterSpawnTimer());
            }

            spawnTimerDict[monsterType].UpdateInterval(currentStageData.monsterInstIntervals[monsterTypeIndex]);

            // TODO: 하드 코딩 바꿀 것
            monsterObjectPooler.AddPoolInfo(index, monsterResourceHandler.monsterInitCount, monsterResourceHandler.monsterMaxCount);
        }

        // TODO : Remove Hard Coding
        currentMainStageNum = mainStageNum % stageRepeatValue;
        currentMainStageNum = currentMainStageNum == 0 ? stageRepeatValue : currentMainStageNum;
        (Sprite, Sprite) backgroundImages = stageResourceDataHandler.GetBackgroundSprites(currentMainStageNum);
        OnUpdateBackgroundSprite?.Invoke(backgroundImages.Item1, backgroundImages.Item2);

        stageElapsedTime = 0f;
        isFirstWaveFinished = false;
        isSecondWaveFinished = false;

        AdsManager.instance.TryShowInterstitial();
    }

    private void UpdateClearRewardUI()
    {
        BigInteger goldAmount = stageInGameDataHandler.GetInGameCurrency();
        BigInteger enforcePowderAmount = goldAmount / Consts.PERCENT_DIVIDE_VALUE;
        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.EnforcePowder, enforcePowderAmount);
        OnUpdateClearCurrencyUI?.Invoke(goldAmount, enforcePowderAmount);
    }

    private void UpdateMonsterBasement()
    {
        (Sprite,Sprite) spriteTuple = ResourceManager.instance.stage.GetMonsterBasementSprites(mainStageNum);

        for (int i = 0; i < monsterBasements.Length; i++)
        {
            StageData tempStageData = stageResourceDataHandler.GetStageData(mainStageNum, i + 1);

            MonsterBasement monsterBasement = monsterBasements[i];
            MonsterData tempMonsterData = new MonsterData(-1, $"monsterBasement{difficultyNum}_{mainStageNum}_{checkpointNum}", MonsterType.Basement, 0, tempStageData.enemyBaseHealth, 0);
            monsterBasement.SetMonsterBaseData(tempMonsterData);

            if (i < monsterBasements.Length - 1) 
            {
                monsterBasement.UpdateSprite(spriteTuple.Item1);
            }
            else
            {
                monsterBasement.UpdateSprite(spriteTuple.Item2);
            }
        }
    }

#if UNITY_EDITOR
    public void DestroyBasementAll()
    {
        for (int i = 0; i < monsterBasements.Length; i++)
        {
            DeactivateMonsterBasement(monsterBasements[i]); 
        }
    }
#endif
}
