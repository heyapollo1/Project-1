/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Public method to load a scene
    public void LoadScene(string sceneName, bool showLoadingScreen = false)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, showLoadingScreen));
    }

    // Asynchronous scene loading with optional loading screen
    private IEnumerator LoadSceneRoutine(string sceneName, bool showLoadingScreen)
    {
        if (showLoadingScreen)
        {
            UIManager.Instance.ShowLoadingScreen();
        }

        yield return new WaitForSeconds(0.5f);

        // Load the scene asynchronously
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            // Optionally update a loading bar or UI
            UIManager.Instance.UpdateLoadingProgress(asyncLoad.progress);
            yield return null;
        }

        // Hide loading screen after loading
        if (showLoadingScreen)
        {
            UIManager.Instance.HideLoadingScreen();
        }

        OnSceneLoaded(sceneName);
    }

    private void OnSceneLoaded(string sceneName)
    {
        Debug.Log($"Scene '{sceneName}' loaded successfully.");
        NotifyManagersSceneChanged(sceneName);
    }

    // Notify all relevant managers about the scene change
    private void NotifyManagersSceneChanged(string sceneName)
    {
        var sceneListeners = FindObjectsOfType<MonoBehaviour>().OfType<ISceneListener>();
        foreach (var listener in sceneListeners)
        {
            listener.OnSceneChange(sceneName);
        }
    }
}*/
