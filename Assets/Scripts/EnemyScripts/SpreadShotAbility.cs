using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShotAbility : EnemyAbilityBase
{
    [SerializeField]
    public EnemyAbilityData spreadShotAbilityData;
    private Coroutine spreadShotCoroutine;

    public float projectileSpeed = 12f;
    public float projectileDelay = 0.6f;
    private Vector2 direction;
    private bool hasAttacked;

    // Spread parameters
    public int projectileCount = 5;  // Number of projectiles per spread
    public float spreadAngle = 30f;  // Angle of spread in degrees

    public override bool CanUseAbility(EnemyAI enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        return distanceToPlayer < spreadShotAbilityData.detectRadius && IsAbilityReady();
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
        spreadShotCoroutine = StartCoroutine(Shoot(enemy));
    }

    IEnumerator Shoot(EnemyAI enemy)
    {
        for (int i = 0; i < 1; i++)
        {
            direction = (enemy.player.position - enemy.transform.position).normalized;
            enemy.animator.SetTrigger("Shoot");
            yield return new WaitForSeconds(projectileDelay);  // Delay between shots
            Debug.Log("Firing, shot:" + i);
        }
        Debug.Log("Ending Firing, going to Idle");

        AbilityReset(enemy);  // Reset cooldowns and flags
        CompleteAbility();    // Final cleanup
    }

    // This method is called via the animation event
    public void ShotgunBlast()
    {
        float startAngle = -spreadAngle / 2;  // Starting angle for the spread
        float angleStep = spreadAngle / (projectileCount - 1);  // Angle between projectiles

        for (int i = 0; i < projectileCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Vector2 spreadDirection = rotation * direction;  // Rotate direction by current angle

            // Instantiate the projectile
            GameObject projectile = Instantiate(spreadShotAbilityData.abilityVFXPrefab, transform.position, Quaternion.identity);

            float angle = Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Set projectile velocity
            projectile.GetComponent<Rigidbody2D>().velocity = spreadDirection * projectileSpeed;
            projectile.GetComponent<EnemyProjectile>().SetStats(spreadShotAbilityData.damage, spreadShotAbilityData.knockbackForce);
        }
    }

    public override void InterruptAbility(EnemyAI enemy)
    {
        Debug.Log("Interrupting Shoot Ability");

        if (spreadShotCoroutine != null)
        {
            enemy.StopCoroutine(spreadShotCoroutine);
            spreadShotCoroutine = null;
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void AbilityReset(EnemyAI enemy)
    {
        enemy.animator.ResetTrigger("Shoot");
        StartCooldown(enemy, spreadShotAbilityData.cooldown);
        spreadShotCoroutine = null;
        Debug.Log("Ending.. Starting cooldown!");
        hasAttacked = false;
    }
}
