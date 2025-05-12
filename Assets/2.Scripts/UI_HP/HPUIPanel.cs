using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Keiwando.BigInteger;

public class HPUIPanel : MonoBehaviour
{
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected Image damageFill;

    protected BigInteger maxHP = new BigInteger(0);
    protected BigInteger preHP = new BigInteger(0);

    protected float width;
    protected float height;
    protected RectTransform damageRect;

    [SerializeField] protected float damageFillDecresingDuration = 1f;
    protected Coroutine preCoDamageFillDecrement;

    protected IHasHpUI hasHPUI;

    [SerializeField] private bool isVertical;

    public virtual void init(IHasHpUI hasHPUI)
    {
        this.hasHPUI = hasHPUI;
        damageRect = damageFill.rectTransform;
        width = damageRect.sizeDelta.x;
        height = damageRect.sizeDelta.y;

        hasHPUI.OnResetHPUI += ResetUI;
        hasHPUI.OnUpdateMaxHPUI += UpdateMaxHP;
        hasHPUI.OnUpdateCurrenHPUI += UpdateCurrentHPUI;
        hasHPUI.OnActiveHpUI += ActiveSelf;
    }

    public virtual void ResetUI()
    {
        hpSlider.value = 1;
        SwitchDamageFillActive(false);
        preHP = maxHP;
    }

    public virtual void UpdateMaxHP(BigInteger maxHP, BigInteger currentHp)
    {
        this.maxHP = maxHP;
        preHP = currentHp;
        UpdateCurrentHPUI(preHP);
    }

    public virtual void UpdateCurrentHPUI(BigInteger currentHp)
    {
        if (maxHP == 0)
        {
            return;
        }
        float ratio = currentHp.ToFloat() / maxHP.ToFloat();
        hpSlider.value = ratio;
        if (preHP > currentHp)
        {
            if (preCoDamageFillDecrement != null)
            {
                StopCoroutine(preCoDamageFillDecrement);
            }

            SwitchDamageFillActive(true);
            InitDamageFillSize(currentHp);
            SetDamageFillPos(ratio);
            if (gameObject.activeInHierarchy)
            {
                preCoDamageFillDecrement = StartCoroutine(CoDamageFillDecrease());
            }
        }

        preHP = currentHp;
    }

    private void SetDamageFillPos(float currentValue)
    {
        Vector2 position = damageRect.anchoredPosition;
        if (isVertical)
        {
            position.y = height * currentValue;
        }
        else
        {
            position.x = width * currentValue;
        }
        damageRect.anchoredPosition = position;
    }

    private void InitDamageFillSize(BigInteger currentHp)
    {
        Vector2 size = damageRect.sizeDelta;
        if (isVertical)
        {
            size.y = height * ((preHP - currentHp).ToFloat() / maxHP.ToFloat());
        }
        else
        {
            size.x = width * ((preHP - currentHp).ToFloat() / maxHP.ToFloat());
        }
        damageRect.sizeDelta = size;
    }

    private IEnumerator CoDamageFillDecrease()
    {
        float elapsedTime = Time.deltaTime;

        float ratio = elapsedTime / damageFillDecresingDuration;

        while (ratio < 1f)
        {
            SetDamageFillSize(ratio);
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / damageFillDecresingDuration;
            yield return null;
        }

        SwitchDamageFillActive(false);
        preCoDamageFillDecrement = null;
    }

    private void SetDamageFillSize(float ratio)
    {
        Vector2 size = damageRect.sizeDelta;
        if (isVertical)
        {
            size.y = Mathf.Lerp(size.y, 0, ratio);
        }
        else
        {
            size.x = Mathf.Lerp(size.x, 0, ratio);
        }
        damageRect.sizeDelta = size;
    }

    private void SwitchDamageFillActive(bool isActive)
    {
        damageFill.gameObject.SetActive(isActive);
    }

    private void ActiveSelf(bool isActive)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
        }
        else
        {
            if (preCoDamageFillDecrement != null)
            {
                StopCoroutine(preCoDamageFillDecrement);
            }

            gameObject.SetActive(false);
        }
    }
}
