using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMoveController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private float offsetSpeed = 1f;

    [Header("WheelRotation")]
    [SerializeField] private Transform[] wheels;
    [SerializeField] private float wheelRotationValue = 135f;

    [field: SerializeField] public float currentSpeed { get; private set; }

    private Vector2 boxColliderOffsetSize;

    public bool isMonsterBasementEncountered { get; private set; }

    public event Action<float> OnUpdateBoxPosX;

    [SerializeField] private VerticalWall verticalWall;

    private bool isMoving;

    private GameManager gameManager;
    private StageManager stageManager;

    private Vector2 offsetPos;

    public event Action<float> OnUpdateVelocity;

    private HashSet<MonsterCollisionHandler> monsters = new HashSet<MonsterCollisionHandler>();

    private readonly RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[20];
    private float stopModByMonster = 1.25f;

    private float speedDecreasingValue = 0.2f;

    private void FixedUpdate() 
    {
        TryUpdateMovingState();
    }

    private void Update() 
    {
        if (isMoving)
        {
            Move();
            OnUpdateBoxPosX?.Invoke(transform.position.x);
        }
    }

    #region Initialize
    public void Init()
    {
        currentSpeed = offsetSpeed;
        gameManager = GameManager.instance;
        UpdateMovingState(false);
        offsetPos = transform.position;
        gameManager.OnReset += Reset;
        stageManager = StageManager.instance;
        stageManager.OnClearStage += () => UpdateMovingState(false);
        verticalWall.Init();
        gameManager.OnStart += () => UpdateMovingState(true);
    }
    #endregion

    #region ChangeState
    public void UpdateMovingState(bool isMoving)
    {
        if (isMoving)
        {
            if (HeroManager.instance.hero.isDead)
            {
                return;
            }
        }
        this.isMoving = isMoving;
    }

    private void Move()
    {
        Vector2 pos = transform.position;
        pos.x += currentSpeed * Time.deltaTime;
        transform.position = pos;
    }

    public void SetBoxUnitSize(Vector2 size)
    {
        boxColliderOffsetSize = size;
        boxCollider2D.size = size;
    }

    public void UpdateBoxSize(int count)
    {
        Vector2 offsetPos = boxCollider2D.offset;
        offsetPos.y = boxColliderOffsetSize.y * Consts.HALF * count;
        boxCollider2D.offset = offsetPos;

        Vector2 boxSize = boxColliderOffsetSize;
        boxSize.y += boxColliderOffsetSize.y * count;
        boxCollider2D.size = boxSize;
    }

    public void UpdateMonsterBasementEncounterState(bool isEncountered)
    {
        isMonsterBasementEncountered = isEncountered;
    }
    #endregion

    private void TryUpdateMovingState()
    {
        if (!GameManager.instance.isGameState || isMonsterBasementEncountered || StageManager.instance.isClearPopupShowed)
        {
            return;
        }

        UpdateMovingState();
    }

    private void UpdateMovingState()
    {
        Vector2 originPos = transform.position;
        originPos.x += boxCollider2D.size.x * Consts.HALF * stopModByMonster;
        Vector2 direction = Vector2.up;
        float distance = float.MaxValue;

        int count = Physics2D.RaycastNonAlloc(originPos, direction, raycastHit2Ds, distance, Consts.LAYER_1 | Consts.LAYER_2 | Consts.LAYER_3);
        #if UNITY_EDITOR
        Debug.DrawRay(originPos, direction * distance);
        #endif

        int monsterCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (raycastHit2Ds[i].collider != null && raycastHit2Ds[i].collider.CompareTag(Consts.MONSTER_TAG))
            {
                monsterCount++;
                if (monsterCount >= 5)
                {
                    currentSpeed = 0f;
                    UpdateMovingState(false);
                    RotateWheel();
                    return;
                }
            }
        }

        if (monsterCount < 5)
        {
            float speed = offsetSpeed * (1 - speedDecreasingValue * monsterCount);
            currentSpeed = currentSpeed - Time.deltaTime * offsetSpeed * speed;
            currentSpeed = currentSpeed >= speed ? currentSpeed : speed;
        }

        UpdateMovingState(true);
        RotateWheel();
    }


    public void Reset()
    {
        UpdateMovingState(false);
        transform.position = offsetPos;
    }

    private void RotateWheel()
    {
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(new Vector3(0, 0, -wheelRotationValue * currentSpeed * Time.deltaTime));
        }
    }
}
