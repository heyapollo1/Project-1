using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : BaseManager
{
    public static CustomSceneManager Instance { get; private set; }

    public override int Priority => 10;
    private string currentScene;

    protected override void OnInitialize()
    {
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadScene(string sceneName, GameData selectedSave = null, bool isNewGame = true)
    {
        Debug.Log($"Loading scene: {sceneName} with {(selectedSave != null ? "save data" : "no save data")}");

        // Only pass the selected save data if it's a gameplay scene
        if (sceneName == "GameScene" && selectedSave == null && !isNewGame)
        {
            Debug.Log("Attempted to load GameScene without save data. Creating new game data.");
            selectedSave = new GameData();
        }
        StartCoroutine(LoadSceneWithTransition(sceneName, selectedSave, isNewGame));
    }

    private IEnumerator LoadSceneWithTransition(string targetScene, GameData selectedSave, bool isNewGame)
    {
        LoadingMenu.Instance.ShowLoadingScreen();
        if (LoadingMenu.Instance != null)
        {
            LoadingMenu.Instance.UpdateProgress(0);
        }
        EventManager.Instance.TriggerEvent("SceneUnloaded", currentScene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;
        
        while (asyncLoad.progress < 0.9f)
        {
            if (LoadingMenu.Instance != null)
            {
                LoadingMenu.Instance.UpdateProgress(asyncLoad.progress);
            }
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncLoad.isDone);
        yield return new WaitForEndOfFrame();
        currentScene = targetScene;

        if (targetScene == "GameScene")
        {
            EventManager.Instance.TriggerEvent("LoadPlayerFromSave", selectedSave, isNewGame);
            if (selectedSave != null && !isNewGame)
            {
                SaveManager.ApplySaveData(selectedSave);
                SaveManager.SaveGame(selectedSave);
                Debug.Log("Save data applied after player spawn.");
            }
            EventManager.Instance.TriggerEvent("SceneLoaded", targetScene);
            yield return GameManager.Instance.WaitForDependenciesAndStartGame();
        }
        else
        {
            EventManager.Instance.TriggerEvent("SceneLoaded", targetScene);
        }
        //Debug.LogError("Waiting for dpendencies.");
        yield return new WaitUntil(() => asyncLoad.isDone);
        if (SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Loaded scene: {targetScene}");
            LoadingMenu.Instance.HideLoadingScreen();
        }
    }
    //public string CurrentScene => currentScene;
    
    /*private IEnumerator LoadSceneWithTransition(string targetScene)
    {
        if (!string.IsNullOrEmpty(currentScene))
        {
            Debug.Log("Starting to unload current scene");
            yield return SceneManager.UnloadSceneAsync(currentScene);
            Debug.Log("Finished unloading current scene");
        }
        EventManager.Instance.TriggerEvent("SceneUnloaded", currentScene);

        if (!SceneManager.GetSceneByName("LoadingScene").isLoaded)
        {
            Debug.Log("Starting to load loading scene");
            yield return SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
            Debug.Log("Finished loading loading scene");
        }

        EventManager.Instance.TriggerEvent("ShowLoadingScreen");
        Debug.Log("ShowLoadingScreen triggered");

        Debug.Log("Starting to load target scene");
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        Debug.Log("Finished loading target scene");

        Debug.Log("Starting to unload loading scene");
        if (SceneManager.GetSceneByName("LoadingScene").isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync("LoadingScene");
            Debug.Log("Finished unloading loading scene");
        }

        currentScene = targetScene;
        Debug.Log($"SceneLoaded triggered for {targetScene}");
        EventManager.Instance.TriggerEvent("SceneLoaded", targetScene);
    }*/
}
