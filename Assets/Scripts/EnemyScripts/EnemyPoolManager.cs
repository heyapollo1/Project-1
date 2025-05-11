using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : BaseManager
{
    public static EnemyPoolManager Instance;
    public override int Priority => 30;
    
    [System.Serializable]
    public class Pool
    {
        public GameObject enemyPrefab;
        public string enemyType;
        public int poolSize = 10;
        public int spawnWeight = 1;
    }

    private Dictionary<string, int> enemyPoolSizes = new Dictionary<string, int>();
    private Dictionary<string, Queue<GameObject>> enemyPools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, int> enemyWeights = new Dictionary<string, int>();

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("PortalCleanup", HandlePortalTransition);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("PortalCleanup", HandlePortalTransition);
        CleanupPools();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnSceneUnloaded(string sceneName)
    {
        if (sceneName == "GameScene")
        {
            CleanupPools();
        }
    }

    public void PrepareEnemyPool(StageSettings currentStage) //Setup for next stage
    {
        Debug.Log("Prep pool");
        foreach (WaveSettings wave in currentStage.wavesInStage)
        {
            foreach (var enemyRequirement in wave.enemyPayload)
            {
                string enemyType = enemyRequirement.enemyType;
                const int fixedPoolSize = 20;
                
                if (!enemyPools.ContainsKey(enemyType))
                {
                    Debug.Log($"Creating pool for enemy type: {enemyType}");
                    enemyPools[enemyType] = new Queue<GameObject>();
                    enemyPoolSizes[enemyType] = fixedPoolSize;
                }

                Queue<GameObject> pool = enemyPools[enemyType];

                while (pool.Count < fixedPoolSize)
                {
                    Debug.Log("Set false and queue pooled enemy");
                    GameObject newEnemy = Instantiate(enemyRequirement.enemyPrefab);
                    newEnemy.SetActive(false); // Ensure it's inactive
                    pool.Enqueue(newEnemy);
                }
            }
        }
        Debug.Log("Enemy pools prepared for the entire stage.");
    }
    
    public void InitializeEnemyWeights(WaveSettings wave)
    {
        enemyWeights.Clear();

        foreach (var enemyRequirement in wave.enemyPayload)
        {
            if (!enemyWeights.ContainsKey(enemyRequirement.enemyType))
            {
                enemyWeights[enemyRequirement.enemyType] = enemyRequirement.spawnWeight;
                Debug.Log($"Set weight for {enemyRequirement.enemyType} to {enemyRequirement.spawnWeight}");
            }
        }

        Debug.LogWarning($"Initialized enemy weights. Total types: {enemyWeights.Count}");
        EventManager.Instance.TriggerEvent("UpdateWave", wave);
    }

    public void InitializeBossWeight(WaveSettings wave)
    {
        enemyWeights.Clear();

        foreach (var enemyRequirement in wave.enemyPayload)
        {
            if (!enemyWeights.ContainsKey(enemyRequirement.enemyType))
            {
                enemyWeights[enemyRequirement.enemyType] = enemyRequirement.spawnWeight;
                Debug.Log($"Set weight for {enemyRequirement.enemyType} to {enemyRequirement.spawnWeight}");
            }
        }

        Debug.LogWarning($"Initialized enemy weights. Total types: {enemyWeights.Count}");
        EventManager.Instance.TriggerEvent("UpdateBossWave", wave);
    }

    public void HandlePortalTransition()
    {
        if (enemyPools.Count == 0) return;
        CleanupPools();
    }

    public void CleanupPools()
    {
        foreach (var pool in enemyPools)
        {
            Queue<GameObject> enemies = pool.Value;

            while (enemies.Count > 0)
            {
                GameObject enemy = enemies.Dequeue();
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
        }
        
        enemyPools.Clear();
        enemyPoolSizes.Clear();
        enemyWeights.Clear();
        Debug.Log("Enemy pools cleaned up.");
    }

    public GameObject GetEnemy(string enemyType, Vector3 position, Quaternion rotation)
    {
        if (enemyPools.TryGetValue(enemyType, out Queue<GameObject> pool) && pool.Count > 0)
        {
            GameObject enemy = pool.Dequeue();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;

            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Debug.LogWarning($"ACTIVATING ENEMY");
                enemyAI.ActivateFromPool(position);
            }

            return enemy;
        }

        Debug.LogWarning($"{enemyType} not in pool!");
        return null;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        string enemyType = enemy.name.Replace("(Clone)", "").Trim();  // Get prefab name without "(Clone)"

        if (enemyPools.TryGetValue(enemyType, out Queue<GameObject> pool))
        {
            pool.Enqueue(enemy);
        }
        else
        {
            Debug.LogWarning($"No pool found for enemy type {enemyType}");
        }
    }

    public void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
    }

    public void SpawnWeightedEnemy(Vector3 position)
    {
        Debug.LogWarning($"Spawning Weight Enemy");
        string enemyType = GetWeightedRandomEnemyType();
        if (!string.IsNullOrEmpty(enemyType))
        {
            GameObject newEnemy = GetEnemy(enemyType, position, Quaternion.identity);
            if (newEnemy == null)
            {
                Debug.LogWarning($"No {enemyType} enemies available in pool.");
            }
        }
    }

    private string GetWeightedRandomEnemyType()
    {
        int totalWeight = 0;

        foreach (var weight in enemyWeights.Values)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        foreach (var kvp in enemyWeights)
        {
            cumulativeWeight += kvp.Value;
            if (randomValue < cumulativeWeight)
            {
                return kvp.Key;
            }
        }
        return null;
    }
    
    public Dictionary<string, int> GetPreppedPoolData()
    {
        return new Dictionary<string, int>(enemyPoolSizes);
    }
    
    public void RecreatePoolsFromSave(WorldState worldState)
    {
        Debug.Log($"Stage Level: {worldState.currentStageIndex}. Config: {worldState.currentStageConfigName}");
        StageConfig currentStageConfig = StageDatabase.Instance.GetStageByName(worldState.currentStageConfigName);
        StageSettings currentStage = currentStageConfig.Stages[worldState.currentStageIndex];

        foreach (WaveSettings wave in currentStage.wavesInStage)
        {
            foreach (var enemyRequirement in wave.enemyPayload)
            {
                string enemyType = enemyRequirement.enemyType;
                const int fixedPoolSize = 20;

                if (!enemyPools.ContainsKey(enemyType))
                {
                    Debug.Log($"Creating pool for enemy type: {enemyType}");
                    enemyPools[enemyType] = new Queue<GameObject>();
                    enemyPoolSizes[enemyType] = fixedPoolSize;
                }

                Queue<GameObject> pool = enemyPools[enemyType];

                while (pool.Count < fixedPoolSize)
                {
                    Debug.Log("Set false and queue pooled enemy");
                    GameObject newEnemy = Instantiate(enemyRequirement.enemyPrefab);
                    newEnemy.SetActive(false); 
                    pool.Enqueue(newEnemy);
                }
            }
        }
    }
    
}
