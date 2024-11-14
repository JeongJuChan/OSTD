using Keiwando.BigInteger;
using System.Collections;
using UnityEngine;

public interface ISkillDrop
{
    IEnumerator CoMoveToTarget(Monster target, bool isVibrated, BigInteger damage);
    IEnumerator CoAppear();
    void SetWallTriggerPosX(float wallTriggerposX);
}