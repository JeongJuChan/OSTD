using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/PoolInfoSO", fileName = "PoolInfoSO")]
// TODO: 다음 메인 스테이지로 넘어갈 때 Pool에 등록된 몬스터들 할당 해제하고 다음 것 등록해야 함
public class PoolInfoSO : ScriptableObject
{
    [field: SerializeField] public int Index { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int InitCount { get; private set; } = 10;
    [field: SerializeField] public int MaxCount { get; private set; } = 100;

    public void SetIndex(int index)
    {
        Index = index;
    }
}
