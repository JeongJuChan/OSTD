using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCalculator
{
    private float energyRecoveryTime;
    private float currenElapsedTime;

    public event Action<float> OnUpdateInGameEnergyUI;
    public event Action OnIncreaseInGameEnergy;

    public void Update()
    {
        currenElapsedTime += Time.deltaTime;
        if (currenElapsedTime >= energyRecoveryTime)
        {
            currenElapsedTime -= energyRecoveryTime;
            OnIncreaseInGameEnergy?.Invoke();
        }

        OnUpdateInGameEnergyUI?.Invoke(currenElapsedTime / energyRecoveryTime);
    }

    public void UpdateEnergyRecoveryTime(float energyRecoveryTimePerSec)
    {
        this.energyRecoveryTime = 1 / energyRecoveryTimePerSec;
    }

    public void Reset()
    {
        currenElapsedTime = 0f;
    }
}
