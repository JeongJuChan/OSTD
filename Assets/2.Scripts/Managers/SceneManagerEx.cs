using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviourSingleton<SceneManagerEx>
{
    private AsyncOperation currentOperation;

    private const float MINIMUM_WAIT_DURATION = 1f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PrepareSceneLoaded(SceneType sceneType, Action<float> OnUpdateProgress,  Action OnLoadComplete)
    {
        StartCoroutine(CoLoadSceneAsync(sceneType, OnUpdateProgress, OnLoadComplete));
    }

    public void LoadSceneLoaded()
    {
        currentOperation.allowSceneActivation = true;
    }

    private IEnumerator CoLoadSceneAsync(SceneType sceneType, Action<float> onUpdateProgress, Action OnLoadComplete)
    {
        yield return null;

        currentOperation = SceneManager.LoadSceneAsync((int)sceneType);

        currentOperation.allowSceneActivation = false;

        float elapsedTime = Time.deltaTime;

        while (currentOperation.progress < 0.9f || elapsedTime < MINIMUM_WAIT_DURATION)
        {
            Debug.Log(elapsedTime);
            float ratio = currentOperation.progress <= elapsedTime ? elapsedTime : currentOperation.progress;
            ratio = ratio > 1f ? 1f : ratio;
            onUpdateProgress?.Invoke(ratio);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        OnLoadComplete?.Invoke();
    }
}
