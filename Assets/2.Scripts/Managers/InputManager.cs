using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    public bool isDragging { get; private set; }

    public void SetIsDragging(bool isDragging)
    {
        this.isDragging = isDragging;
    }
}
