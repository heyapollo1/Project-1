using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public Vector2 arenaMinBounds = new (-12f, -12f); // Bottom-left corner of the arena
    public Vector2 arenaMaxBounds = new (12f, 12f);   // Top-right corner of the arena
    public LayerMask obstacleLayer;
    public Transform bossSpawnLocation;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyPoolManager poolManager;

    [SerializeField] public float spawnRadius = 10.0f;
    [SerializeField] public float spawnBufferDistance = 5.0f;

    public float waveDuration = 0.4f;
    private float spawnInterval;
    private int enemiesPerSpawn;
    private bool stageActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        GameManager.Instance.RegisterDependency("SpawnManager", this);

        EventManager.Instance.StartListening("UpdateWave", UpdateWaveSettings);
        EventManager.Instance.StartListening("UpdateBossWave", UpdateBossWaveSettings);

        poolManager = EnemyPoolManager.Instance;
        player = PlayerManager.Instance.playerInstance?.transform;

        GameManager.Instance.MarkSystemReady("SpawnManager");

        Debug.Log("SpawnManager initialized.");
    }

    public void OnDestroy()
    {
        EventManager.Instance.StopListening("UpdateWave", UpdateWaveSettings);
        EventManager.Instance.StartListening("UpdateBossWave", UpdateBossWaveSettings);
    }
    
    public void UpdateWaveSettings(WaveSettings wave)
    {
        Debug.LogWarning($"updated spawn Enemies..");
        spawnInterval = wave.spawnInterval;
        enemiesPerSpawn = wave.enemiesPerSpawn;
        
        StartCoroutine(WaveDelay(0.2f));
    }

    private IEnumerator WaveDelay(float delay)
    {
        if (!stageActive)
        {
            Debug.LogWarning($"stagetimer");
            stageActive = true;
            EventManager.Instance.TriggerEvent("StartStageTimer");
        }
        
        yield return new WaitForSeconds(delay);

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        Debug.LogWarning("Spawning Enemies...");
        float elapsedTime = 0f;

        while (elapsedTime < waveDuration)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                Vector2 spawnPosition = GetValidSpawnPosition();
                if (spawnPosition != Vector2.zero)
                {
                    poolManager.SpawnWeightedEnemy(spawnPosition);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;
        }
        
        EventManager.Instance.TriggerEvent("WaveCompleted");
    }

    private void UpdateBossWaveSettings(WaveSettings wave)
    {
        spawnInterval = wave.spawnInterval;
        enemiesPerSpawn = wave.enemiesPerSpawn;
        Debug.LogWarning($"updated boss settings..");

        StartCoroutine(SpawnBoss(2f));
    }

    private IEnumerator SpawnBoss(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.LogWarning("Spawning Boss...");
        poolManager.SpawnWeightedEnemy(bossSpawnLocation.position);

        //EventManager.Instance.TriggerEvent("BossSpawned");
    }

    public void ResetSpawnManager()
    {
        stageActive = false;
    }

    public Vector3 GetValidSpawnPosition()
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomSpawnOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 randomSpawnPosition = (Vector2)player.position + randomSpawnOffset;

            if (Vector2.Distance(randomSpawnPosition, player.position) > spawnBufferDistance &&
            IsWithinArenaBounds(randomSpawnPosition) && IsPositionValid(randomSpawnPosition))
            {
                return randomSpawnPosition;
            }
        }
        return Vector2.zero;
    }

    bool IsWithinArenaBounds(Vector2 position)
    {
        return position.x >= arenaMinBounds.x && position.x <= arenaMaxBounds.x &&
           position.y >= arenaMinBounds.y && position.y <= arenaMaxBounds.y;
    }

    bool IsPositionValid(Vector2 position)
    {
        if (Physics.CheckSphere(position, 0.5f, obstacleLayer))
        {
            Debug.LogWarning($"Notfound");
            return false;
        }
        Debug.LogWarning($"Valid Position Found, Spawning!");
        return true;
    }

    void OnDrawGizmos()
    {
        Color gizmoColor = Color.green;

        Gizmos.color = gizmoColor;

        // Calculate the four corners of the arena based on min and max bounds
        Vector3 bottomLeft = new Vector3(arenaMinBounds.x, arenaMinBounds.y, 0);
        Vector3 bottomRight = new Vector3(arenaMaxBounds.x, arenaMinBounds.y, 0);
        Vector3 topLeft = new Vector3(arenaMinBounds.x, arenaMaxBounds.y, 0);
        Vector3 topRight = new Vector3(arenaMaxBounds.x, arenaMaxBounds.y, 0);

        // Draw the lines to form a rectangle
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}