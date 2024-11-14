using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BottomBarController : MonoBehaviour
{
    [System.Serializable]
    public struct UIElement
    {
        public RedDotIDType redDotID;
        public FeatureID featureID;
        [HideInInspector] public UnlockData unlockData;
        public UI_BottomElement canvas;
        public Button button;
        public Image image;
        public TextMeshProUGUI text;
        public GameObject lockIcon;
    }

    [Header("UI Elements")]
    [SerializeField] public UIElement[] uiElements;
    [SerializeField] private UI_Base[] ui_bases;

    [SerializeField] private GuideController guide;

    // TODO : 락 풀 것
    //private bool[] isLocked = new bool[6] { true, true, true, true, true, true };
    private bool[] isLocked = new bool[5] { true, true, false, false, false };

    private UIElement? activeElement = null;
    private Sprite activeSprite;
    private string activeElementName;
    private UI_Alert uI_Alert;

    protected Sprite onSprite;
    protected Sprite offSprite;

    [SerializeField] private UI_BottombarTutorialPanel uI_BottombarTutorialPanel;

    public void Init()
    {
        uI_BottombarTutorialPanel.Init();
        LoadDatas();
        onSprite = Resources.Load<Sprite>($"UI/BottomBar/Bottombar_on");
        offSprite = Resources.Load<Sprite>($"UI/BottomBar/Bottombar_off");
        RewardManager.instance.OnUnlockHero += SetLockState;
        StageManager.instance.OnUnlockShop += SetLockState;
        InitializeButtons();
        uiElements[1].canvas.GetComponentInChildren<UI_HeroTutorialPanel>().OnHeroTapUnlock += SetLockState;

        // guide.Initialize();

        // uI_Alert = UIManager.instance.GetUIElement<UI_Alert>();
    }

    private void InitializeButtons()
    {
        //InitializeButtonsSize();

        for (int i = 0; i < uiElements.Length; i++)
        {
            int index = i;
            var uiElement = uiElements[i];

           /* UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(uiElement.featureID);
            uiElement.unlockData = unlockData;*/

            uiElement.button.onClick.AddListener(() =>
            {
                // ToggleCanvas(uiElement, isLocked[index]);
                ToggleCanvas(uiElement, false);
            });
            // uiElement.lockIcon.SetActive(isLocked[i]);

            uiElement.canvas = (UI_BottomElement)UIManager.instance.GetUIElement(ui_bases[i]);

                        // if (uiElements[i].canvas.name == "UI_Castle(Clone)")
            // {
            // }
            //uiElement.canvas.cloaseBtn.onClick.AddListener(() => CloseCanvas());

            uiElement.canvas.openUI += () => OpenCanvas(uiElement);
            uiElement.canvas.Initialize();

            SetLockState(i, isLocked[i]);
            uiElements[i] = uiElement;

        }

        GameManager.instance.OnStart += () => UpdateActiveState(false);
        GameManager.instance.OnReset += () => UpdateActiveState(true);
    }

    private void UpdateActiveState(bool isactive)
    {
        gameObject.SetActive(isactive);
    }

    private void InitializeButtonsSize()
    {
        /*int buttonLength = ui_bases.Length;
        float buttonSize = Screen.width / buttonLength;
        int halfIndex = buttonLength / 2 - 1;


        if (halfIndex < 0)
        {
            RectTransform rectTransform = ui_bases[0].GetComponent<RectTransform>();
            Vector2 size = rectTransform.sizeDelta;
            size.x = Screen.width;
            rectTransform.sizeDelta = size;
        }
        else
        {
            RectTransform rectTransform = ui_bases[halfIndex].GetComponent<RectTransform>();
            Vector2 offsetSize = rectTransform.sizeDelta;
            Vector2 selectedSize = new Vector2(offsetSize.x * mainButtonSizeMod.x, offsetSize.y * mainButtonSizeMod.y);
            bool isButtonLengthEven = buttonLength % 2 == 0;

            float widthDiff = selectedSize.x - offsetSize.x;

            for (int i = 0; i < buttonLength; i++)
            {
                
            }
        }*/



    }

    public void InitUnlock()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            int index = i;
            var uiElement = uiElements[i];

            if (uiElement.unlockData == null)
            {
                continue;
            }

            UnlockManager.instance.RegisterFeature(new UnlockableFeature(uiElement.unlockData.featureType, uiElement.unlockData.featureID, uiElement.unlockData.count,
            () =>
            {
                SetLockState(index, false);
            }));

            NotificationManager.instance.SetNotification(uiElement.redDotID.ToString(), false);
            uiElement.canvas.StartInit();
        }
    }

    public void SetLockState(int index, bool state)
    {
        if (index < 0 || index >= isLocked.Length) return;

        isLocked[index] = state;
        uiElements[index].button.interactable = !state;
        uiElements[index].lockIcon.SetActive(state);

        SaveDatas();
    }

    public void GameFailedCanvas()
    {
        ToggleCanvas(uiElements[1], isLocked[1]);
    }

    public void ToggleCanvas(UIElement element, bool locked)
    {
        if (locked)
        {
            Debug.Log("This element is locked.");
            if (element.unlockData.featureType == FeatureType.Stage)
            {
                uI_Alert.AlertMessage($"<color=green>{Difficulty.TransformStageNumber(element.unlockData.count)}</color>을 클리어 해야합니다.");
            }
            return;
        }

        if (activeElement != null && activeElement.Value.canvas == element.canvas)
        {
            // CloseCanvas();
        }
        else
        {
            if (activeElement != null)
            {
                CloseCanvas();
            }
            OpenCanvas(element);
        }
    }

    public void OpenDungeonCanvas()
    {
        ToggleCanvas(uiElements[3], isLocked[3]);
    }

    private void OpenCanvas(UIElement element)
    {
        activeElement = element;
        activeElementName = element.text.text;
        activeSprite = element.image.sprite;
        activeElement.Value.button.image.sprite = onSprite;
        element.canvas.OpenUI();
        //NotificationManager.instance.SetNotification(element.redDotID.ToString(), false);
    }

    private void CloseCanvas()
    {
        if (activeElement == null) return;

        activeElement.Value.canvas.CloseUI();
        activeElement.Value.image.sprite = activeSprite;
        activeElement.Value.text.text = activeElementName;
        activeElement.Value.button.image.sprite = offSprite;

        // NotificationManager.instance.SetNotification(activeElement.Value.redDotID.ToString(), false);

        activeElement = null;
        activeElementName = null;
        activeSprite = null;
    }

    private void SaveDatas()
    {

        ES3.Save<bool[]>(Consts.IS_LOCKED, isLocked, ES3.settings);

        ES3.StoreCachedFile();
    }

    private void LoadDatas()
    {
        if (ES3.KeyExists(Consts.IS_LOCKED))
        {
            isLocked = ES3.Load<bool[]>(Consts.IS_LOCKED);
        }
    }
}