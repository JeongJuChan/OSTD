using System;
using UnityEngine;

public class MonsterSpawnDataHandler : MonoBehaviour
{
    [Header("SpawnData")]
    private MonsterSpawnData spawnData;

    

    [SerializeField] private BoxCollider2D wallCollider;


    #region UnityMethods
    public void Init()
    {
        spawnData = GetSpawnData();
        InitDataToCastle();
        InitDataToHeroProjectileSpawner();
        InitDataToMonsterSpawner();
    }

    private void InitDataToMonsterSpawner()
    {
        // monsterSpawner.SetSpawnData(spawnData);
    }

    private void Start()
    {
        InitDataToTargetInActiveRangeTrigger();
    }
    #endregion

    #region InitMonsterSpawnDataMethods
    private void InitDataToHeroProjectileSpawner()
    {
    }

    private void InitDataToTargetInActiveRangeTrigger()
    {
        float colliderSizeY = spawnData.SpawnMaxPosition.y - spawnData.SpawnMinPosition.y + 2;
        Vector3 colliderPos = Consts.HALF * (spawnData.SpawnMaxPosition + spawnData.SpawnMinPosition);

        SetWallColliderSize(colliderSizeY);
    }

    private void SetWallColliderSize(float colliderSizeY)
    {
        Vector2 colliderSize = wallCollider.size;
        colliderSize.y = colliderSizeY;
        wallCollider.size = colliderSize;
    }

    private void InitDataToCastle()
    {
    }

    private MonsterSpawnData GetSpawnData()
    {
        float screenSpawnMaxPosY = Screen.height * 0.8f;
        float screenSpawnMinPosY = Screen.height * 0.5f;

        Vector2 maxPos = new Vector2(Screen.width, screenSpawnMaxPosY);
        Vector2 minPos = new Vector2(Screen.width, screenSpawnMinPosY);

        Vector3 spawnMinPosition = Camera.main.ScreenToWorldPoint(minPos);
        spawnMinPosition.z = 0;

        Vector3 spawnMaxPosition = Camera.main.ScreenToWorldPoint(maxPos);
        spawnMaxPosition.z = 0;

        return new MonsterSpawnData(spawnMinPosition, spawnMaxPosition);
    }
    #endregion


}
