using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    public List<StageSettings> Stages;
}

[System.Serializable]
public class StageSettings
{
    public int stageNumber;
    public bool isBossStage = false;
    public List<WaveSettings> wavesInStage;
}

[System.Serializable]
public class WaveSettings
{
    public int waveNumber;
    public float spawnInterval = 1.0f;      // Time between spawns
    public int enemiesPerSpawn = 1;         // How many enemies spawn at once
    public List<EnemyTypeRequirement> enemyRequirements; // Preload and weight requirements
}

[System.Serializable]
public class EnemyTypeRequirement
{
    public GameObject enemyPrefab;          // Prefab for the enemy
    public string enemyType;                // Identifier for the enemy type
    public int spawnWeight = 1;             // Weight for random spawning
    //public int poolSize = 10;
}