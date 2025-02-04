using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeDeathEffect : IOnDeathEffect
{
    private float radius;
    private float damage;
    private float knockbackForce;
    private GameObject explosionPrefab;

    public KamikazeDeathEffect(float radius, float damage, float knockbackForce, GameObject explosionPrefab)
    {
        this.radius = radius;
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.explosionPrefab = explosionPrefab;
    }

    public void Execute(GameObject enemy)
    {
        Debug.Log("Trigger explosion kamikaze");
        Animator animator = enemy.GetComponent<Animator>();
        animator.SetTrigger("Power");
        InstantiateExplosionAnimation(enemy.transform.position);
        AudioManager.TriggerSound("Enemy_Kamikaze", enemy.transform.position);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                var playerHealth = hitCollider.GetComponent<PlayerHealthManager>();
                if (playerHealth != null)
                {
                    Vector2 knockbackDirection = (playerHealth.transform.position - enemy.transform.position).normalized;
                    playerHealth.TakeDamage(damage, knockbackDirection, knockbackForce);
                }
            }

            if (hitCollider.CompareTag("Enemy") && hitCollider.gameObject != enemy.gameObject)
            {
                var otherEnemyHealth = hitCollider.GetComponent<EnemyHealthManager>();
                if (otherEnemyHealth != null)
                {
                    Vector2 knockbackDirection = (otherEnemyHealth.transform.position - enemy.transform.position).normalized;
                    otherEnemyHealth.TakeDamage(damage, knockbackDirection, knockbackForce);
                }
            }

            if (hitCollider.CompareTag("BreakableObstacle") && hitCollider.gameObject != enemy.gameObject)
            {
                var obstacle = hitCollider.GetComponent<ObjectHealthManager>();
                if (obstacle != null)
                {
                    obstacle.TakeDamage(damage);
                }
            }
        }
        Debug.Log("Explosion triggered on death of " + enemy.name);
    }

    private void InstantiateExplosionAnimation(Vector2 position)
    {
        Object.Instantiate(explosionPrefab, position, Quaternion.identity);
    }
}
