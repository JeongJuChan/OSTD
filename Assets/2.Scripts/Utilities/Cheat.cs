using UnityEngine;

public class Cheat : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            HeroManager.instance.hero.Die(true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Research, 10);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gem, 100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.EnforcePowder, 100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ShotGunBluePrint, 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.GranadeBluePrint, 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.CapBluePrint, 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ArmorBluePirnt, 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, 50);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, 100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, 10000);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.SawCurrency, 10);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.FlameThrowerCurrency, 10);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.MachineGunCurrency, 10);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ShockerCurrency, 10);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.LaserCurrency, 10);
        }
        

        if (Input.GetKeyDown(KeyCode.N))
        {
            StageManager.instance.DestroyBasementAll();
        }
    }

    #region SkillCheat
    #endregion

    #region StageCheat
    public void KillCastle()
    {
        //castle.TakeDamage(int.MaxValue);
    }

    // public void SkipCurrentRoutineStage()
    // {
    //     stageController.EditorSkipCurrentRoutineStage();
    // }

    // public void SkipCurrentSubStage()
    // {
    //     stageController.EditorSkipCurrentSubStage();
    // }

    // public void ChallengeBoss()
    // {
    //     stageController.EditorChallengeBoss();
    // }

    // public void SkipCurrentMainStage()
    // {
    //     stageController.EditorSkipCurrentMainStage();
    // }
    #endregion
#endif
}
