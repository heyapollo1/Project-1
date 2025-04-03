using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnDeathEffect : IOnDeathEffect
{
    private float radius;
    private float damage;
    private float knockbackForce;
    private GameObject explosionPrefab;

    public BurnDeathEffect(float radius, float damage, float knockbackForce)
    {
        this.radius = radius;
        this.damage = damage;
        this.knockbackForce = knockbackForce;
    }

    public void Execute(GameObject enemy)
    {
        explosionPrefab = FXManager.Instance.PlayFX("BurnExplosionFX", enemy.transform.position);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, radius);

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject != enemy.gameObject)
            {
                var otherEnemyHealth = collider.GetComponent<EnemyHealthManager>();
                if (otherEnemyHealth != null)
                {
                    StatusEffectManager.Instance.ApplyBurnEffect(collider.gameObject, 5f, AttributeManager.Instance.currentDamage / 2);
                    Vector2 knockbackDirection = (otherEnemyHealth.transform.position - enemy.transform.position).normalized;
                    otherEnemyHealth.TakeDamage(damage, knockbackDirection, knockbackForce);
                }
            }

            if (collider.CompareTag("BreakableObstacle") && collider.gameObject != enemy.gameObject)
            {
                var obstacle = collider.GetComponent<ObjectHealthManager>();
                if (obstacle != null)
                {
                    obstacle.TakeDamage(damage);
                }
            }
        }
    }
}

