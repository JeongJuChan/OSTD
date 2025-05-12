using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class Granade : ExplosionProjectile
{
    [Header("Coroutine")]
    private WaitForSeconds explosionDelaySeconds;

    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private float explosionDuration = 3f;

    private Vector2 shootDir = new Vector2(0.5f, 1).normalized;

    [SerializeField] private float angulerDragAfterCollided = 5f;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private float offsetAngularDrag;

    public event Action OnShakeCamera;

    protected override void OnEnable()
    {
        base.OnEnable();
        rigid.angularDrag = offsetAngularDrag;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        rigid.angularDrag = angulerDragAfterCollided;    
    }

    public void SetDelayTime(float delayTime)
    {
        explosionDelaySeconds = CoroutineUtility.GetWaitForSeconds(delayTime);
    }

    public override void Init()
    {
        base.Init();
        disableDelaySeconds = CoroutineUtility.GetWaitForSeconds(explosionParticle.main.duration);
        offsetAngularDrag = rigid.angularDrag;
    }

    protected override IEnumerator DisableDelay()
    {
        rigid.velocity = shotPower * shootDir;
        yield return explosionDelaySeconds;
        Explode();
        rendererObject.SetActive(false);
        explosionParticle.Play();

        transform.rotation = Quaternion.identity;
        rigid.velocity = Vector2.zero;
        rigid.freezeRotation = true;
        circleCollider2D.enabled = false;
        rigid.isKinematic = true;

        yield return disableDelaySeconds;

        ReturnToPool();
    }

    public override void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D collider in colliders)
        {
            if (collider == circleCollider2D)
            {
                continue;
            }

            if (collider.CompareTag(Consts.MONSTER_TAG))
            {
                if (collider.TryGetComponent(out MonsterBase monster))
                {
                    if (!monster.isDead)
                    {
                        BattleManager.instance.OnMonsterAttacked(monster, monster.GetDamageTextPivot().position, false, damage);
                    }
                }
            }
        }

        OnShakeCamera?.Invoke();
    }

    public override void Fire()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        disableCoroutine = StartCoroutine(DisableDelay());
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
