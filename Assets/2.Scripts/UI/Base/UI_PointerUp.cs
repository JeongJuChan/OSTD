using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_PointerUp : UI_Base, IPointerEnterHandler, IPointerExitHandler
{
    protected GameObject enteredObject;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        enteredObject = eventData.pointerEnter;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        enteredObject = null;
    }
}
