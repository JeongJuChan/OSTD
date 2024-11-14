using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Keiwando.BigInteger;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IIndex, IPoolable<Projectile>
{
    [Header("MoveFields")]
    protected float shotPower;
    
    protected Transform myTransform;
    public Action<EffectType, Vector2> OnEffectSpawned;
    public Action<IDamageable, Vector2> OnAttacked;

    [field: SerializeField] public int index { get; private set; }

    [Header("Physics2D")]
    [SerializeField] protected Rigidbody2D rigid;

    [field: Header("Pool")]
    protected Action<Projectile> returnAction;

    protected Coroutine disableCoroutine;

    protected BigInteger damage;


    #region MoveSettingMethods
    public virtual void Init()
    {
        myTransform = GetComponent<Transform>();
    }

    public void SetShotPower(float shotPower)
    {
        this.shotPower = shotPower;
    }

    public void UpdateDamage(BigInteger damage)
    {
        this.damage = damage;
    }
    
    #endregion

    public void SetIndex(int index)
    {
        this.index = index;
    }

    #region Pooling
    public void ReturnToPool()
    {
        returnAction?.Invoke(this);
    }

    public void Initialize(Action<Projectile> returnAction)
    {
        this.returnAction = returnAction;
    }
    #endregion

    public abstract void Fire();
}
