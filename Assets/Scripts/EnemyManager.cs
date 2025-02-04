using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : BaseManager
{
    public static EnemyManager Instance { get; private set; }
    public override int Priority => 30;

    [HideInInspector] public List<GameObject> activeEnemies = new List<GameObject>();
    private Queue<GameObject> toUnregister = new Queue<GameObject>();  // Queue for dead enemies

    private float cleanupInterval = 1f;
    private float cleanupTimer = 0f;

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("PortalEntered", EndStage);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("PortalEntered", EndStage);
    }

    private void OnSceneUnloaded(string scene)
    {
        if (scene == "GameScene")
        {
            ClearEnemies();
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            Debug.Log($"Enemy registered: {enemy?.name}");
            activeEnemies.Add(enemy);
        }
    }

    public void BossDeath()
    {
        StartCoroutine(BossDeathCoroutine());
    }

    private IEnumerator BossDeathCoroutine()
    {
        yield return StartCoroutine(KillAllEnemies());

        activeEnemies.Clear();
    }

    public void EndStage()
    {
        StartCoroutine(EndStageCoroutine());
    }

    private IEnumerator EndStageCoroutine()
    {
        yield return StartCoroutine(DisableAllEnemies());

        //GameObject player = PersistentManager.Instance.playerInstance;
        //TransitionManager.Instance.StartTransition(player, destinationName, destinationSpawn);
        //Debug.Log("Portal transition.");
        activeEnemies.Clear();
    }
    
    public IEnumerator DisableAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && !enemy.CompareTag("Boss"))
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                enemyAI.DisableEnemy();
            }
            else
            {
                yield return null;
            }
        }
    }

    public IEnumerator KillAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && !enemy.CompareTag("Boss"))
            {
                EnemyHealthManager enemyHealth = enemy.GetComponent<EnemyHealthManager>();

                if (enemyHealth == null)
                {
                    Debug.Log("no enemy health");
                    continue;
                }

                enemyHealth.DeathOnStageEnd(transform);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void ClearEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy); // Destroy each active enemy
            }
        }

        activeEnemies.Clear();
        Debug.Log("All active enemies have been destroyed and cleared.");
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemy != null && activeEnemies.Contains(enemy))
        {
            toUnregister.Enqueue(enemy);  // Add to queue for later removal
        }
    }

    public void ClearInactiveEnemies()
    {
        while (toUnregister.Count > 0)
        {
            GameObject enemy = toUnregister.Dequeue();
            activeEnemies.Remove(enemy);
        }
    }

    public List<GameObject> GetEnemiesInRange(Vector3 position, float detectionRange, GameObject callingEnemy)
    {
        List<GameObject> enemiesInRange = new List<GameObject>();

        foreach (var enemy in activeEnemies)
        {
            if (enemy == null || !enemy.activeInHierarchy || enemy == callingEnemy) continue;

            float distanceToEnemy = Vector3.Distance(position, enemy.transform.position);
            if (distanceToEnemy <= detectionRange)
            {
                enemiesInRange.Add(enemy);
            }
        }

        return enemiesInRange;
    }

    public bool HasAlliesInRange(Vector3 position, float detectionRange, GameObject callingEnemy)
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null || !enemy.activeInHierarchy || enemy == callingEnemy) continue;

            float distanceToEnemy = Vector3.Distance(position, enemy.transform.position);
            if (distanceToEnemy <= detectionRange)
            {
                return true; // Found at least one ally within range
            }
        }
        return false;
    }

    // Returns the closest active enemy to the given position
    public GameObject GetClosestEnemy(Vector3 fromPosition, List<GameObject> excludedEnemies = null, float maxRange = Mathf.Infinity)
    {
        GameObject closestEnemy = null;
        float closestDistanceSqr = maxRange * maxRange;

        foreach (var enemy in activeEnemies)
        {
            if (enemy == null || !enemy.activeInHierarchy) continue;

            if (excludedEnemies != null && excludedEnemies.Contains(enemy)) continue;

            float distanceSqr = (enemy.transform.position - fromPosition).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing) return;

        cleanupTimer += Time.deltaTime;

        if (cleanupTimer >= cleanupInterval)
        {
            ClearInactiveEnemies();
            cleanupTimer = 0f; // Reset timer
        }
    }
}
