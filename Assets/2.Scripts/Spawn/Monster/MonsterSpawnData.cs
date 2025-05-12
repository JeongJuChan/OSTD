using UnityEngine;

public struct MonsterSpawnData
{
    public MonsterSpawnData(Vector3 spawnMinPosition, Vector3 spawnMaxPosition)
    {
        SpawnMinPosition = spawnMinPosition;
        SpawnMaxPosition = spawnMaxPosition;
    }

    public Vector3 SpawnMinPosition { get; private set; }
    public Vector3 SpawnMaxPosition { get; private set; }
}