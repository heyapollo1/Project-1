using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageBracket
{
    A, B, C, D, E, F
}

[CreateAssetMenu(fileName = "StageConfig", menuName = "Game/StageConfig", order = 1)]
public class StageConfig : StageData
{
    public List<StageSettings> Stages;

    public StageBracket GetStageBracket(int currentStageNumber)
    {
        if (currentStageNumber <= 1) return StageBracket.A;
        if (currentStageNumber <= 2) return StageBracket.B;
        if (currentStageNumber <= 3) return StageBracket.C;
        if (currentStageNumber <= 4) return StageBracket.D;
        if (currentStageNumber <= 5) return StageBracket.E;
        return StageBracket.F;
    }
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
    public List<EnemyPayload> enemyPayload; // Preload and weight requirements
}

[System.Serializable]
public class EnemyPayload
{
    public GameObject enemyPrefab;          // Prefab for the enemy
    public string enemyType;                // Identifier for the enemy type
    public int spawnWeight = 1;             // Weight for random spawning
    public void AutoAssignEnemyType()
    {
        if (enemyPrefab != null)
        {
            enemyType = enemyPrefab.name;
        }
    }
}
