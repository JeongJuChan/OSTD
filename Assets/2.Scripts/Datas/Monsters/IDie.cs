using System;
using UnityEngine;

public interface IDie
{
    public Action OnDead { get; set; }
    ParticleSystem deadEffect { get; set; }
    void Die(bool isCausedByBattle);
}