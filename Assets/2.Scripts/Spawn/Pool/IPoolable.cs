using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable<T>
{
    void ReturnToPool();
    void Initialize(Action<T> returnAction);
}