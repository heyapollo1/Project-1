
using System.IO;
using UnityEngine;

public static class SaveManager {
    
    private static string savePath = $"{Application.persistentDataPath}/gameState.json";
    
    public static void SaveGame(GameData gameData) {
        
        if (gameData == null)
        {
            Debug.LogError("Cannot save game: GameData is null.");
            return;
        }

        gameData.isNewGame = false;
        var player = PlayerManager.Instance.playerInstance;
        var attributes = AttributeManager.Instance;
        
        // Player Position
        if (player != null)
        {
            gameData.playerState.position = player.transform.position;
            Debug.Log($"Player correctly positioned at saved position: {gameData.playerState.position}");
        }
        else
        {
            Debug.LogError($"playerPosition null because new game, no player spawned yet.");
        }
        
        //Player State
        gameData.playerState.xpAmount = XPManager.Instance.GetCurrentXP();
        gameData.playerState.xpTotal = XPManager.Instance.GetXPToNextLevel();
        gameData.playerState.level = XPManager.Instance.currentLevel;
        gameData.playerState.health = PlayerHealthManager.Instance.currentHealth;
        gameData.playerState.resources = ResourceManager.Instance.currentCurrency;
        gameData.playerState.inventorySlots = InventoryManager.Instance.inventorySlots;
        gameData.playerState.inventoryWithRarity = InventoryManager.Instance.GetInventoryNamesWithRarity();
        gameData.playerState.acquiredWeapons = WeaponManager.Instance.GetWeaponNames();
        gameData.playerState.currentLoadoutID = WeaponManager.Instance.GetCurrentLoadoutID();
        //gameData.playerState.equippedLeftWeapon = WeaponManager.Instance.leftWeapon?.weaponData?.weaponTitle ?? "";
        //gameData.playerState.equippedRightWeapon = WeaponManager.Instance.rightWeapon?.weaponData?.weaponTitle ?? "";
        gameData.playerState.savedWeaponLoadouts.Clear();
        foreach (var loadout in WeaponManager.Instance.GetPlayerLoadouts())
        {
            WeaponLoadoutData loadoutData = new WeaponLoadoutData(loadout);
            gameData.playerState.savedWeaponLoadouts.Add(loadoutData);
            Debug.Log($"Saving Loadout: Left-{loadoutData.leftWeapon} | Right-{loadoutData.rightWeapon}");
        }
        //gameData.playerState.equippedWeapon = WeaponManager.Instance.GetEquippedWeaponName();
        gameData.playerState.acquiredAbilities = PlayerAbilityManager.Instance.GetAbilityNames();
        gameData.playerState.upgrades = UpgradeManager.Instance.GetUpgradeNames();
        //gameData.playerState.flatBonuses = attributes.GetFlatBonuses();
        //gameData.playerState.percentBonuses = attributes.GetPercentBonuses();
        
        //World State
        gameData.worldState.currentStageConfigName = StageManager.Instance.GetCurrentStageConfigName();
        gameData.worldState.currentStageIndex = StageManager.Instance.GetCurrentStageIndex();
        gameData.worldState.isMapActive = MapUI.Instance.IsMapActive();
        gameData.worldState.isStageActive = StageManager.Instance.IsStageActive();
        gameData.worldState.activeItemNames = ItemDatabase.Instance.GetActiveItemNames();
        gameData.worldState.encounterData = EncounterManager.Instance.GetCurrentEncounterData();
        gameData.worldState.currentEncounter = EncounterManager.Instance != null ? EncounterManager.Instance.GetCurrentEncounterType() : EncounterType.None;
        
        //Shop State
        if (gameData.worldState.currentEncounter == EncounterType.Shop)
        {
            gameData.worldState.pedestalItems.Clear();
            gameData.worldState.pedestalItems = ShopManager.Instance.GetPedestalItemNamesAndRarity();
        }
        
        // Save Camera Positions
        gameData.cameraState.playerCameraPosition = CameraManager.Instance.GetPlayerCamera().transform.position;
        gameData.cameraState.cutsceneCameraPosition = CameraManager.Instance.GetCutsceneCamera().transform.position;
        gameData.cameraState.playerCameraOrthographicSize = CameraManager.Instance.GetPlayerCamera().Lens.OrthographicSize;
        gameData.cameraState.cutsceneCameraOrthographicSize = CameraManager.Instance.GetCutsceneCamera().Lens.OrthographicSize;

        try
        {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Game Saved to: " + savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
        
        try {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Game Saved to: " + savePath);
        } catch (System.Exception e) {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public static GameData LoadGame() { // get game data
        
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found. Creating a new game state.");
            return new GameData();
        }
        
        try
        {
            string json = File.ReadAllText(savePath);
            var loadedData = JsonUtility.FromJson<GameData>(json);
            if (loadedData.worldState == null)
            {
                loadedData.worldState = new WorldState();
                Debug.LogWarning("WorldState missing from save file; initialized with defaults.");
            }
            
            return loadedData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load game: {ex.Message}");
            return new GameData();
        }
    }
    
    public static void ApplySaveData(GameData gameData)
    {
        if (gameData == null)
        {
            Debug.LogError("Cannot apply save data: gameData is null.");
            return;
        }
        TutorialManager.Instance.SetTutorialComplete(); //finish tutorial
        AttributeManager.Instance.ResetAllStats(); // Prepare stats to be modified
        PlayerManager.Instance.ApplyPlayerState(gameData.playerState); //apply base stats
        CameraManager.Instance.LoadCamerasFromSave(gameData.cameraState); //apply saved camera positions
        StageManager.Instance.SetStageProgress(gameData.worldState); //apply stage progression
        
        InventoryManager.Instance.LoadInventoryFromSave(gameData.playerState); //load items but dont apply effects yet
        WeaponManager.Instance.LoadWeaponsFromSave(gameData.playerState); //load weapons but dont apply effects yet
        PlayerAbilityManager.Instance.LoadAbilitiesFromSave(gameData.playerState); //load abilities but dont apply effects yet
        UpgradeManager.Instance.LoadUpgradesFromSave(gameData.playerState); //Load upgrades but dont apply effects yet
        
        InventoryManager.Instance.ApplyItemBonuses(); //now base stats are ready, APPLY MODIFIERS
        UpgradeManager.Instance.ApplyAllUpgrades(); //SAME
        
        ItemDatabase.Instance.LoadActiveItemState(gameData.worldState); //set active item states
        MapUI.Instance.ApplyMapUIState(gameData.worldState); //set map state
        EncounterManager.Instance.LoadEncounterFromSave(gameData.worldState); //load encounter data
        
        UIManager.Instance.ApplyUIModifications(); //Handle and initalize UI inputs(visual aspect)
    }
    
    public static bool CheckSaveExists() {
        return File.Exists(savePath);
    }

    public static void DeleteSave() {
        if (File.Exists(savePath)) {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        } else {
            Debug.LogWarning("No save file to delete.");
        }
    }
}
