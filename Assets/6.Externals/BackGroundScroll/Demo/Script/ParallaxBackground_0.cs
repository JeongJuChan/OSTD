using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxBackground_0 : MonoBehaviour
{
    public bool Camera_Move;
    public float Camera_MoveSpeed = 1.5f;

    [Header("Layer Setting")]
    public float[] Layer_Speed = new float[2];
    public GameObject[] Layer_Objects = new GameObject[2];

    [SerializeField] private SpriteRenderer[] loadRenderers;
    [SerializeField] private SpriteRenderer[] backgroundRenderers;

    private Transform _camera;
    private float[] startPos = new float[2];
    private float[] startYPos = new float[2];  // 각 레이어의 초기 y 위치를 저장할 배열
    private float boundSizeX;
    private float sizeX;

    void Start()
    {
        _camera = Camera.main.transform;
        sizeX = Layer_Objects[0].transform.localScale.x;
        boundSizeX = Layer_Objects[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        for (int i = 0; i < 2; i++)
        {
            startPos[i] = _camera.position.x;
            startYPos[i] = Layer_Objects[i].transform.position.y;  // 초기 y 위치를 저장
        }

        // TODO: 하드코딩 지우기
        int mainStageNum = DataBaseManager.instance.Load(Consts.MAIN_STAGE_NUM, 1);
        int currentMainStageNum = mainStageNum % 3;
        currentMainStageNum = currentMainStageNum == 0 ? 3 : currentMainStageNum;
        (Sprite, Sprite) backgroundImages = ResourceManager.instance.stage.GetBackgroundSprites(currentMainStageNum);
        UpdateSprites(backgroundImages.Item1, backgroundImages.Item2);
        StageManager.instance.OnUpdateBackgroundSprite += UpdateSprites;
    }

    void Update()
    {
        // Moving camera
        // if (Camera_Move)
        // {
        //     _camera.position += Vector3.right * Time.deltaTime * Camera_MoveSpeed;
        // }

        for (int i = 0; i < 2; i++)
        {
            float temp = (_camera.position.x * (1 - Layer_Speed[i]));
            float distance = _camera.position.x * Layer_Speed[i];

            // Y 위치를 초기 값인 startYPos[i]로 유지하면서 x축 위치만 변경
            Layer_Objects[i].transform.position = new Vector2(startPos[i] + distance, startYPos[i]);

            if (temp > startPos[i] + boundSizeX * sizeX)
            {
                startPos[i] += boundSizeX * sizeX;
            }
            else if (temp < startPos[i] - boundSizeX * sizeX)
            {
                startPos[i] -= boundSizeX * sizeX;
            }
        }
    }

    private void UpdateSprites(Sprite loadSprite, Sprite backgroundSprite)
    {
        for (int i = 0; i < loadRenderers.Length; i++)
        {
            loadRenderers[i].sprite = loadSprite;
        }

        for (int i = 0; i < backgroundRenderers.Length; i++)
        {
            backgroundRenderers[i].sprite = backgroundSprite;
        }
    }
}
