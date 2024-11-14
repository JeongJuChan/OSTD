using Keiwando.BigInteger;
using System.Collections;
using UnityEngine;

public abstract class AnimatedSkill : Skill, ISkillAniamted
{
    protected Animator animator { get; private set; }

    protected WaitForSeconds animationDuration { get; private set; }

    protected float duration;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void DeActivateAsAnimationFinished(int layerIndex)
    {
        StartCoroutine(CoWaitUntilAnimationFinished(layerIndex));
    }

    protected IEnumerator CoWaitUntilAnimationFinished(int layerIndex)
    {
        if (animationDuration == null)
        {
            animationDuration = CoroutineUtility.GetWaitForSeconds(duration);
        }

        yield return animationDuration;

        gameObject.SetActive(false);
    }

    public float GetAnimationDuraiton(int layerIndex)
    {
        if (duration == 0f)
        {
            duration = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
            if (duration == 0f)
            {
                duration = 1f;
            }
            animationDuration = CoroutineUtility.GetWaitForSeconds(duration);
        }

        return duration;
    }
}
