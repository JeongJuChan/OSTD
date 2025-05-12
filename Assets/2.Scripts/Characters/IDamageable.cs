using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    event Action OnDamaged;
    void TakeDamage(BigInteger damage);
}
