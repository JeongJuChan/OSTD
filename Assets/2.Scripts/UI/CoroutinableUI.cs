using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoroutinableUI : UI_Popup, ICoroutinable<CoroutinableUI>
{
    public Coroutine currentCoroutine { get; set; }
    public Action<CoroutinableUI, bool, Action> OnUpdateCoroutineState { get; set; }
}
