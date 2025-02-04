using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAbility : EnemyAbilityBase
{
    [SerializeField]
    public EnemyAbilityData shootAbilityData;
    private Coroutine shootCoroutine;

    public float shootProjectileSpeed = 15f;
    public float shootProjectileDelay = 0.6f;
    private Vector2 direction;
    private bool hasAttacked;

    public override bool CanUseAbility(EnemyAI enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        return distanceToPlayer < shootAbilityData.detectRadius && IsAbilityReady();
    }

    public override void UseAbility(EnemyAI enemy)
    {
        if (!hasAttacked)
        {
            HandleShooting(enemy);
        }
    }

    private void HandleShooting(EnemyAI enemy)
    {
        hasAttacked = true;
        enemy.rb.velocity = Vector2.zero;
        shootCoroutine = StartCoroutine(Shoot(enemy));
    }

    IEnumerator Shoot(EnemyAI enemy)
    {
        for (int i = 0; i < 2; i++)
        {
            direction = (enemy.player.position - transform.position).normalized;
            AudioManager.TriggerSound("Enemy_Ability_Shoot", transform.position, group:"Gunfire");
            enemy.animator.SetTrigger("Shoot");
            yield return new WaitForSeconds(shootProjectileDelay);  // Adjust this to match `Shoot` duration
            Debug.Log("Firing, shot:" + i);
            //enemy.animator.ResetTrigger("Shoot");
        }
        Debug.Log("Ending Firing, going to Idle");

        AbilityReset(enemy);  // Reset cooldowns and flags
        CompleteAbility();    // Final cleanup
    }

    public void ShootProjectile()
    {
        GameObject projectile = ObjectPoolManager.Instance.GetFromPool("EnemyProjectile", transform.position, Quaternion.identity);

        if (projectile != null)
        {
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Reset velocity
            }

            if (enemyProjectile != null)
            {
                enemyProjectile.SetStats(shootAbilityData.damage, shootAbilityData.knockbackForce);
            }

            rb.velocity = direction * shootProjectileSpeed;
        }
        else
        {
            Debug.LogError("No projectile available in pool!");
            // Optionally exit the coroutine early if critical
            StopCoroutine(shootCoroutine);
        }
    }

    public override void InterruptAbility(EnemyAI enemy)
    {
        Debug.Log("Interrupting Shoot Ability");

        if (shootCoroutine != null)
        {
            enemy.StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void AbilityReset(EnemyAI enemy)
    {
        enemy.animator.ResetTrigger("Shoot");
        StartCooldown(enemy, shootAbilityData.cooldown);
        shootCoroutine = null;
        Debug.Log("Ending.. Starting cooldown!");
        hasAttacked = false;  // Reset flag to ensure ability can be reused cleanly
    }
}
