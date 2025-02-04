using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float damage;

    public void SetTarget(Transform enemyTarget, float projectileSpeed, float projectileDamage)
    {
        target = enemyTarget;
        speed = projectileSpeed;
        damage = projectileDamage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move toward the target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // If we're close enough to the target, hit it
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        // Apply damage to the target
        EnemyHealthManager enemyHealth = target.GetComponent<EnemyHealthManager>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage, Vector2.zero, 0f);
        }

        // Destroy the projectile after hitting the target
        Destroy(gameObject);
    }
}
