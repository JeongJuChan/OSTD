using System;
using System.Collections;
using UnityEngine;

public class BlinkCalculator<T> where T: MonoBehaviour
{
    private T t;
    private WaitForSeconds blinkWaitForSeconds;

    private Coroutine preCoroutine;

    private Material[] materials;

    public bool isBlinkable = true;

    private const string FLASH_AMOUNT = "_FlashAmount";

    private SpriteRenderer[] spriteRenderers;

    public BlinkCalculator(T t, SpriteRenderer[] spriteRenderers, float blinkDuration)
    {
        Material offsetMat = ResourceManager.instance.effect.GetMaterial(EffectMaterialType.DamageFlash);

        this.spriteRenderers = spriteRenderers;

        materials = new Material[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer spriteRenderer = spriteRenderers[i];
            Material material = new Material(offsetMat);
            material.SetTexture("_Texture2D", spriteRenderer.sprite.texture);
            spriteRenderer.sharedMaterial = material;
            materials[i] = material;
        }

        this.t = t;

        blinkWaitForSeconds = CoroutineUtility.GetWaitForSeconds(blinkDuration);

        if (t.TryGetComponent(out IDamageable damagable))
        {
            damagable.OnDamaged += Blink;
        }
        if (t.TryGetComponent(out IAlive alive))
        {
            alive.OnAlive += ResetBlink;
        }
    }

    private void Blink()
    {
        if (!isBlinkable)
        {
            return;
        }

        if (t.gameObject.activeInHierarchy)
        {
            preCoroutine = t.StartCoroutine(CoBlink());
        }
    }

    private IEnumerator CoBlink()
    {
        isBlinkable = false;
        // spriteRenderer.color = Color.red;
        foreach (Material material in materials)
        {
            material.SetFloat(FLASH_AMOUNT, 0.3f);
        }
        
        yield return blinkWaitForSeconds;
        isBlinkable = true;
        ResetBlink();
    }

    public void ResetBlink()
    {
        if (preCoroutine != null)
        {
            t.StopCoroutine(preCoroutine);
        }

        foreach (Material material in materials)
        {
            material.SetFloat(FLASH_AMOUNT, 0);
        }
    }

    public void UpdateTextures()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            materials[i].SetTexture("_Texture2D", spriteRenderers[i].sprite.texture);
        }
    }
}