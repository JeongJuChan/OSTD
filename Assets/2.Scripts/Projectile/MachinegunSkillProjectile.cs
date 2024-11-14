using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinegunSkillProjectile : ExplosionProjectile
{
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleSystem explosionDustParticle;
    [SerializeField] private ParticleSystem skillBulletParticle;

    protected override void OnEnable()
    {
        base.OnEnable();
        skillBulletParticle.Play();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollided)
        {
            return;
        }

        if (other.gameObject.CompareTag(Consts.MONSTER_TAG))
        {
            isCollided = true;
            explosionParticle.Play();
            Explode();
        }
        else if(other.gameObject.CompareTag(Consts.FLOOR_TAG))
        {
            isCollided = true;
            explosionParticle.Play();
            explosionDustParticle.Play();
            Explode();
        }
    }

    public override void Init()
    {
        base.Init();
        disableDelaySeconds = CoroutineUtility.GetWaitForSeconds(explosionParticle.main.duration);
    }

    protected override IEnumerator DisableDelay()
    {
        explosionParticle.Play();
        rendererObject.SetActive(false);
        transform.rotation = Quaternion.identity;
        rigid.velocity = Vector2.zero;
        rigid.freezeRotation = true;
        circleCollider2D.enabled = false;
        rigid.isKinematic = true;

        yield return disableDelaySeconds;

        ReturnToPool();
    }
}
