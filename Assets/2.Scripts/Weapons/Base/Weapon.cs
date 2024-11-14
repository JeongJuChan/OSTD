using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Weapon : MonoBehaviour, IUsingSkill, IPoolable<Weapon>, IIndex
{
    protected WeaponData weaponData;
    private Action<Weapon> OnReturnAction;
    
    [Header("Animator")]
    protected Animator animator;
    [SerializeField] protected Animator[] animators;
    [field: SerializeField] public float skillDuration { get; protected set; } = 3f;

    [field: SerializeField] public int index { get; private set; }

    [SerializeField] private SortingGroup sortingGroup;

    protected virtual void OnDisable() 
    {
        if (animator != null)
        {
            animator = animators[0];
            animator.SetTrigger(AnimatorParameters.START_TRIGGER_HASH);
        }

        transform.rotation = Quaternion.identity;

        sortingGroup.sortingLayerName = Consts.PLAYER_LAYER;    
    }

    public abstract void Init();

    public void Initialize(Action<Weapon> returnAction)
    {
        OnReturnAction = returnAction;
    }

    public void ReturnToPool()
    {
        OnReturnAction?.Invoke(this);
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public virtual void UpdateWeaponData(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        int index = weaponData.level - 1;
        index = index >= animators.Length ? animators.Length - 1 : index;
        animator = animators[index];

        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].gameObject.SetActive(i == index);
        }

        animator.gameObject.SetActive(true);
    }

    public WeaponType GetWeaponType()
    {
        return weaponData.weaponType;
    }

    public virtual void UpdateWeaponDragSettings(bool isDragging)
    {
        sortingGroup.sortingLayerName = isDragging ? Consts.POPUP_UI_LAYER : Consts.PLAYER_LAYER;
    }

    public abstract void UseSkill();
}
