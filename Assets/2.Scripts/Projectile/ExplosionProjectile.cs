using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExplosionProjectile : Projectile, IExplosion
{
    [SerializeField] protected CircleCollider2D circleCollider2D;
    protected float radius;
    protected float explosionPower;

    [SerializeField] protected GameObject rendererObject;

    protected WaitForSeconds disableDelaySeconds;

    protected bool isCollided;

    protected virtual void OnEnable()
    {
        rendererObject.SetActive(true);
        isCollided = false;
        circleCollider2D.enabled = true;
        rigid.freezeRotation = false;
        rigid.isKinematic = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        if (isCollided)
        {
            return;
        }

        if (other.gameObject.CompareTag(Consts.MONSTER_TAG) || other.gameObject.CompareTag(Consts.FLOOR_TAG))
        {
            isCollided = true;
            Explode();
        }
    }

    public virtual void SetRadius(float radius)
    {
        this.radius = radius;
    }

    public void SetExplosionPower(float explosionPower)
    {
        this.explosionPower = explosionPower;
    }

    public virtual void Explode()
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

                // if (collider.TryGetComponent(out Rigidbody2D rigidbody))
                // {
                //     rigidbody.AddForce((collider.transform.position - transform.position).normalized * explosionPower, ForceMode2D.Impulse);
                // }
            }
        }

        StartCoroutine(DisableDelay());
    }

    protected abstract IEnumerator DisableDelay();

    public override void Fire()
    {
        rigid.velocity = shotPower * transform.right;
    }
}
