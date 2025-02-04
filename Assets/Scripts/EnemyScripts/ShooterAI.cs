/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAI : EnemyBase
{
    public float maintainDistance; 

    [Header("Shoot Settings")]
    public GameObject shootProjectilePrefab; // Prefab for the projectile
    public float shootProjectileSpeed;
    public float shootCooldown; // Cooldown between ranged attacks
    public float shootProjectileDelay;
    public float shootAttackRange; // Range within which the enemy can perform a ranged attack

    [Header("Melee Settings")]
    public int meleeDamage;
    public float meleePreparationTime; // Time before the attack hits
    public float meleeCooldown; // Cooldown between melee attacks
    public float meleeRange;
    public float meleeKnockbackForce;

    private float meleeCooldownTimer = 0f;
    private float shootCooldownTimer = 0f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void StateHandler()
    {
        HandleCooldown(ref shootCooldownTimer);
        HandleCooldown(ref meleeCooldownTimer);

        switch (currentState)
        {
            case EnemyState.Chasing:
                HandleChasing();
                break;

            case EnemyState.Shooting:
                HandleShooting();
                break;

            case EnemyState.Melee:
                HandleMelee();
                break;
        }
    }

    void HandleChasing()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= meleeRange && meleeCooldownTimer <= 0f)
        {
            currentState = EnemyState.Melee;
        }
        else if (distance <= shootAttackRange && distance > maintainDistance && shootCooldownTimer <= 0f)
        {
            currentState = EnemyState.Shooting;
        }
        else if (distance > maintainDistance)
        {
            // Move towards the player if too far away;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (distance < maintainDistance)
        {
            // Move away from the player if too close
            Vector2 direction = (transform.position - player.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + new Vector3(direction.x, direction.y), speed * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        if (!isAttacking)
        {
            StartCoroutine(Shoot());
        }
    }

    void HandleMelee()
    {
        if (!isAttacking)
        {
            StartCoroutine(Melee());
        }
    }

    IEnumerator Shoot()
    {
        isAttacking = true;
        //animator.SetTrigger("RangedAttack"); // Trigger ranged attack animation

        // Wait for animation preparation (if needed)
        yield return new WaitForSeconds(0.5f); // Adjust based on animation length

        for (int i = 0; i < 3; i++)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            GameObject projectile = Instantiate(shootProjectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * shootProjectileSpeed;

            yield return new WaitForSeconds(shootProjectileDelay); // Delay between each projectile
        }

        shootCooldownTimer = shootCooldown;
        currentState = EnemyState.Chasing;
        isAttacking = false;
    }

    IEnumerator Melee()
    {
        Debug.Log("Melee Attack Start!");
        isAttacking = true;

        // Optionally trigger a melee animation here
        // animator.SetTrigger("Melee");

        yield return new WaitForSeconds(meleePreparationTime); // Delay before dealing damage

        Debug.Log("Strike!");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, meleeRange);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                HealthManager healthManager = collider.GetComponent<HealthManager>();
                if (healthManager != null)
                {
                    healthManager.TakeDamage(meleeDamage, knockbackDirection, meleeKnockbackForce, KnockbackType.Light);
                }
            }
        }

        meleeCooldownTimer = meleeCooldown;
        currentState = EnemyState.Chasing; // Move back to chasing after the bite
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState == EnemyState.Idle)
        {
            currentState = EnemyState.Chasing;
        }
    }
}*/