using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : BaseManager
{
    public static GameManager Instance { get; private set; }
    public override int Priority => 30;

    private Dictionary<string, object> dependencies = new Dictionary<string, object>();
    private HashSet<string> readySystems = new HashSet<string>();

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("LoadMainMenu", LoadMainMenu);
        EventManager.Instance.StartListening("LoadGameplay", LoadGameplay);
        EventManager.Instance.StartListening("OpenPauseMenu", PauseGame);
        //EventManager.Instance.StartListening("ClosePauseMenuAndResumeGame", UnpauseGame);
        EventManager.Instance.StartListening("CloseGameOverMenu", HideGameOverMenu);
        EventManager.Instance.StartListening("CloseVictoryMenu", HideVictoryMenu);
        EventManager.Instance.StartListening("QuitGame", ExitGame);
        EventManager.Instance.StartListening("PlayerDied", HandlePlayerDeath);
        //EventManager.Instance.StartListening("CutsceneFinished", HandleVictory);
        EventManager.Instance.StartListening("LevelUp", HandleLevelUp);
        EventManager.Instance.StartListening("EndLevelUp", HideLevelUp);
        EventManager.Instance.StartListening("StageComplete", HandleStageEnd);
        EventManager.Instance.StartListening("TravelDeparture", HandleMapTransition);

    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("LoadMainMenu", LoadMainMenu);
        EventManager.Instance.StopListening("LoadGameplay", LoadGameplay);
        EventManager.Instance.StopListening("OpenPauseMenu", PauseGame);
        //EventManager.Instance.StopListening("ClosePauseMenuAndResumeGame", UnpauseGame);
        EventManager.Instance.StopListening("CloseGameOverMenu", HideGameOverMenu);
        EventManager.Instance.StopListening("CloseVictoryMenu", HideVictoryMenu);
        EventManager.Instance.StopListening("QuitGame", ExitGame);
        EventManager.Instance.StopListening("PlayerDied", HandlePlayerDeath);
        //EventManager.Instance.StopListening("CutsceneFinished", HandleVictory);
        EventManager.Instance.StopListening("LevelUp", HandleLevelUp);
        EventManager.Instance.StopListening("EndLevelUp", HideLevelUp);
        EventManager.Instance.StopListening("StageComplete", HandleStageEnd);
        EventManager.Instance.StopListening("TravelDeparture", HandleMapTransition);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterDependency(string key, object dependency)
    {
        if (!dependencies.ContainsKey(key))
        {
            dependencies[key] = dependency;
            Debug.Log($"Registered scene dependency - {key}");
        }
    }

    public T GetDependency<T>(string key) where T : class
    {
        if (dependencies.TryGetValue(key, out var dependency))
        {
            return dependency as T;
        }

        Debug.LogError($"Dependency {key} not found!");
        return null;
    }

    public void MarkSystemReady(string systemName)
    {
        readySystems.Add(systemName);
        Debug.Log($"{systemName} is ready.");
    }

    public bool AreAllSystemsReady(params string[] requiredSystems)
    {
        foreach (var system in requiredSystems)
        {
            if (!readySystems.Contains(system))
                return false;
        }
        return true;
    }

    public IEnumerator WaitForDependenciesAndStartGame()
    {

        string[] requiredSystems = { "SpawnManager", "StageUI", "GridManager", "FlowFieldManager", "AbilityUI", "CameraManager", "CutsceneManager"};

        Debug.Log("Waiting for all dependencies...");
        while (!AreAllSystemsReady(requiredSystems))
        {
            yield return null;
        }

        Debug.Log("All dependencies are ready. Starting game.");
        GameStateManager.Instance.SetGameState(GameState.Playing);
        EventManager.Instance.TriggerEvent("GameStarted");
    }

    private void Start()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        CustomSceneManager.Instance.LoadScene("MainMenu");
        GameStateManager.Instance.SetGameState(GameState.MainMenu);
        Debug.Log("MainMenu loaded, Time.timeScale = " + Time.timeScale);
    }

    public void LoadGameplay()
    {
        Debug.Log("Gameplay load trigger");
        CustomSceneManager.Instance.LoadScene("GameScene");
        //(WaitForDependenciesAndStartGame());
        Debug.Log("Gameplay loaded, Time.timeScale = " + Time.timeScale);
    }

    public void PauseGame()
    {
        Debug.Log("Pause triggered2");
        EventManager.Instance.TriggerEvent("ShowPauseUI");
        GameStateManager.Instance.SetGameState(GameState.Paused);
    }

    public void UnpauseGame()
    {
        EventManager.Instance.TriggerEvent("HidePauseUI");
        GameStateManager.Instance.SetGameState(GameState.Playing);
    }

    public void HidePauseMenu()
    {
        EventManager.Instance.TriggerEvent("HidePauseUI");
    }

    public void HideGameOverMenu()
    {
        EventManager.Instance.TriggerEvent("HideGameOverUI");
    }

    public void HideVictoryMenu()
    {
        EventManager.Instance.TriggerEvent("Victory");
    }

    public void HandleStageEnd()
    {
        EventManager.Instance.TriggerEvent("ResetStageUI");
        EventManager.Instance.TriggerEvent("SpawnPortal", "Shop");
    }

    public void ExitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private void HandlePlayerDeath()
    {
        GameStateManager.Instance.SetGameState(GameState.GameOver);
        EventManager.Instance.TriggerEvent("HandlePlayerDeath");
        EventManager.Instance.TriggerEvent("DisablePlayerAbilities");
        EventManager.Instance.TriggerEvent("ShowGameOverUI");
    }

    private void HandleVictory(string cutsceneName)
    {
        Debug.Log("Victory!");
        if (cutsceneName == "BossDefeatCutscene")
        {
            Debug.Log($"triggering {cutsceneName} cutscene");

            EventManager.Instance.TriggerEvent("SpawnPortal", "Victory");
        }

    }

    private void HandleLevelUp(float currentLevel)
    {
        GameStateManager.Instance.SetGameState(GameState.LevelUp);
        EventManager.Instance.TriggerEvent("UpdateLevelUI", currentLevel);
        if (currentLevel == 2 || currentLevel == 5)
        {
            EventManager.Instance.TriggerEvent("ShowAbilityChoices", 3);
        }
        else
        {
            EventManager.Instance.TriggerEvent("ShowUpgradeChoices", 3);
        }
    }

    private void HideLevelUp()
    {
        GameStateManager.Instance.SetGameState(GameState.Playing);
        EventManager.Instance.TriggerEvent("HideLevelUpUI");
    }

    private void HandleMapTransition(string destination)
    {
        if (destination == "Shop")
        {
            AudioManager.Instance.PlayBackgroundMusic("Music_Shopping", 1.0f);
        }

        if (destination == "Game")
        {
            AudioManager.Instance.PlayBackgroundMusic("Music_Gameplay", 1.0f);
        }

        if (destination == "Victory")
        {
            Debug.Log("Play victory music.");
            AudioManager.Instance.PlayBackgroundMusic("Music_Victory", 1.0f);
        }
    }

    public void ResetDependencies()
    {
        dependencies.Clear();
        readySystems.Clear();
        Debug.Log("GameManager dependencies and ready systems have been reset.");
    }
}