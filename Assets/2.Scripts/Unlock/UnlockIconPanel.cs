using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockIconPanel : MonoBehaviour
{
    [SerializeField] private UnlockIconDataSO unlockIconDataSO;
    [SerializeField] private Image unlockIconImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button disableButton;

    private Queue<UnlockIconData> iconStackRegistered = new Queue<UnlockIconData>();

    public void Init()
    {
        unlockIconDataSO.Init();

        ActivateIconPanel(false);
        disableButton.onClick.AddListener(() => ActivateIconPanel(false));
        unlockIconDataSO.OnUpdateUnlockIcon += TryShowUnlockIcon;
    }

    private void TryShowUnlockIcon(UnlockIconData unlockIconData)
    {
        bool wasSummonForgeDialogueFinished = ES3.Load<bool>(Consts.FirstOpen, false, ES3.settings);
        if (!wasSummonForgeDialogueFinished)
        {
            iconStackRegistered.Enqueue(unlockIconData);
            return;
        }

        if (GetIsUnlockCanvasActiveState())
        {
            if (!iconStackRegistered.Contains(unlockIconData) && unlockIconImage.sprite != unlockIconData.sprite)
            {
                iconStackRegistered.Enqueue(unlockIconData);
            }
        }
        else
        {
            ShowUnlockIcon(unlockIconData);
        }
    }

    private void ShowUnlockIcon(UnlockIconData unlockIconData)
    {
        bool wasShowed = ES3.Load<bool>($"unlock_{unlockIconData.iconName}", false, ES3.settings);
        if (!wasShowed && unlockIconImage != null)
        {
            unlockIconImage.sprite = unlockIconData.sprite;
            descriptionText.text = $"<color=green>{unlockIconData.iconName}</color>\n해금 되었습니다.";
            ActivateIconPanel(true);
            ES3.Save<bool>($"unlock_{unlockIconData.iconName}", true, ES3.settings);
            ES3.StoreCachedFile();
        }
    }

    private void ActivateIconPanel(bool isActive)
    {
        if (!isActive)
        {
            if (iconStackRegistered.Count > 0)
            {
                ShowUnlockIcon(iconStackRegistered.Dequeue());
                return;
            }
        }

        gameObject.SetActive(isActive);
    }

    private bool GetIsUnlockCanvasActiveState()
    {
        if (this != null)
        {
            return gameObject.activeInHierarchy;
        }

        return false;
    }
}