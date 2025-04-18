using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    static LevelsManager instance;
    public enum Scenes
    {
        Surface,
        HigherCave,
        LowerCave
    }
    Scenes nextScene = Scenes.Surface;
    [SerializeField] Scenes currentScene = Scenes.Surface;
    public bool GoingUp = true;
    public PersistentData persistentData; //we keep this in this persistent object so it won't be deleted when unloading a scene
    public List<Vector3> beaconsPosInLoadedLevel = new();
    public static LevelsManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (!instance)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            nextScene = currentScene;
            persistentData.ClearData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void LoadHigherCavesScene()
    {
        GoingUp = nextScene > Scenes.HigherCave;
        nextScene = Scenes.HigherCave;
        SceneManager.LoadScene("LoadingScreenScene");
    }

    public void LoadLowerCavesScene()
    {
        GoingUp = nextScene > Scenes.LowerCave;
        nextScene = Scenes.LowerCave;
        SceneManager.LoadScene("LoadingScreenScene");
    }

    public void LoadSurfaceCavesScene()
    {
        GoingUp = nextScene > Scenes.Surface;
        nextScene = Scenes.Surface;
        SceneManager.LoadScene("LoadingScreenScene");
    }

    string GetScene(Scenes scene)
    {
        switch (scene)
        {
            case Scenes.Surface:
                return "SurfaceScene";
            case Scenes.HigherCave:
                return "HigherCaveScene";
            case Scenes.LowerCave:
                return "LowerCaveScene";
            default:
                return "SurfaceScene";
        }
    }

    public string GetNextSceneName()
    {
        return GetScene(nextScene);
    }

    public string GetCurrentSceneName()
    {
        return GetScene(currentScene);
    }

    public void OnLoadingNextSceneCompleted()
    {
        currentScene = nextScene;
        beaconsPosInLoadedLevel = persistentData.GetLevelBeaconsPos(GetScene(currentScene)) != null? new(persistentData.GetLevelBeaconsPos(GetScene(currentScene))) : null;
        GameplayController.Get().SpawnMapBeacons();
    }
}
