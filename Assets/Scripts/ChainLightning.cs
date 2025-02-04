using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChainLightning : MonoBehaviour
{
    //public float refreshRate = 0.05f;
    public float delayBetweenEachChain = 0.03f;
    public float chainLightningRange = 5f;  // Maximum range for chaining

    private bool isChaining;
    private float chainCount = 1;
    //List<GameObject> spawnedLineRenderers = new List<GameObject>();
    List<GameObject> enemiesInChain = new List<GameObject>();

    public static ChainLightning Instance { get; private set; }

    public bool allowRechain = true;

    public void StartChain(Transform origin, Action<GameObject, Vector3> onHitEnemy, Action<Vector3, Vector3> chainNextEnemy, GameObject lineRendererPrefab, float maxEnemiesInChain)
    {
        isChaining = true;
        chainCount = 1;
        enemiesInChain.Clear();

        // Find the first enemy to zap from the origin
        GameObject firstEnemy = EnemyManager.Instance.GetClosestEnemy(origin.position, null, chainLightningRange);
        // Add debug logs before invoking
        if (firstEnemy == null)
        {
            StopChain();
            return;
        }

        onHitEnemy?.Invoke(firstEnemy, origin.transform.position);

        // Double-check that both origin and firstEnemy are still valid
        if (origin == null || firstEnemy == null || !firstEnemy.activeInHierarchy)
        {
            Debug.Log("Chain failed: Origin or firstEnemy became null or inactive.");
            StopChain();
            return;
        }

        Debug.Log($"Invoking chainNextEnemy from {origin.name} to {firstEnemy.name}...");
        chainNextEnemy?.Invoke(origin.position, firstEnemy.transform.position);
        //NewLineRenderer(origin, firstEnemy.transform, lineRendererPrefab);
        StartCoroutine(ChainReaction(firstEnemy, onHitEnemy, chainNextEnemy, lineRendererPrefab, maxEnemiesInChain));
    }

    private IEnumerator ChainReaction(
     GameObject currentEnemy,
     Action<GameObject, Vector3> onHitEnemy,
     Action<Vector3, Vector3> chainNextEnemy,
     GameObject lineRendererPrefab,
     float maxEnemiesInChain)
    {
        yield return new WaitForSeconds(delayBetweenEachChain);

        // Ensure current enemy is valid before proceeding
        if (currentEnemy == null || !currentEnemy.activeInHierarchy)
        {
            Debug.LogWarning("Chain stopped: Current enemy is null or inactive.");
            StopChain();
            yield break;
        }

        // Ensure chain count hasn't been exceeded
        if (chainCount > maxEnemiesInChain || !isChaining)
        {
            StopChain();
            yield break;
        }

        // Add the current enemy to the chain
        if (!enemiesInChain.Contains(currentEnemy))
        {
            enemiesInChain.Add(currentEnemy);
        }

        chainCount++;

        // Modify the exclusion list based on allowRechain flag
        //List<GameObject> exclusionList = allowRechain ? null : enemiesInChain;

        // Find the next enemy based on the current enemy’s position
        GameObject nextEnemy = EnemyManager.Instance.GetClosestEnemy(
            currentEnemy.transform.position,  // Use current enemy's position as the origin
            allowRechain ? null : enemiesInChain,  // Use exclusion list based on allowRechain flag
            chainLightningRange
        );

        if (nextEnemy == null || !nextEnemy.activeInHierarchy)
        {
            Debug.Log("No valid enemy for further chaining.");
            StopChain();
            yield break;
        }

        // Invoke hit and visual effect on the next enemy
        onHitEnemy?.Invoke(nextEnemy, currentEnemy.transform.position);
        chainNextEnemy?.Invoke(currentEnemy.transform.position, nextEnemy.transform.position);

        // Continue the chain with the next enemy
        StartCoroutine(ChainReaction(nextEnemy, onHitEnemy, chainNextEnemy, lineRendererPrefab, maxEnemiesInChain));
    }

    public void StopChain()
    {
        isChaining = false;
        enemiesInChain.Clear();
    }

    // Unlock function to enable re-chaining to previously chained targets
    public void UnlockRechain()
    {
        allowRechain = true;
        Debug.Log("Rechain unlocked! Enemies can now be chained multiple times.");
    }
}

/*void NewLineRenderer(Transform startPos, Transform endPos, GameObject LineRendererPrefab)
{
    GameObject line = Instantiate(LineRendererPrefab);
    spawnedLineRenderers.Add(line);
    StartCoroutine(UpdateLineRenderer(line, startPos, endPos));
}*/

/*IEnumerator UpdateLineRenderer(GameObject line, Transform startPos, Transform endPos)
{
    var lineController = line.GetComponent<LineRendererController>();

    while (isChaining && line != null)
    {
        lineController.SetPosition(startPos, endPos);
        yield return new WaitForSeconds(refreshRate);
    }
}*/


        /*foreach (var line in spawnedLineRenderers)
        {
            Destroy(line);
        }

        spawnedLineRenderers.Clear();*/

    /*IEnumerator UpdateLineRenderer(GameObject lineR, Transform startPos, Transform endPos, bool fromPlayer = false)
    {
        if(shooting && shot && lineR != null)
        {
            lineR.GetComponent<LineRendererController>().SetPosition(startPos, endPos);

            yield return new WaitForSeconds(refreshRate);

            if (fromPlayer)
            {
                StartCoroutine(UpdateLineRenderer(lineR, startPos, playerEnemyDetector.GetClosestEnemy().transform, true));
                if(currentClosestEnemy != playerEnemyDetector.GetClosestEnemy())
                {
                    StopShooting();
                    StartShooting();
                }
            }
            else
            {
                StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos));
            }
        }

    }*/

    /*IEnumerator ChainReaction (GameObject closestEnemy)
    {
        yield return new WaitForSeconds(delayBetweenEachChain);

        if(counter == maximumEnemiesInChain)
        {
            yield return null;
        }
        else
        {
            if(shooting && closestEnemy != null)
            {
                counter++;

                enemiesInChain.Add(closestEnemy);

                // Use the EnemyManager to find the next closest enemy
                GameObject nextEnemy = EnemyManager.Instance.GetClosestEnemy(closestEnemy.transform.position, enemiesInChain, chainLightningRange);
                // Get the next closest enemy
                //EnemyDetector enemyDetector = closestEnemy.GetComponent<EnemyDetector>();
                //GameObject nextEnemy = enemyDetector?.GetClosestEnemy();

                // Ensure the next enemy is not already in the chain
                if (nextEnemy != null && !enemiesInChain.Contains(nextEnemy))
                {
                    NewLineRenderer(closestEnemy.transform, nextEnemy.transform);
                    StartCoroutine(ChainReaction(nextEnemy));
                }
                else
                {
                    Debug.Log("No more unique enemies in chain.");
                }
                /*
                if (!enemiesInChain.Contains(closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy()))
                {
                    NewLineRenderer(closestEnemy.transform, closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy().transform);
                    StartCoroutine(ChainReaction(closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy()));
                }
                else
                {
                    Debug.Log("No more unique enemies in chain bro");
                }
            }
        }
    }*/
    /*
    void StopShooting()
    {
        shooting = false;
        shot = false;

        for(int i = 0; i < spawnedLineRenderers.Count; i++)
        {
            Destroy(spawnedLineRenderers[i]);
        }

        spawnedLineRenderers.Clear();
        enemiesInChain.Clear();
    }

}*/