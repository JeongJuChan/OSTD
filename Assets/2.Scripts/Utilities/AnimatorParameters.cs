using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameters
{
    [Header("Common")]
    public static readonly int IS_ATTACK_HASH = Animator.StringToHash("IsAttacking");
    public static readonly int ATTACK_SPEED_RATE_HASH = Animator.StringToHash("AttackSpeedRate");
    public static readonly int IS_IDLE_HASH = Animator.StringToHash("IsIdle");

    [Header("Weapon")]
    public static readonly int START_TRIGGER_HASH = Animator.StringToHash("StartTrigger");

    [Header("Monster")]
    public static readonly int IS_STUNNED_HASH = Animator.StringToHash("IsStunned");
    public static readonly int IS_DEAD_HASH = Animator.StringToHash("IsDead");

    [Header("Boss")]
    public static readonly int ATTACK_INDEX = Animator.StringToHash("AttackIndex");

    [Header("Skill")]
    public static readonly int IS_TRIGGER_HASH = Animator.StringToHash("IsTrigger");

    [Header("Box")]
    public static readonly int BOX_LEVEL_UP = Animator.StringToHash("LevelUp");

    [Header("Hero")]
    public static readonly int HERO_ATTACK = Animator.StringToHash("Attack");
    public static readonly int GUN_LOAD = Animator.StringToHash("Gunload");
}
