using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatLockElement : MonoBehaviour
{
    [SerializeField] private FeatureID featureID;

    public void InitUnlock()
    {
        bool isLocked = ES3.Load($"{name}{Consts.IS_LOCKED}", true);

        if (isLocked)
        {
            UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(featureID);

            Debug.Log(unlockData);

            UnlockManager.instance.RegisterFeature(new UnlockableFeature(unlockData.featureType, unlockData.featureID, unlockData.count, () =>
            {
                isLocked = false;

                gameObject.SetActive(true);

                ES3.Save($"{name}{Consts.IS_LOCKED}", isLocked, ES3.settings);

                ES3.StoreCachedFile();
            }));
        }
    }
}
