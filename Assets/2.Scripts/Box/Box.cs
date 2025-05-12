using System;
using System.Collections;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Rendering;

public class Box : MonoBehaviour, IDamageable, IDie, IHasHpUI, IAlive
{
    [Header("Collide")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Rigidbody2D rigid;

    [Header("Box")]
    [SerializeField] private Transform spriteParent;
    [SerializeField] private DragableBox dragableBox;
    [SerializeField] private Transform lastPosObjectTrans;
    [SerializeField] private SpriteRenderer spriteRenderer;
    

    [Header("Weapon")]
    [SerializeField] private Transform weaponParent;

    [Header("Box Data")]
    private BoxData boxData;
    private bool isLevelMax;

    [field: Header("Box Events")]
    public event Action OnDamaged;
    public Action OnDead { get; set; }
    public event Action<Box> OnLevelUpBox;
    public event Action<BigInteger, BigInteger> OnUpdateMaxHPUI;
    public event Action<BigInteger> OnUpdateCurrenHPUI;
    public event Action OnAlive;

    public Action OnResetHPUI{ get; set; }
    public Action<bool> OnActiveHpUI{ get; set; }

    [field: Header("Index")]
    public int index { get; private set; }

    [Header("Box UI")]
    [SerializeField] private HPUIPanel hpUIPanel;
    private UI_UpgradeBoxButton ui_UpgradeBoxButton;
    public UI_BoxWeaponPanel ui_WeaponPanel { get; private set; }
    private RectTransform canvasRect;

    [Header("Box Animation")]
    [SerializeField] private Animator levelUpAnimator;

    private WaitForSeconds deadWaitForSeconds;

    private CurrencyManager currencyManager;

    private Vector2 offsetPosition;

    [field: SerializeField] public bool isDead { get; private set; }
    [field: SerializeField] public ParticleSystem deadEffect { get; set; }

    private BigInteger hp = 0;

    private Vector2 upgradeBoxLocalPos;
    private Vector2 weaponPanelLocalPos;

    private BlinkCalculator<Box> blinkCalculator;

    private Action<BigInteger> OnUpdatePriceText;

    [SerializeField] private float uiBoxButtonDistanceMod = 0.9f;

    #region Unity Events
    private void OnEnable() 
    {
        OnAlive?.Invoke();
    }

    private void OnDisable() 
    {
        UpdateBoxUIActiveState(false);
    }
    #endregion

    #region Initialize
    public void Init()
    {
        currencyManager = CurrencyManager.instance;
        UI_Bin ui_Bin = UIManager.instance.GetUIElement<UI_Bin>();
        dragableBox.OnDragStart += () => UpdateBoxUIActiveState(false);
        dragableBox.OnDragStart += UpdateSellingPrice;
        dragableBox.OnDragStart += () => UpdateWeaponDragState(true);
        OnUpdatePriceText += ui_Bin.OnUpdatePriceText;
        dragableBox.OnDragEnd += () => ui_Bin.UpdateSellingPriceTextActiveState(false);
        dragableBox.OnDragEnd += () => UpdateBoxUIActiveState(true);
        dragableBox.OnDragEnd += () => ui_Bin.TrySellBox(this);
        dragableBox.OnDragEnd += UpdateBoxPos;
        dragableBox.OnDragEnd += () => UpdateWeaponDragState(false);
        dragableBox.OnDrag += UpdateCurrentDraggedBoxPos;
        dragableBox.Init();
        dragableBox.SetDamageable(this);
        hpUIPanel.init(this);
        OnDead += () => OnActiveHpUI?.Invoke(false);
        OnDead += hpUIPanel.ResetUI;
        OnDead += () => BoxManager.instance.RemoveInGameBox(this);
        canvasRect = UIManager.instance.GetUIElement<UI_Battle>().GetComponent<RectTransform>();
        OnActiveHpUI?.Invoke(false);
        deadWaitForSeconds = CoroutineUtility.GetWaitForSeconds(deadEffect.main.duration);
        blinkCalculator = new BlinkCalculator<Box>(this, new SpriteRenderer[] { spriteRenderer }, 0.25f);
        OnAlive?.Invoke();
        dragableBox.OnUpdateSprite += blinkCalculator.UpdateTextures;
    }

    public float GetUnitSizeY()
    {
        float sizeY = boxCollider.size.y * boxCollider.transform.lossyScale.y;
        return sizeY;
    }

    public float GetUnitSizeX()
    {
        float sizeX = boxCollider.size.x * boxCollider.transform.lossyScale.x;
        return sizeX;
    }
    #endregion

    #region BoxData
    public void UpdateBoxData(BoxData boxData)
    {
        this.boxData = boxData;
        isLevelMax = boxData.cost == 0;

        hp = boxData.hp;

        UpdateInteractableLevelUpButton(currencyManager.GetCurrencyValue(CurrencyType.Gold));
        hpUIPanel.UpdateMaxHP(boxData.hp, boxData.hp);
        ui_UpgradeBoxButton.UpdateBoxDataUI(boxData, isLevelMax);
    }

    public void UpdateBoxSprite(Sprite sprite)
    {
        dragableBox.UpdateSprite(sprite);
    }

    public int GetBoxLevel()
    {
        return boxData.level;
    }

    public void SetIndex(int index)
    {
        this.index = index;
        ui_WeaponPanel.SetIndex(index);
    }
    #endregion

    #region BoxUI
    public void SetBoxUI(UI_UpgradeBoxButton ui_UpgradeBoxButton, UI_BoxWeaponPanel ui_WeaponPanel)
    {
        this.ui_UpgradeBoxButton = ui_UpgradeBoxButton;
        this.ui_WeaponPanel = ui_WeaponPanel;
        UpdateBoxUIActiveState(false);
        ui_UpgradeBoxButton.AddButtonAction(LevelUpButtonClicked);
        currencyManager.GetCurrency(CurrencyType.Gold).OnCurrencyChange += UpdateInteractableLevelUpButton;
    }

    private void UpdateInteractableLevelUpButton(BigInteger amount)
    {
        if (isLevelMax)
        {
            return;
        }

        ui_UpgradeBoxButton.UpdateInteractable(amount >= ResourceManager.instance.box.GetBoxData(GetBoxLevel()).cost);
    }

    public void UpdateBoxUIActiveState(bool isActive)
    {
        if (ui_UpgradeBoxButton != null)
        {
            ui_UpgradeBoxButton.gameObject.SetActive(isActive);
        }
        if (ui_WeaponPanel != null)
        {
            ui_WeaponPanel.gameObject.SetActive(isActive);
        }
    }

    public void UpdateBoxUIPos()
    {
        float distX = GetUnitSizeX() * uiBoxButtonDistanceMod;
        Vector3 upgradeBoxPos = transform.position;
        upgradeBoxPos.x += -distX;
        upgradeBoxPos.y += spriteParent.transform.localPosition.y;
        Vector3 weaponPanelPos = transform.position;
        weaponPanelPos.x += distX;
        weaponPanelPos.y += spriteParent.transform.localPosition.y;

        // Convert world position to screen position
        Vector3 upgradeBoxScreenPos = Camera.main.WorldToScreenPoint(upgradeBoxPos);
        Vector3 weaponPanelScreenPos = Camera.main.WorldToScreenPoint(weaponPanelPos);

        // Convert screen position to local position on the canvas

        Vector2 boxLocalPos;
        Vector2 weaponLocalPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, upgradeBoxScreenPos, Camera.main, out boxLocalPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, weaponPanelScreenPos, Camera.main, out weaponLocalPos);

        // Set the UI element positions

        if (upgradeBoxLocalPos != Vector2.zero)
        {
            upgradeBoxLocalPos.y = boxLocalPos.y;
            weaponPanelLocalPos.y = weaponLocalPos.y;
        }
        else
        {
            upgradeBoxLocalPos = boxLocalPos;
            weaponPanelLocalPos = weaponLocalPos;
        }

        ui_UpgradeBoxButton.rect.anchoredPosition = upgradeBoxLocalPos;
        ui_WeaponPanel.rect.anchoredPosition = weaponPanelLocalPos;
    }
    #endregion

