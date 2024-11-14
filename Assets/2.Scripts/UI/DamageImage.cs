using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Color[] imageColors;
    [SerializeField] private Color[] textColors;

    private CanvasGroup group;
    private Vector3 originalScale = Vector3.one;
    private Sequence anim;

    public event Action<DamageImage> OnAnimationEnd;


    public void ClearCallback()
    {
        OnAnimationEnd = null;
    }

    public void ShowDamage(string amount, int damageType, int direction)
    {
        text.text = amount;
        text.color = textColors[damageType];
        image.color = imageColors[damageType];

        transform.localScale = new Vector3(originalScale.x * direction, originalScale.y, originalScale.z);

        if (!gameObject.activeSelf)
        {
            AnimationEnd();
            return;
        }

        anim = UIAnimations.DamageAnimation(this);
        anim.Restart();
    }

    public void AnimationEnd()
    {
        if (!group) TryGetComponent<CanvasGroup>(out group);
        image.transform.localPosition = Vector3.zero;
        group.alpha = 0.8f;
        transform.localScale = originalScale;
        OnAnimationEnd?.Invoke(this);
    }
}
