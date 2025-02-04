using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject impactEffect;
    public Sprite muzzleFlash;
    public TrailRenderer trail;
    private Sprite defaultSprite;

private float damage;  
    private float currentKnockbackForce;
    private bool willInflictBleed;
    private bool willInflictBurn;
    private bool isCriticalHit;

    public int framesToFlash = 3; 
    public float destroyTime = 2f;  

    private SpriteRenderer spriteRend;
    private bool hasHit = false; 

    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        willInflictBleed = PlayerStatManager.Instance.AttemptToApplyStatusEffect(StatType.BleedChance);
        willInflictBurn = PlayerStatManager.Instance.AttemptToApplyStatusEffect(StatType.BurnChance);
        defaultSprite = spriteRend.sprite;
    }

    private void OnEnable()
    {
        hasHit = false;
        if (trail != null)
        {
            trail.Clear();
        }

        StartCoroutine(ReturnToPoolAfterTime());
    }

    public void SetStats(float damageAmount, float knockBackAmount, bool criticalHit = false)
    {
        damage = damageAmount;
        currentKnockbackForce = knockBackAmount;
        isCriticalHit = criticalHit;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.TryGetComponent(out IDamageable damageable))
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            damageable.TakeDamage(damage, knockbackDirection, currentKnockbackForce, isCriticalHit);

            if (willInflictBleed)
            {
                StatusEffectManager.Instance.ApplyBleedEffect(collision.gameObject, 5f, PlayerStatManager.Instance.currentDamage / 2, 1f);
            }

            if (willInflictBurn)
            {
                StatusEffectManager.Instance.ApplyBurnEffect(collision.gameObject, 5f, PlayerStatManager.Instance.currentDamage / 2);
            }

            Impact();
        }
        else if (collision.gameObject.CompareTag("BreakableObstacle"))
        {
            ObjectHealthManager obstacle = collision.gameObject.GetComponent<ObjectHealthManager>();
            obstacle.TakeDamage(damage, isCriticalHit);
            Impact();
        }
        else if (collision.gameObject.CompareTag("InvincibleObstacle"))
        {
            Impact();
        }
    }

    private void Impact()
    {
        hasHit = true;
        isCriticalHit = false;
        rb.velocity = Vector2.zero;
        StopAllCoroutines();
        AudioManager.TriggerSound("Weapon_Impact", transform.position);
        FXManager.Instance.PlayFX("BulletImpactFX", transform.position);
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }

    private IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        if (!hasHit)
        {
            hasHit = true;
            isCriticalHit = false;
            rb.velocity = Vector2.zero;
            if (gameObject.activeSelf)
            {
                ObjectPoolManager.Instance.ReturnToPool(gameObject);
            }
        }
    }
}