    #region Update State
    public void Reset()
    {
        isDead = false;
        SetInLobby();
        UpdateBoxUIPos();
    }

    public void UpdateBoxPos(Vector2 pos)
    {
        transform.position = pos;
        UpdateBoxOffsetPos();
    }

    public void UpdateBoxOffsetPos()
    {
        offsetPosition = transform.position;
    }

    private void UpdateBoxPos()
    {
        transform.position = lastPosObjectTrans.position;
        lastPosObjectTrans.localPosition = Vector2.zero;
        offsetPosition = transform.position;
        UpdateBoxUIPos();
    }

    private void UpdateCurrentDraggedBoxPos()
    {
        BoxManager.instance.TryUpdateBoxLastPos(lastPosObjectTrans, index);
    }

    private void LevelUpButtonClicked()
    {
        OnLevelUpBox?.Invoke(this);
        levelUpAnimator.SetTrigger(AnimatorParameters.BOX_LEVEL_UP);
    }
    #endregion

    #region InGame
    public void SetInGame()
    {
        SetKinematicState(false);
        UpdateBoxUIActiveState(false);
        dragableBox.InGameSetting();
    }

    public void TakeDamage(BigInteger damage)
    {
        if (isDead)
        {
            return;
        }

        boxData.hp -= damage;
        boxData.hp = boxData.hp < 0 ? 0 : boxData.hp;


        if (boxData.hp <= 0)
        {
            Die(true);
            return;
        }

        OnDamaged?.Invoke();
        OnActiveHpUI?.Invoke(true);
        hpUIPanel.UpdateCurrentHPUI(boxData.hp);
    }
    #endregion

