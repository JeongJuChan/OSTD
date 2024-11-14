using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour, IPoolable<Effect>, IIndex
{
    private Action<Effect> OnReturnAction;

    [SerializeField] private Animator animator;

    [field: SerializeField] public int index { get; private set; }

    public void Initialize(Action<Effect> returnAction)
    {
        OnReturnAction = returnAction;
    }

    public void ReturnToPool()
    {
        OnReturnAction?.Invoke(this);
    }

    public void ReturnAfterAnimation()
    {
        StartCoroutine(CoReturnAfterAnimation());
    }

    private IEnumerator CoReturnAfterAnimation(int layerIndex = 0)
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        yield return CoroutineUtility.GetWaitForSeconds(animatorStateInfo.length);

        ReturnToPool();
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }
}
