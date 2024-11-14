using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageImageSpawner
{
    private DamageImage prefab;
    private Transform parent;

    private Queue<DamageImage> imagePool = new Queue<DamageImage>();
    private HashSet<DamageImage> imageOnField = new HashSet<DamageImage>();

    public void SetPrefab(DamageImage prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        BattleManager.instance.OnSpawnDamageUI += ShowDamage;

        for (int i = 0; i < 250; i++)
        {
            DamageImage image = UnityEngine.Object.Instantiate(prefab, parent);
            image.OnAnimationEnd += ReturnToPool;
            ReturnToPool(image);
        }
    }

    private void ShowDamage(BigInteger damage, DamageType damageType, int direction, Vector2 pos)
    {
        DamageImage image;

        if (imagePool.Count > 0)
        {
            image = imagePool.Dequeue();
        }
        else
        {
            image = UnityEngine.Object.Instantiate(prefab, parent);
            image.OnAnimationEnd += ReturnToPool;
        }

        image.gameObject.SetActive(true);
        image.transform.position = pos;
        image.ShowDamage(damage.ChangeMoney(), (int)damageType, direction);
    }

    private void ReturnToPool(DamageImage image)
    {
        image.gameObject.SetActive(false);
        imageOnField.Remove(image);
        imagePool.Enqueue(image);
    }
}
