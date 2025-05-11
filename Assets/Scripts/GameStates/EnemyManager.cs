using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    //public override int Priority => 30;

    [HideInInspector] public List<GameObject> activeEnemies = new List<GameObject>();
    private Queue<GameObject> toUnregister = new Queue<GameObject>();  // Queue for dead enemies

    private float cleanupInterval = 1f;
    private float cleanupTimer = 0f;
    private bool allWavesComplete = false;
    private bool stageCompleteTriggered = false;

    public void Initialize()
    {
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("PortalCleanup", ResetEnemyManager);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("PortalCleanup", ResetEnemyManager);
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
    
    public void SetAllWavesComplete()
    {
        Debug.Log("All waves complete");
        if (allWavesComplete) return;
        allWavesComplete = true;
        CheckForStageCompletion();
    }
    
    private void CheckForStageCompletion()
    {
        Debug.Log("Checking if Stage complete!");
        if (!stageCompleteTriggered && allWavesComplete && activeEnemies.Count == 0)
        {
            stageCompleteTriggered = true;
            Debug.Log("All enemies defeated! Stage complete!");
            EventManager.Instance.TriggerEvent("StageCompleted");
        }
    }
    
    public void DisableAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && !enemy.CompareTag("Boss"))
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                enemyAI.DisableEnemy();
            }
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

                //enemyHealth.DeathOnStageEnd(transform);
            }
            else
            {
                yield return null;
            }
        }
    }
    
    public void ResetEnemyManager()
    {
        ClearEnemies();
        allWavesComplete = false;
        stageCompleteTriggered = false;
    }
    
    public void ClearEnemies()
    {
        Debug.Log("Clearing enemies");
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy); // Destroy each active enemy
            }
        }
        activeEnemies.Clear();
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
        if (StageManager.Instance.stageIsActive && stageCompleteTriggered) return;

        cleanupTimer += Time.deltaTime;
        //Debug.Log("All waves complete: Checking for stage completion");
        if (cleanupTimer >= cleanupInterval)
        {
            ClearInactiveEnemies();
            cleanupTimer = 0f; // Reset timer
            if (allWavesComplete)
            {
                Debug.Log("All waves complete: Checking for stage completion");
                CheckForStageCompletion();
            }
        }
    }
}
