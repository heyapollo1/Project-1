using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    List<GameObject> enemiesInRange = new List<GameObject>();

    private float debugRange = 3.5f;

    public GameObject FindNearestEnemyInRange(float detectionRange)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        if (hitColliders.Length == 0)
        {
            Debug.LogWarning("No enemies found within range.");
            return null;
        }

        float shortestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider2D collider in hitColliders)
        {
            GameObject potentialEnemy = collider.gameObject;

            //Debug.Log($"Found collider: {collider.name}");
            if (potentialEnemy != null && potentialEnemy.activeInHierarchy && collider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, potentialEnemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestEnemy = potentialEnemy;
                }
            }
        }

        if (closestEnemy == null)
        {
        }
        return closestEnemy;
    }

    public bool IsEnemyInRange(float detectionRange, GameObject enemy)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log($"Checking if {enemy.name} is within range. Total detected: {hitColliders.Length}");
            if (collider.gameObject == enemy && enemy.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    public List<GameObject> GetEnemiesInRange()
    {
        // Filter out any destroyed or inactive enemies
        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        return enemiesInRange;
    }

    public int CountEnemiesInRange(GameObject enemy, float range)
    {
        // Filter out any destroyed or inactive enemies
        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        return enemiesInRange.Count;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.gameObject != null && collision.gameObject.activeInHierarchy)
        {
            if (enemiesInRange.Count == 0)
            {
                enemiesInRange.Add(collision.gameObject);
            }
            else if (!enemiesInRange.Contains(collision.gameObject))
            {
                enemiesInRange.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (enemiesInRange.Count > 0)
            {
                enemiesInRange.Remove(collision.gameObject);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, debugRange);
    }
}