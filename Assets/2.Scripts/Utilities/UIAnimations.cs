using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAnimations : Singleton<UIAnimations>
{
    private RectTransform uI_SummonResult;
    private Vector3 ui_SummonResultOriginalPos;
    private Camera camera;

    private float shakeDuration = .5f;
    private float shakeStrength = 10f;
    private int vibrato = 10;
    private float randomness = 90f;

    public void Initialize()
    {
        if (!UIManager.instance.GetUIElement<UI_SummonResult>().TryGetComponent(out uI_SummonResult))
        {
            Debug.LogError("Failed to initialize uI_SummonResult");
        }

        camera = Camera.main;

        ui_SummonResultOriginalPos = uI_SummonResult.anchoredPosition;
    }


    public Sequence UIBaseOpenAnimation(UIAnimationType type, UI_Base ui)
    {
        GameObject target = ui.transform.GetChild(ui.transform.childCount - 1).gameObject;

        switch (type)
        {
            case UIAnimationType.Scale:
                return ScaleUp(target);
            case UIAnimationType.VerticalPunch:
                return VerticalPunchUpward(target);
            default:
                return null;
        }
    }

    public Sequence UIBaseCloseAnimation(UIAnimationType type, UI_Base ui)
    {
        Action endAction = () => ui.gameObject.SetActive(false);

        GameObject target = ui.transform.GetChild(ui.transform.childCount - 1).gameObject;

        switch (type)
        {
            case UIAnimationType.Scale:
                return ScaleDown(target, endAction);
            case UIAnimationType.VerticalPunch:
                return VerticalPunchDownward(target, endAction);
            default:
                endAction();
                return null;
        }
    }



    public Sequence ScaleUp(GameObject target, Action endAction = null)
    {
        if (!target.TryGetComponent(out RectTransform rect)) return null;

        Sequence animation = DOTween.Sequence();

        animation.Append(rect.DOScale(1f, 0.2f));
        animation.OnStart(() => rect.localScale = Vector3.zero);
        if (endAction != null) animation.OnComplete(() => endAction());

        return animation.SetUpdate(true);
    }

    public Sequence VerticalPunchUpward(GameObject target, Action endAction = null)
    {
        Sequence animation = DOTween.Sequence();

        animation.Append(target.transform.DOScaleY(1, 0.1f));
        animation.Join(target.transform.DOPunchScale(new Vector3(0, 0.05f, 0), 0.3f, 2, 0.5f));
        animation.OnStart(() => target.transform.localScale = new Vector3(1, 0, 1));
        if (endAction != null) animation.OnComplete(() => endAction());

        return animation.SetUpdate(true);
    }

    public Sequence VerticalPunchDownward(GameObject target, Action endAction = null)
    {
        Sequence animation = DOTween.Sequence();

        animation.Append(target.transform.DOPunchScale(new Vector3(0, 0.05f, 0), 0.1f, 1, 0.5f));
        animation.Append(target.transform.DOScaleY(0, 0.1f));
        animation.OnStart(() => target.transform.localScale = Vector3.one);
        if (endAction != null) animation.OnComplete(() => endAction());

        return animation.SetUpdate(true);
    }

    public Sequence ScaleDown(GameObject target, Action endAction = null)
    {
        if (!target.TryGetComponent(out RectTransform rect)) return null;

        Sequence animation = DOTween.Sequence();

        animation.Append(rect.DOScale(0, 0.2f));
        animation.OnStart(() => rect.localScale = Vector3.one);
        if (endAction != null) animation.OnComplete(() => endAction());

        return animation.SetUpdate(true);
    }

    public Sequence FadeIn(GameObject target, Action endAction = null)
    {
        if (!target.TryGetComponent(out CanvasGroup group)) return null;

        Sequence animation = DOTween.Sequence();

        animation.Append(group.DOFade(1, 0.5f));
        animation.OnStart(() => group.alpha = 0);
        animation.OnStart(() => target.SetActive(true));
        if (endAction != null) animation.OnComplete(() => endAction());

        target.SetActive(true);
        return animation.SetUpdate(true);
    }

    public Sequence FadeOut(GameObject target, Action endAction = null)
    {
        if (!target.TryGetComponent(out CanvasGroup group)) return null;

        Sequence animation = DOTween.Sequence();

        animation.Append(group.DOFade(0, 0.5f));
        animation.OnStart(() => group.alpha = 1);
        if (endAction != null) animation.OnComplete(() => endAction());

        return animation.SetUpdate(true);
    }

    Dictionary<int, Vector3> originalLocalScales = new Dictionary<int, Vector3>();

    public Sequence PucnchScaleEmphasize(GameObject target)
    {
        if (originalLocalScales.ContainsKey(target.GetInstanceID()))
        {
            target.transform.localScale = originalLocalScales[target.GetInstanceID()];
        }
        else
        {
            originalLocalScales.Add(target.GetInstanceID(), target.transform.localScale);
        }

        Sequence animation = DOTween.Sequence();

        animation.Append(target.transform.DOPunchScale(Vector2.one * 0.15f, 0.3f, 2, 1));

        return animation.SetUpdate(true);
    }

    private Tween currentShakeTween;

    public void ShakeSummonResult()
    {
        if (currentShakeTween != null)
        {
            currentShakeTween.Kill();
            uI_SummonResult.anchoredPosition = ui_SummonResultOriginalPos;
        }

        currentShakeTween = uI_SummonResult.DOShakeAnchorPos(shakeDuration, shakeStrength, vibrato, randomness, false)
        .OnComplete(() =>
        {
            uI_SummonResult.anchoredPosition = ui_SummonResultOriginalPos;
            currentShakeTween = null;
        });
    }

    public void ShakeCamera()
    {
        if (camera == null)
        {
            Debug.LogError("Camera is not initialized");
            return;
        }

        camera.transform.DOShakePosition(.1f, .1f, vibrato, randomness, false);
    }

    public void ShakeObject(GameObject target)
    {
        target.transform.DOShakePosition(shakeDuration, shakeStrength, vibrato, randomness, false);
    }

    public void Hide(Image image)
    {

        image.gameObject.SetActive(true);
        Color color = image.color;
        color.a = 1;
        image.color = color;

        image.DOFade(0, .5f).SetDelay(.4f).OnComplete(() => image.gameObject.SetActive(false));
    }

    public void ShowRankEffect(Image image, Rank rank)
    {
        if (rank == Rank.Common) return;  // Early return if the rank is Common

        // Reset and prepare the image properties based on rank
        image.gameObject.SetActive(true);

        // Fetch color based on rank and set alpha to 0
        Color rankColor = ResourceManager.instance.rank.GetRankColor(rank);
        rankColor.a = 0;  // Set alpha to 0
        image.color = rankColor;  // Apply the color with alpha set to 0

        image.rectTransform.localScale = GetSizeForRank(rank);  // Get size based on rank
        image.rectTransform.localRotation = Quaternion.identity;  // Reset rotation

        // Create a sequence for the effect
        Sequence seq = DOTween.Sequence();

        // Initial delay before the animations start
        seq.PrependInterval(0.2f);  // Initial delay of 1 second (note: your comment says 0.3 seconds but code says 1f)

        // Fade in and rotate the image
        seq.Append(image.DOFade(.5f, 0.3f));  // Fade in from alpha 0 to 1
        seq.Join(image.rectTransform.DORotate(new Vector3(0, 0, 45), 0.4f, RotateMode.FastBeyond360));  // Rotate

        // Fade out after a brief display time
        seq.AppendInterval(0.1f);  // Display time before fading out
        seq.Append(image.DOFade(0, 0.1f));  // Fade out

        // On completion, hide the GameObject
        seq.OnComplete(() => image.gameObject.SetActive(false));
    }


    // public void ShowRankEffect(Image image, Rank rank)
    // {
    //     if (rank == Rank.Common) return;  // Early return if the rank is Common

    //     // Reset and prepare the image properties based on rank
    //     image.gameObject.SetActive(true);

    //     // Fetch color based on rank and initially set alpha to 0
    //     Color rankColor = ResourceManager.instance.rank.GetRankColor(rank);
    //     rankColor.a = 0;  // Set alpha to 0
    //     image.color = rankColor;  // Apply the color with alpha set to 0

    //     image.rectTransform.localScale = GetSizeForRank(rank);  // Get size based on rank
    //     image.rectTransform.localRotation = Quaternion.identity;  // Reset rotation

    //     // Create a sequence for the effect
    //     Sequence seq = DOTween.Sequence();

    //     // Initial delay before the animations start
    //     seq.PrependInterval(0.2f);

    //     // Fade in and rotate the image
    //     var fadeTween = image.DOFade(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);  // Fade in and out repeatedly
    //     seq.Append(fadeTween);

    //     if (rank >= Rank.Excellent)
    //     {
    //         // Continuous rotation for Epic or higher ranks
    //         var rotateTween = image.rectTransform.DORotate(new Vector3(0, 0, 360), 4f, RotateMode.FastBeyond360)
    //                             .SetLoops(-1, LoopType.Restart);  // Indefinite rotation
    //         seq.Join(rotateTween);
    //     }
    //     else
    //     {
    //         // Single rotation for non-Epic ranks
    //         var rotateTween = image.rectTransform.DORotate(new Vector3(0, 0, 360), 4f, RotateMode.FastBeyond360);
    //         seq.Join(rotateTween);

    //         // Sequence ends with a fade out and hide
    //         seq.AppendInterval(0.1f);  // Display time before fading out
    //         seq.Append(image.DOFade(0, 0.1f));  // Fade out
    //         seq.OnComplete(() => image.gameObject.SetActive(false));  // Hide after completion
    //     }
    // }



    private Vector2 GetSizeForRank(Rank rank)
    {
        switch (rank)
        {
            case Rank.Common: return new Vector3(1.75f, 1.75f);
            case Rank.Great: return new Vector3(1.75f, 1.75f);
            case Rank.Excellent: return new Vector3(2.25f, 2.25f);
            case Rank.Rare: return new Vector3(2.75f, 2.75f);
            default: return new Vector3(1, 1);
        }
    }

    public static Sequence DamageAnimation(DamageImage image)
    {
        image.TryGetComponent(out CanvasGroup group);
        Vector3 endPos = image.transform.localPosition + new Vector3(150, 0, 0);

        Sequence animation = DOTween.Sequence();
        animation.Append(image.transform.DOLocalJump(endPos, 40, 1, 1).OnComplete(image.AnimationEnd));
        animation.Join(group.DOFade(0, 0.2f).SetDelay(0.5f));

        return animation;
    }
}
