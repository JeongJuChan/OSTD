using System;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private ResourceManager resourceManager;
    private EffectSpawner effectSpawner;
    private CurrencyManager currencyManager;
    private UIManager uiManager;
    private AdsManager adsManager;
    private SkillManager skillManager;
    private OfflineTimerController offlineTimerController;

    public bool isInitializing { get; private set; } = false;

    public bool isGameState { get; private set; }

    public event Action OnReset;
    public event Action OnStart;

    private void Awake()
    {
        DataBaseManager.instance.Init();
        currencyManager = CurrencyManager.instance;
        currencyManager.Initialize();
        resourceManager = ResourceManager.instance;
        resourceManager.Init();
        adsManager = AdsManager.instance;
        adsManager.Initialize();
        PoolManager.instance.Init();
        CameraManager.instance.Init();
        HeroManager.instance.Init();
        EquipmentManager.instance.Init();
        RewardMovingController.instance.Init();

        uiManager = UIManager.instance;
        uiManager.Init();
        FindAnyObjectByType<BottomBarController>().Init();

        BattleManager.instance.Init();
        BoxManager.instance.Init();
        WeaponManager.instance.Init();
        StageManager.instance.Init();
        skillManager = SkillManager.instance;
        skillManager.Init();

        RewardManager.instance.Init();

        UIManager.instance.GetUIElement<UI_Battle>().OnClickGameStart += () => UpdateGameState(true);

        UIAnimations.instance.Initialize();

#if UNITY_EDITOR || UNITY_ANDROID
        PushNotificationManager.instance.StartInit();
#endif
    }

    public void SetInitializing()
    {
        isInitializing = true;
    }

    public void UpdateGameState(bool isGameState)
    {
        this.isGameState = isGameState;
        if (isGameState)
        {
            OnStart?.Invoke();
        }
        if (!isGameState)
        {
            OnReset?.Invoke();
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            UIAnimations.instance.ShakeCamera();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // stageController.EditorSkipCurrentRoutineStageeee();
        }
    }
#endif

    private void OnDestroy()
    {
        UnlockManager.DisPose();
        UIAnimations.DisPose();
    }

    private void OnApplicationPause(bool pause)
    {
        if (ES3.KeyExists("PlayerNickName") && gameObject.scene.name == "GameScene")
        {
            if (pause)
            {
                if (offlineTimerController != null) offlineTimerController.TimeReset();
            }
            else
            {
                if (offlineTimerController != null) offlineTimerController.RefilKey();
            }
        }
    }
}
