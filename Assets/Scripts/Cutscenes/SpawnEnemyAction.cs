using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;


[CreateAssetMenu(menuName = "Cutscene/Actions/Spawn Enemy")]
public class SpawnEnemyAction : CutsceneAction
{
    public GameObject enemyPrefab;  // Assign the enemy prefab in the inspector
    public Vector3 spawnPosition;   // Set spawn location in the inspector

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("SpawnEnemyAction: No enemy prefab assigned!");
            yield break;
        }

        // Instantiate the enemy at the desired position
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.Initialize();
            Debug.Log($"Enemy spawned at {spawnPosition} and initialized.");
        }
        else
        {
            Debug.LogWarning("Spawned object does not have an EnemyAI component!");
        }

        // ✅ Allow cutscene to continue
        yield return new WaitForSeconds(0.5f); // Optional small delay for effect
        onComplete?.Invoke();
    }
}