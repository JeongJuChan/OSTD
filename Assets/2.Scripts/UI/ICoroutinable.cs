using System;
using System.Collections;
using UnityEngine;

public interface ICoroutinable<T> where T : MonoBehaviour
{
    public Action<CoroutinableUI, bool, Action> OnUpdateCoroutineState { get; set; }
    Coroutine currentCoroutine { get; set; }
}