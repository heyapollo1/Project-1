using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaveEffect : MonoBehaviour
{
    private float damage;
    private float knockbackForce;
    private float critChance;
    private float critDamage;
    private float spinAmount;

    private Transform player;
    private LayerMask enemyLayer;
    private MaterialPropertyBlock propertyBlock;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float startSwingSpeed = 360f;   // Start speed
    private float maxSwingSpeed = 750f;     // Peak speed
    private float endSwingSpeed = 120f;      // slow-down speed
    float targetAngle = 360f;
    float accumulatedRotation = 0f;
    private bool isSwinging = false;

    public void Initialize(LayerMask enemyLayer)
    {
        this.enemyLayer = enemyLayer;

        transform.localPosition = Vector3.right * 1.5f;
        propertyBlock = new MaterialPropertyBlock();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (!isSwinging)
        {
            Debug.Log("trigger anim");
            animator.Play("SpawnAnimation");
            StartCoroutine(HandleCleave());
        }
    }

    private void OnEnable()
    {
        accumulatedRotation = 0f;
        isSwinging = false;
        //spriteRenderer.SetPropertyBlock(propertyBlock);
        //propertyBlock.SetFloat("_FadeAmount", 1f);
    }

    public void SetStats(float damage, float knockbackForce, float critChance, float critDamage, float spinAmount, Vector2 cleaveSize, Transform player)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.critChance = critChance;
        this.critDamage = critDamage;
        this.spinAmount = spinAmount;
        this.player = player;
        transform.localScale = cleaveSize;

        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    private IEnumerator HandleCleave()
    {
        yield return new WaitForSeconds(0.6f);

        Debug.Log("trigger swing");
        yield return PerformSwings();
        //yield return StartCoroutine(FadeOut(0.2f));

        if (!isSwinging)
        {
            transform.localPosition = Vector3.right * 1.5f; // end swing

            yield return new WaitForSeconds(0.3f);

            transform.SetParent(null);
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
            Debug.Log("Cleave ended and deactivated");
        }
    }

    private IEnumerator PerformSwings()
    {
        animator.SetBool("IsSwinging", true);
        isSwinging = true;
        float accelerationAngleThreshold = targetAngle * 0.70f;
        float totalSwingTime = 0f;

        for (int i = 0; i < spinAmount; i++)
        {
            accumulatedRotation = 0f;
 
            while (accumulatedRotation < targetAngle)
            {
                // Calculate swing speed based on accumulated angle
                float currentSwingSpeed;
                if (accumulatedRotation < accelerationAngleThreshold)
                {
                    // Accelerate from start to max speed for the first 75% of the angle
                    currentSwingSpeed = Mathf.Lerp(startSwingSpeed, maxSwingSpeed, accumulatedRotation / accelerationAngleThreshold);
                }
                else
                {
                    // Decelerate from max to end speed for the remaining 25% of the angle
                    currentSwingSpeed = Mathf.Lerp(maxSwingSpeed, endSwingSpeed, (accumulatedRotation - accelerationAngleThreshold) / (targetAngle - accelerationAngleThreshold));
                }

                float rotationStep = currentSwingSpeed * Time.deltaTime;
                transform.RotateAround(player.position, Vector3.forward, rotationStep);
                accumulatedRotation += rotationStep;

                Vector3 directionToPlayer = player.position - transform.position;
                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + 90f); // Adjust by 90 deg so blade edge faces outward!

                totalSwingTime += Time.deltaTime;
                yield return null;
            }
        }

        animator.SetBool("IsSwinging", false);
        Debug.Log("Total swing time calculated: " + totalSwingTime + " seconds");
        totalSwingTime = 0;
        isSwinging = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & enemyLayer) != 0)
        {
            if (collider.TryGetComponent(out EnemyHealthManager enemyHealth))
            {
                bool isCritical = Random.value < critChance;
                float finalDamage = isCritical ? damage * critDamage : damage;

                Vector2 knockbackDirection = (collider.transform.position - transform.position).normalized;
                enemyHealth.TakeDamage(finalDamage, knockbackDirection, knockbackForce, DamageSource.Player, isCritical);
            }
        }
    }
}