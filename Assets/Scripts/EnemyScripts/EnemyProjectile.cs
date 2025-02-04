using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject impactEffect;
    private float damage;
    private float knockbackForce;
    private bool hasHit = false;

    public float destroyTime = 2.5f;

    private void OnEnable()
    {
        hasHit = false;
        StartCoroutine(ReturnToPoolAfterTime());
    }

    public void SetStats(float damageAmount, float knockBackAmount)
    {
        damage = damageAmount;
        knockbackForce = knockBackAmount;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        switch (collision.gameObject.tag)
        {
            case "InvincibleObstacle":
                Impact();
                break;

            case "BreakableObstacle":
                Impact();
                break;

            case "Player":
                PlayerHealthManager playerHealth = collision.GetComponent<PlayerHealthManager>();
                if (playerHealth != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerHealth.TakeDamage(damage, knockbackDirection, knockbackForce);
                }
                Impact();
                break;
        }
    }

    public void Impact()
    {
        if (!hasHit)
        {
            hasHit = true;
            rb.velocity = Vector2.zero;
            StopAllCoroutines();
            FXManager.Instance.PlayFX("EnemyBulletImpactFX", transform.position);
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }

    private IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        if (!hasHit)
        {
            hasHit = true;
            rb.velocity = Vector2.zero;
            if (gameObject.activeSelf)
            {
                ObjectPoolManager.Instance.ReturnToPool(gameObject);
            }
        }
    }
}