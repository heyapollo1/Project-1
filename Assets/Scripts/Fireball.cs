using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private float damage;
    private float knockbackForce;
    private float projectileSpeed;
    private bool IsCriticalHit;

    //private LayerMask enemyLayer;
    private MaterialPropertyBlock propertyBlock;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private bool hasHit = false;

    public void Initialize(Vector2 aimDirection)
    {
        propertyBlock = new MaterialPropertyBlock();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (!hasHit)
        {
            Debug.Log("fireball cast anim");
            animator.Play("FireballCast");

            // Calculate rotation angle based on aim direction
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            StartCoroutine(LaunchFireball(aimDirection));
        }
    }

    private void OnEnable()
    {
        hasHit = false;
        //spriteRenderer.SetPropertyBlock(propertyBlock);
        //propertyBlock.SetFloat("_FadeAmount", 1f);
    }

    public void SetStats(float damage, float knockbackForce, float projectileSpeed, Vector2 fireballSize, bool IsCriticalHit = false)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.projectileSpeed = projectileSpeed;
        this.IsCriticalHit = IsCriticalHit;
        transform.localScale = fireballSize;
    }

    private IEnumerator LaunchFireball(Vector2 aimDirection)
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("FireballActive", true);
        Debug.Log("fireball cast");
        //FXManager.Instance.PlayFX(FXType.FireballLaunchFX, transform.position);

        GetComponent<Rigidbody2D>().AddForce(aimDirection * projectileSpeed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("InvincibleObstacle"))
        {
            StartCoroutine(Impact());
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out EnemyHealthManager enemyHealth))
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                enemyHealth.TakeDamage(damage, knockbackDirection, knockbackForce, DamageSource.Player, IsCriticalHit);
            }
            StartCoroutine(Impact());
        }
        else if (collision.CompareTag("BreakableObstacle"))
        {
            if (collision.TryGetComponent(out ObjectHealthManager destructable))
            {
                destructable.TakeDamage(damage);
            }
            StartCoroutine(Impact());
        }
    }

    private IEnumerator Impact()
    {
        hasHit = true;
        IsCriticalHit = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        animator.SetBool("FireballActive", false);
        yield return new WaitForSeconds(0.2f);
        //FXManager.Instance.PlayFX(FXType.FireballImpactFX, transform.position);
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
        Debug.Log("fireball ended and returned");
    }
}
