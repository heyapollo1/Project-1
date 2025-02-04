using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellAbility : EnemyAbilityBase
{
    [SerializeField]
    public EnemyAbilityData bulletHellAbilityData;

    private Coroutine bulletHellCoroutine;
    public float shootProjectileSpeed = 7.5f;
    public float shootProjectileDelay = 0.6f;  // Delay between each volley
    public int firePoints = 12;  // Number of firing points
    public int totalVolleys = 5;  // Total volleys to shoot

    private bool hasAttacked;

    public override bool CanUseAbility(EnemyAI enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        return distanceToPlayer < bulletHellAbilityData.detectRadius && IsAbilityReady();
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
        bulletHellCoroutine = StartCoroutine(BulletHellSweep(enemy));
    }

    IEnumerator BulletHellSweep(EnemyAI enemy)
    {
        float angleStep = 360f / firePoints;

        for (int volley = 0; volley < totalVolleys; volley++)
        {
            float startAngle = volley * (360f / totalVolleys);

            for (int point = 0; point < firePoints; point++)
            {
                float angle = startAngle + (angleStep * point);
                ShootProjectileAtAngle(angle);
            }

            AudioManager.TriggerSound("Enemy_Ability_Shoot", transform.position, group: "Gunfire");
            enemy.animator.SetTrigger("Shoot");

            yield return new WaitForSeconds(shootProjectileDelay);
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    private void ShootProjectileAtAngle(float angle)
    {
        GameObject projectile = ObjectPoolManager.Instance.GetFromPool("EnemyProjectile", transform.position, Quaternion.Euler(0, 0, angle));

        if (projectile != null)
        {
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

            if (rb != null)
            {
                rb.velocity = direction * shootProjectileSpeed;
            }

            EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
            if (enemyProjectile != null)
            {
                enemyProjectile.SetStats(bulletHellAbilityData.damage, bulletHellAbilityData.knockbackForce);
            }
        }
        else
        {
            Debug.LogError("No projectile available in pool!");
        }
    }

    public override void InterruptAbility(EnemyAI enemy)
    {
        Debug.Log("Interrupting Shoot Ability");

        if (bulletHellCoroutine != null)
        {
            enemy.StopCoroutine(bulletHellCoroutine);
            bulletHellCoroutine = null;
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void AbilityReset(EnemyAI enemy)
    {
        enemy.animator.ResetTrigger("Shoot");
        StartCooldown(enemy, bulletHellAbilityData.cooldown);
        bulletHellCoroutine = null;
        Debug.Log("Ending.. Starting cooldown!");
        hasAttacked = false;
    }
}