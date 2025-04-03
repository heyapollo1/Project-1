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
        EventManager.Instance.StartListening("StartNewGame", StartNewGame);
        EventManager.Instance.StartListening("LoadSaveFile", LoadSaveFile);
        
        EventManager.Instance.StartListening("PlayerDied", HandlePlayerDeath);
        EventManager.Instance.StartListening("PlayerRevived", HandlePlayerRevive);
        
        EventManager.Instance.StartListening("LevelUp", HandleLevelUp);
        EventManager.Instance.StartListening("EndLevelUp", HideLevelUp);
        
        EventManager.Instance.StartListening("GameVictory", ClaimVictory);
        EventManager.Instance.StartListening("LoseGame", ClaimLoss);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("StartNewGame", StartNewGame);
        EventManager.Instance.StopListening("LoadSaveFile", LoadSaveFile);
        
        EventManager.Instance.StopListening("PlayerDied", HandlePlayerDeath);
        EventManager.Instance.StopListening("PlayerRevived", HandlePlayerRevive);
        
        EventManager.Instance.StopListening("LevelUp", HandleLevelUp);
        EventManager.Instance.StopListening("EndLevelUp", HideLevelUp);
        
        EventManager.Instance.StopListening("GameVictory", ClaimVictory);
        EventManager.Instance.StopListening("LoseGame", ClaimLoss);
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
        string[] requiredSystems = { "SpawnManager", "GridManager", "FlowFieldManager", "AbilityUI", "CameraManager", "CutsceneManager"};

        Debug.Log("Waiting for all dependencies...");
        while (!AreAllSystemsReady(requiredSystems))
        {
            yield return null;
        }

        Debug.Log("All dependencies are ready. Starting game.");
        GameStateManager.Instance.SetGameState(GameState.Playing);
        //EventManager.Instance.TriggerEvent("GameStarted"); //testing enemy behaviour
    }

    private void Start()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        CustomSceneManager.Instance.LoadScene("MainMenu");
        GameStateManager.Instance.SetGameState(GameState.MainMenu);
    }

    public void StartNewGame()
    {
        Debug.Log("New game trigger");
        GameData newGameData = new GameData { isNewGame = true };
        //SaveManager.SaveGame(newGameData);
        CustomSceneManager.Instance.LoadScene("GameScene", newGameData);
    }
    
    public void LoadSaveFile()
    {
        string savePath = $"{Application.persistentDataPath}/gameState.json";
        
        if (System.IO.File.Exists(savePath))
        {
            GameData loadedSave = SaveManager.LoadGame();
            Debug.Log("Loading GameScene with saved game data.");
            CustomSceneManager.Instance.LoadScene("GameScene", loadedSave, false);
        }
        else
        {
            Debug.LogWarning("No save file, youre fucked.");
        }
    }
    
    private void  HandlePlayerRevive()
    {
        GameStateManager.Instance.SetGameState(GameState.Playing);
        EventManager.Instance.TriggerEvent("RevivePlayer");
        EventManager.Instance.TriggerEvent("EnablePlayerAbilities");
        EventManager.Instance.TriggerEvent("EnablePlayerWeapons");
        EventManager.Instance.TriggerEvent("HideGameOverUI");
    }
    
    private void HandlePlayerDeath()
    {
        GameStateManager.Instance.SetGameState(GameState.GameOver);
        EventManager.Instance.TriggerEvent("KillPlayer");
        EventManager.Instance.TriggerEvent("DisablePlayerAbilities");
        EventManager.Instance.TriggerEvent("DisablePlayerWeapons");
        EventManager.Instance.TriggerEvent("ShowGameOverUI");
    }

    private void HandleLevelUp(float currentLevel)
    {
        GameStateManager.Instance.SetGameState(GameState.LevelUp);
        EventManager.Instance.TriggerEvent("UpdateLevelUI", currentLevel);
        if (currentLevel == 5 || currentLevel == 10)
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
        UIManager.Instance.HideLevelUpUI();
    }

    public void ClaimVictory()
    {
        Debug.Log("Victory claimed.");
        Vector3 portalSpawnLocation = SpawnManager.Instance.GetValidSpawnPosition();
        EventManager.Instance.TriggerEvent("SpawnPortal", "Victory", portalSpawnLocation);
        Debug.LogWarning($"Portal spawned: {portalSpawnLocation}");
    }
    
    public void ClaimLoss()
    {
        Debug.Log("Loss claimed.");
    }
    
    public void ResetDependencies()
    {
        dependencies.Clear();
        readySystems.Clear();
        Debug.Log("GameManager dependencies and ready systems have been reset.");
    }
}