    #region InLobby
    public void SetInLobby()
    {
        if (rigid != null)
        {
            rigid.velocity = Vector2.zero;
            SetKinematicState(true);
        }
        UpdateBoxUIActiveState(true);
        dragableBox.Reset();
        gameObject.SetActive(true);
        OnActiveHpUI?.Invoke(false);
    }
    #endregion

    #region Rigid
    private void SetKinematicState(bool isActive)
    {
        rigid.isKinematic = isActive;
    }

    public void Die(bool isCausedByBattle)
    {
        isDead = true;

        if (isCausedByBattle)
        {
            StartCoroutine(CoOnDie());
        }
        else
        {
            boxData.hp = hp;
            OnUpdateCurrenHPUI?.Invoke(hp);
            OnDead?.Invoke();
        }
    }

    protected IEnumerator CoOnDie()
    {
        deadEffect.Play();
        dragableBox.gameObject.SetActive(false);
        OnDead?.Invoke();
        yield return deadWaitForSeconds;
        
        boxData.hp = hp;
        OnUpdateCurrenHPUI?.Invoke(hp);
        gameObject.SetActive(false);
    }
    #endregion


    public void UpdateUpgradeWeaponInteractable(bool isInteractable)
    {
        ui_WeaponPanel.upgradeWeaponButton.UpdateInteractable(isInteractable);
    }

    public void UpdateUpgradeWeaponUI(int level, int upgradeBlockCount, BigInteger damage, BigInteger cost, BigInteger currentCurrency)
    {
        ui_WeaponPanel.UpdateLevelUpWeaponButton(level, upgradeBlockCount, damage, cost, currentCurrency);
    }

    public void SetWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(weaponParent);
    }

    public void UpdateInGamePositionX(float x)
    {
        Vector2 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

    public void ResetWeaponPanel()
    {
        ui_WeaponPanel.UpdateSelectPanelActiveState(true);
    }

    public void ActivateUpgradeWeaponPanel()
    {
        ui_WeaponPanel.UpdateSelectPanelActiveState(false);
    }

    public void ChangeDragableState(bool isDragable)
    {
        boxCollider.enabled = isDragable;
    }

    private void UpdateSellingPrice()
    {
        BigInteger totalGold = BoxManager.instance.GetTotalGoldByBoxIndex(index);
        OnUpdatePriceText?.Invoke(totalGold);
    }

    private void UpdateWeaponDragState(bool isDragging)
    {
        Weapon weapon = WeaponManager.instance.GetWeaponByBoxIndex(index);
        if (weapon == null)
        {
            return;
        }
        weapon.UpdateWeaponDragSettings(isDragging);
    }

    
}
