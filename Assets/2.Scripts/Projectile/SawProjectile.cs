using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawProjectile : ShootingProjectile
{
    [Header("Animator")]
    protected Animator animator;
    [SerializeField] protected Animator[] animators;

    public override void Fire()
    {
        base.Fire();
        
    }

    public void UpdateAnimIndex(int index)
    {
        if (animators.Length <= index)
        {
            return;
        }

        if (index >= 0)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                if (i == index)
                {
                    animator = animators[i];
                    animators[i].gameObject.SetActive(true);
                }
                else
                {
                    animators[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
