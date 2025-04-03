using System;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum EncounterType
{
    None,
    Shop,
    Treasure,
    Victory
}

// Main GameState class to replace GameData
[Serializable]
public class GameData
{
    public int version = 1;
    public bool isNewGame = true;
    // Player-related data
    public PlayerState playerState = new PlayerState();
    // World-related data
    public WorldState worldState = new WorldState();
    // Settings-related data
    public SettingsState settingsState = new SettingsState();
    //Shop-related data
    public CameraState cameraState = new CameraState();
}

// Player State
[Serializable]
public class PlayerState
{
    public Vector3 position;
    public float health;
    public int xpAmount;
    public int xpTotal;
    public int level;
    public int resources;

    public Dictionary<StatType, float> flatBonuses = new();
    public Dictionary<StatType, float> percentBonuses = new();

    public int inventorySlots;
    public List<string> inventoryWithRarity = new();
    public List<string> upgrades = new();

    public List<string> acquiredAbilities = new();
    
    public int currentLoadoutID;
    //public int currentLoadoutIndex;
    public List<string> acquiredWeapons = new();
    public List<WeaponLoadoutData> savedWeaponLoadouts = new List<WeaponLoadoutData>();
    //public string equippedWeapon;
}

// World State
[Serializable]
public class WorldState
{
    public string currentStageConfigName;
    public int currentStageIndex;
    public bool isMapActive;
    public bool isStageActive;
    public EncounterType currentEncounter;
    public EncounterData encounterData;
    public List<string> activeItemNames;
    public List<string> pedestalItems = new List<string>();
    //public Dictionary<string, int> enemyPools = new Dictionary<string, int>();
}

[Serializable]
public class CameraState
{
    public Vector3 playerCameraPosition;
    public Vector3 cutsceneCameraPosition;
    public float playerCameraOrthographicSize;
    public float cutsceneCameraOrthographicSize;
}

// Settings State
[Serializable]
public class SettingsState
{
    public float audioVolume = 1.0f;
    //public string graphicsQuality = "High";
}
// File: Assets/Scripts/SaveSystem/SaveManager.cs