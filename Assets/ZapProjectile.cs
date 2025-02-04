using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZapProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject impactEffect;  // Visual effect for bullet impact∂

    private Transform target;
    private float speed;
    private float damage;
    private float knockbackForce = 1f; // Knockback force applied on impact

    public float destroyTime = 3f;   // Time after which the bullet will self-destruct

    private bool hasHit = false;     // Prevent the bullet from applying damage multiple times


    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    public void ProjectileStats(Transform newTarget, float projectileSpeed, float projectileDamage, float projectileKnockbackForce)
    {
        target = newTarget;
        speed = projectileSpeed;
        damage = projectileDamage;
        knockbackForce = projectileKnockbackForce;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);  // Destroy the projectile if the target is gone
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // Detect collision with enemies or objects
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;  // Prevent double hits from applying damage multiple times
        {
            if (collision.CompareTag("InvincibleObstacle"))
            {
                Impact();
            }
            else if (collision.CompareTag("Enemy"))
            {
                hasHit = true;

                EnemyHealthManager enemyHealth = collision.GetComponent<EnemyHealthManager>();
                if (enemyHealth != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                    enemyHealth.TakeDamage(damage, knockbackDirection, knockbackForce);  // KnockbackType.Bullet for regular bullet
                }
                Impact();
            }

            else if (collision.CompareTag("BreakableObstacle"))
            {
                hasHit = true;

                ObjectHealthManager destructable = collision.GetComponent<ObjectHealthManager>();
                if (destructable != null)
                {
                    destructable.TakeDamage(damage);  // Only apply damage, no knockback
                }

                Impact();
            }
        }
    }


    private void Impact()
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
