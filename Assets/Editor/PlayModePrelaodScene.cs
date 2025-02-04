#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class PlayModePreloadScene
{
    private const string PreloadScenePath = "Assets/Scenes/PreloadScene.unity"; // Replace with your Preload scene name
    private static string previousScenePath;

    static PlayModePreloadScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Store the current scene path before entering Play Mode
            previousScenePath = EditorSceneManager.GetActiveScene().path;
            if (previousScenePath != PreloadScenePath)
            {
                Debug.Log($"Remembering current scene: {previousScenePath}");
            }

            // Switch to Preload scene before entering Play Mode
            if (EditorSceneManager.GetActiveScene().path != PreloadScenePath)
            {
                Debug.Log($"Switching to Preload scene: {PreloadScenePath} before Play Mode.");
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(PreloadScenePath);
                }
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Return to the previous scene after exiting Play Mode
            if (!string.IsNullOrEmpty(previousScenePath) && previousScenePath != PreloadScenePath)
            {
                Debug.Log($"Returning to previous scene: {previousScenePath} after exiting Play Mode.");
                EditorSceneManager.OpenScene(previousScenePath);
            }
        }
    }
}
#endif