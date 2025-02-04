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

    private Dictionary<string, Queue<GameObject>> enemyPools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, int> enemyWeights = new Dictionary<string, int>();

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("TravelDeparture", HandleEnemyPoolPreparation);
        //EventManager.Instance.StopListening("TravelArrival", OpenShop);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("TravelDeparture", HandleEnemyPoolPreparation);
        //EventManager.Instance.StopListening("TravelArrival", OpenShop);
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

    public void PrepareEnemyPool(StageSettings currentStage)
    {
        foreach (WaveSettings wave in currentStage.wavesInStage)
        {
            // Iterate through each enemy requirement in the wave
            foreach (var enemyRequirement in wave.enemyRequirements)
            {
                string enemyType = enemyRequirement.enemyType;

                // Check if the pool for this enemy type already exists
                if (!enemyPools.ContainsKey(enemyType))
                {
                    Debug.Log($"Creating pool for enemy type: {enemyType}");
                    enemyPools[enemyType] = new Queue<GameObject>();
                }

                const int fixedPoolSize = 20;
                Queue<GameObject> pool = enemyPools[enemyType];

                while (pool.Count < fixedPoolSize)
                {
                    Debug.Log($"Set false and queue pooled enemy");
                    GameObject newEnemy = Instantiate(enemyRequirement.enemyPrefab);
                    Debug.Log($"Set false and queue pooled enemy");
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

        foreach (var enemyRequirement in wave.enemyRequirements)
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

        foreach (var enemyRequirement in wave.enemyRequirements)
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

    public void HandleEnemyPoolPreparation(string destination)
    {
        if (destination != "Game")
        {
            CleanupPools();
        }
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
                    Destroy(enemy); // Destroy all enemy objects
                }
            }
        }
        //EnemyManager.Instance.ClearEnemies();
        enemyPools.Clear();
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
}
