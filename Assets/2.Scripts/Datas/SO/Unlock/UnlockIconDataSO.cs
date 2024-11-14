using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockIconData", menuName = "SO/UnlockIconData")]
public class UnlockIconDataSO : ScriptableObject
{ 
    [field: SerializeField] public List<UnlockIconData> unlockIconDatas { get; private set; } = new List<UnlockIconData>();

    private Dictionary<FeatureID, UnlockIconData> unlockIconDataDict = new Dictionary<FeatureID, UnlockIconData>();

    public event Action<UnlockIconData> OnUpdateUnlockIcon = null;

    public void Init()
    {
        UnlockManager.instance.OnUpdateUnlockUI += UpdateUnlockIcon;

        foreach (UnlockIconData unlockIconData in unlockIconDatas)
        {
            if (!unlockIconDataDict.ContainsKey(unlockIconData.featureID))
            {
                unlockIconDataDict.Add(unlockIconData.featureID, new UnlockIconData(unlockIconData.featureID, 
                    EnumToKRManager.instance.GetEnumToKR(unlockIconData.featureID), unlockIconData.sprite));
            }
        }
    }

    private void UpdateUnlockIcon(FeatureID featureID)
    {
        if (unlockIconDataDict.ContainsKey(featureID))
        {
            OnUpdateUnlockIcon?.Invoke(unlockIconDataDict[featureID]);
        }
    }
}
