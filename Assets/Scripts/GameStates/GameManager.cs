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
        string[] requiredSystems = { "SpawnManager", "GridManager", "FlowFieldManager", "CameraManager", "CutsceneManager"};

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
            Debug.LogWarning("No save file.");
        }
    }
    
    private void  HandlePlayerRevive()
    {
        GameStateManager.Instance.SetGameState(GameState.Playing);
        EventManager.Instance.TriggerEvent("RevivePlayer");
        EventManager.Instance.TriggerEvent("EnablePlayerAbilities");
        EventManager.Instance.TriggerEvent("HideGameOverUI");
        WeaponManager.Instance.ToggleWeaponVisibility(true);
    }
    
    private void HandlePlayerDeath()
    {
        GameStateManager.Instance.SetGameState(GameState.GameOver);
        EventManager.Instance.TriggerEvent("KillPlayer");
        EventManager.Instance.TriggerEvent("DisablePlayerAbilities");
        EventManager.Instance.TriggerEvent("ShowGameOverUI");
        WeaponManager.Instance.ToggleWeaponVisibility(false);
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
        Debug.LogError("GameManager dependencies and ready systems have been reset.");
    }

    public void LoadWorldItemState(WorldState state) // loading in list
    {
        Debug.Log($"WORLD ITEM LOAD. Count: {state.worldItemData.Count}");
        foreach (var itemData in state.worldItemData)
        {
            string[] parts = itemData.Split(':');
            
            if (parts.Length != 4)
            {
                Debug.LogWarning($"Invalid world item data format: {itemData}");
                continue;
            }
            
            string type = parts[0];
            string uniqueID = parts[1];
            string name = parts[2];
            Vector3 position = StringToVector3(parts[3]);
            
            if (type == "ITEM")
            {
                Debug.Log($"WORLD ITEM LOAD. Count: {state.worldItemData.Count}");
                BaseItem baseItem = ItemDatabase.CreateItem(name);
                baseItem.uniqueID = uniqueID;

                var savedItemPayload = new ItemPayload()
                {
                    weaponScript = null,
                    itemScript = baseItem,
                    isWeapon = false
                };
                
                GameObject dropPrefab = Resources.Load<GameObject>("Prefabs/ItemPrefab");
                GameObject dropObj = Instantiate(dropPrefab, position, Quaternion.identity);
                var dropScript = dropObj.GetComponent<ItemDrop>();

                dropScript.InitializeDroppedItem(savedItemPayload);
                ItemTracker.Instance.AssignItemToTracker(savedItemPayload, ItemLocation.World, -1, -1, dropObj);

                Debug.Log($"WORLD ITEM LOAD - Item: {name} at {position}");
            }
            else if (type == "WEAPON")
            {
                WeaponInstance weaponInstance = WeaponDatabase.Instance.CreateWeaponInstance(name);
                weaponInstance.uniqueID = uniqueID;

                var savedWeaponPayload = new ItemPayload()
                {
                    weaponScript = weaponInstance,
                    itemScript = null,
                    isWeapon = true
                };

                GameObject dropPrefab = Resources.Load<GameObject>("Prefabs/ItemPrefab");
                GameObject dropObj = Instantiate(dropPrefab, position, Quaternion.identity);
                var dropScript = dropObj.GetComponent<ItemDrop>();

                dropScript.InitializeDroppedItem(savedWeaponPayload);
                ItemTracker.Instance.AssignItemToTracker(savedWeaponPayload, ItemLocation.World, -1, -1, dropObj);

                Debug.Log($"WORLD ITEM LOAD - Weapon: {name} at {position}");
            }
            else
            {
                Debug.LogWarning($"Unknown item type '{type}' in world item data.");
            }
        }
    }
    
    public static Vector3 StringToVector3(string s)
    {
        string[] parts = s.Split(',');
        if (parts.Length != 3) return Vector3.zero;

        if (float.TryParse(parts[0], out float x) &&
            float.TryParse(parts[1], out float y) &&
            float.TryParse(parts[2], out float z))
        {
            return new Vector3(x, y, z);
        }

        Debug.LogWarning($"Invalid Vector3 string: {s}");
        return Vector3.zero;
    }
}