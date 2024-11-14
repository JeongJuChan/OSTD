using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviourSingleton<CameraManager>
{
    private Transform boxManagerTransform;

    private GameManager gameManager;

    [SerializeField] private float speed = 5f;

    [SerializeField] private float cameraInGameMovingX = 1f;
    [field: SerializeField] public float cameraLobbyMovingX { get; private set; } = 0.5f;

    private float diffPosX;

    private float cameraZPos;

    private Vector3 offsetPos;

    public void Init()
    {
        boxManagerTransform = BoxManager.instance.transform;
        offsetPos = transform.position;
        gameManager = GameManager.instance;
        gameManager.OnReset += UpdateLobbyPos;
        diffPosX = offsetPos.x - boxManagerTransform.position.x;
        UpdateLobbyPos();
    }

    private void LateUpdate()
    {
        if (GameManager.instance.isGameState)
        {
            UpdateCameraPos();
        }
    }

    private void UpdateLobbyPos()
    {
        Vector3 cameraPos = offsetPos;
        cameraPos.x -= cameraLobbyMovingX;
        transform.position = cameraPos;
    }

    private void UpdateCameraPos()
    {
        Vector3 cameraPos = offsetPos;
        cameraPos.x = boxManagerTransform.position.x + diffPosX + cameraInGameMovingX;
        transform.position = Vector3.Lerp(transform.position, cameraPos, speed * Time.deltaTime);
    }

}
