using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    public void ChangeActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
