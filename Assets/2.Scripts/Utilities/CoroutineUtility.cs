using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtility : MonoBehaviour
{
    private static Dictionary<float, WaitForSeconds> waitForSecondsDict = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWaitForSeconds(float time)
    {
        if (!waitForSecondsDict.ContainsKey(time))
        {
            waitForSecondsDict.Add(time, new WaitForSeconds(time));
        }

        return waitForSecondsDict[time];
    }
}
