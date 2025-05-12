using System.Collections;
using UnityEngine;

public abstract class Bullet : Projectile
{
    [SerializeField] private float disableDelayTime = 5f;
    [SerializeField] protected Collider2D collider2D;
    [SerializeField] protected ParticleSystem holeParticle;
    private WaitForSeconds holeDisableSeconds;
    protected bool isCollided;

    protected WaitForSeconds disableDelaySeconds;

    protected Coroutine holeCoroutine;

    protected virtual void OnEnable() 
    {
        isCollided = false;    
    }

    protected abstract void OnTriggerEnter2D(Collider2D other);

    public override void Init()
    {
        base.Init();
        InitPhysicsSetting();
        disableDelaySeconds = CoroutineUtility.GetWaitForSeconds(disableDelayTime);
        holeDisableSeconds = CoroutineUtility.GetWaitForSeconds(holeParticle.main.duration);
    }

    #region PhysicsSettingsMethods
    private void InitPhysicsSetting()
    {
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        collider2D.isTrigger = true;
    }

    #endregion
    protected virtual IEnumerator CoDisableDelay()
    {
        yield return disableDelaySeconds;
        ReturnToPool();
    }

    public override void Fire()
    {
        rigid.velocity = transform.right * shotPower;
        if (disableCoroutine != null)
        {
            StopCoroutine(CoDisableDelay());
        }
        disableCoroutine = StartCoroutine(CoDisableDelay());
    }

    protected void ShowHoleParticle(Collider2D other)
    {
        holeParticle.transform.position = other.ClosestPoint(transform.position);
        holeParticle.Play();

        if (holeCoroutine != null)
        {
            StopCoroutine(holeCoroutine);
        }
        holeCoroutine = StartCoroutine(CoDisalbeHoleDelay());
    }

    private IEnumerator CoDisalbeHoleDelay()
    {
        yield return holeDisableSeconds;
        ReturnToPool();
    }
}
