using System.Collections;
using UnityEngine;

public interface IKnockbackable
{
    void ApplyKnockback(Vector2 direction, float force);
}

public class KnockbackManager : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isInKnockback = false;
    private float currentKnockbackForce;

    [Header("Knockback Settings")]
    public float knockbackCooldown = 0.2f;
    public float knockbackDuration = 0.3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction, float force, bool isPlayer)
    {
        if (CanApplyKnockback())
        {
            currentKnockbackForce = force;
            isInKnockback = true;
            if (isPlayer)
            {
                rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
                StartCoroutine(KnockbackRecovery());
            }
            else
            {
                StartCoroutine(KnockbackCoroutine(direction, force));
            }
        }
    }

    private bool CanApplyKnockback()
    {
        return !isInKnockback;
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float force)
    {
        float timer = 0f;
        Vector2 knockbackDirection = direction.normalized;

        while (timer < knockbackDuration)
        {
            knockbackDirection = HandleWallCollision(knockbackDirection);

            float knockbackInfluence = Mathf.Lerp(1, 0, timer / knockbackDuration);
            Vector2 knockbackMovement = knockbackDirection * currentKnockbackForce * knockbackInfluence * Time.deltaTime;

            rb.MovePosition(rb.position + knockbackMovement);

            timer += Time.deltaTime;
            yield return null;
        }
        isInKnockback = false;
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isInKnockback = false;
    }

    private Vector2 HandleWallCollision(Vector2 currentDirection)
    {
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, rb.GetComponent<Collider2D>().bounds.size, 0, currentDirection, 0.1f, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            Vector2 collisionNormal = hit.normal;
            currentDirection = Vector2.Reflect(currentDirection, collisionNormal);
            currentKnockbackForce *= 0.4f; 
        }
        return currentDirection;
    }

    public class PlayerKnockback : MonoBehaviour, IKnockbackable
    {
        private KnockbackManager knockbackManager;

        private void Awake()
        {
            knockbackManager = GetComponent<KnockbackManager>();
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            knockbackManager.ApplyKnockback(direction, force, isPlayer: true);
        }
    }

    public class EnemyKnockback : MonoBehaviour, IKnockbackable
    {
        private KnockbackManager knockbackManager;

        private void Awake()
        {
            knockbackManager = GetComponent<KnockbackManager>();
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            knockbackManager.ApplyKnockback(direction, force, isPlayer: false);
        }
    }

    public bool IsInKnockback()
    {
        return isInKnockback;
    }
}