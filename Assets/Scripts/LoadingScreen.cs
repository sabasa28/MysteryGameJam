using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Transform loadingBar;
    [SerializeField] Vector3 lowPos;
    [SerializeField] Vector3 highPos;
    [SerializeField] float timeBeforeLoad;
    Vector3 initialPos;
    Vector3 finalPos;
    float timer = 0.0f;
    [SerializeField] float minimumLoadingTime;
    AsyncOperation loadingScene;
    AsyncOperation unloadingScene;
    bool loading = true;
    bool unloading = false;
    void Start()
    {
        loadingScene = SceneManager.LoadSceneAsync(LevelsManager.Get().GetNextSceneName(), LoadSceneMode.Additive);
        loadingScene.allowSceneActivation = false;
        initialPos = LevelsManager.Get().GoingUp ? lowPos : highPos;
        finalPos = LevelsManager.Get().GoingUp ? highPos : lowPos;
        transform.localPosition = initialPos;
    }

    private void Update()
    {
        if (unloading)
        {
            return;
        }
        if (timer < minimumLoadingTime + timeBeforeLoad && ((timer - timeBeforeLoad) / minimumLoadingTime < loadingScene.progress / 0.9f))
        {
            timer += Time.deltaTime;
        }
        if (timer > timeBeforeLoad)
        {
            if (loading)
            {
                float t = Mathf.Min((timer - timeBeforeLoad) / minimumLoadingTime, loadingScene.progress / 0.9f);
                loadingBar.localPosition = Vector3.Lerp(initialPos, finalPos, t);
                if (t >= 1.0f)
                {
                    loadingScene.allowSceneActivation = true;
                    loading = false;
                }
            }
            else if (loadingScene != null && loadingScene.isDone)
            {
                unloadingScene = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("LoadingScreenScene"));
                unloading = true;
                LevelsManager.Get().OnLoadingNextSceneCompleted();
            }
        }
    }

}
