using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAbility : EnemyAbilityBase
{
    [SerializeField]
    public EnemyAbilityData chargeAbilityData;
     
    public float chargeDuration = 0.5f;        
    public float trailSpawnInterval = 0.1f;

    private bool isCharging;
    private Coroutine chargeCoroutine;
    private HashSet<Collider2D> hitObstacles = new HashSet<Collider2D>();
    private GameObject chargeTrailPrefab;
    private float lastTrailSpawnTime;

    public override bool CanUseAbility(EnemyAI enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        return distanceToPlayer < chargeAbilityData.detectRadius && IsAbilityReady(); // Check range and cooldown
    }

    public override void UseAbility(EnemyAI enemy)
    {
        if (!isCharging)
        {
            HandleCharge(enemy);
        }
    }

    private void HandleCharge(EnemyAI enemy)
    {
        //Debug.Log("Charge Preparing..");
        isCharging = true;
        AudioManager.TriggerSound("Charge_Start", transform.position);
        enemy.animator.SetTrigger("ChargePreparation");
        Debug.Log("ChargePreparation Trigger Set");
        chargeCoroutine = enemy.StartCoroutine(ChargePreparation(enemy));
    }

    private IEnumerator ChargePreparation(EnemyAI enemy)
    {
        enemy.FaceDirection(enemy.directionToPlayer);
        yield return new WaitForSeconds(1f);  // Adjust based on animation length

        Vector2 chargeDirection = (enemy.directionToPlayer).normalized;
        enemy.overrideFacing = true;  // Prevent default FaceDirection
        enemy.FaceDirection(chargeDirection);

        AudioManager.TriggerSound("Charge_Progress", transform.position);
        enemy.animator.SetTrigger("Charge");

        float elapsedTime = 0f;

        while (elapsedTime < chargeDuration)
        {
            chargeTrailPrefab = Instantiate(chargeAbilityData.abilityVFXPrefab, transform.position, Quaternion.identity, transform);
            elapsedTime += Time.deltaTime;

            enemy.rb.MovePosition(enemy.rb.position + chargeDirection * chargeAbilityData.speed * Time.deltaTime);

            if (elapsedTime - lastTrailSpawnTime > trailSpawnInterval)
            {
                SpawnTrail(enemy);
                lastTrailSpawnTime = elapsedTime;
            }

            if (HandleCollisionsDuringCharge(enemy, chargeDirection))
            {
                break;
            }

            yield return null;
        }

        EndCharge(enemy);
        Debug.Log("ChargePreparation coroutine completed");
    }

    private void SpawnTrail(EnemyAI enemy)
    {
        // Instantiate the charge trail prefab at the enemy's current position
        if (chargeAbilityData != null && chargeAbilityData.abilityVFXPrefab != null)
        {
            Instantiate(chargeAbilityData.abilityVFXPrefab, enemy.transform.position, Quaternion.identity);
        }
    }

    // Handle collisions during the charge
    private bool HandleCollisionsDuringCharge(EnemyAI enemy, Vector2 chargeDirection)
    {
        // Raycast to detect walls
        RaycastHit2D wallHit = Physics2D.Raycast(enemy.rb.position, chargeDirection, 0.5f, LayerMask.GetMask("Wall"));
        if (wallHit.collider != null)
        {
            EndCharge(enemy);
            return true;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, 1.15f);  // Adjust detection radius
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealthManager playerHealth = hitCollider.GetComponent<PlayerHealthManager>();
                if (playerHealth != null)
                {
                    Vector2 knockbackDirection = (hitCollider.transform.position - enemy.transform.position).normalized;
                    playerHealth.TakeDamage(chargeAbilityData.damage, knockbackDirection, chargeAbilityData.knockbackForce);
                }

                EndCharge(enemy);
                return true;
            }

            // Check for collision with breakable obstacles
            if (hitCollider.CompareTag("BreakableObstacle") && !hitObstacles.Contains(hitCollider))
            {
                ObjectHealthManager breakable = hitCollider.GetComponent<ObjectHealthManager>();
                if (breakable != null)
                {
                    breakable.TakeDamage(chargeAbilityData.damage);
                    hitObstacles.Add(hitCollider);
                }
            }
        }
        return false;  // No collisions, continue the charge
    }

    private void EndCharge(EnemyAI enemy)
    {
        //Debug.Log("Ending Charge.");
        AudioManager.TriggerSound("Charge_End", transform.position);
        enemy.rb.velocity = Vector2.zero;
        hitObstacles.Clear();
        enemy.animator.SetTrigger("EndCharge");
        enemy.StartCoroutine(EndChargeDelay(enemy));  // Adjust based on animation length
    }

    private IEnumerator EndChargeDelay(EnemyAI enemy)
    {
        yield return new WaitForSeconds(0.3f);
        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void InterruptAbility(EnemyAI enemy)
    {
        Debug.Log("Interrupting Charge Ability");

        if (chargeCoroutine != null)
        {
            enemy.StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
        }

        AbilityReset(enemy);
        CompleteAbility();   
    }

    public override void AbilityReset(EnemyAI enemy)
    {
        Debug.Log("ability reset called.");
        enemy.overrideFacing = false;
        enemy.animator.ResetTrigger("EndCharge");  
        enemy.animator.ResetTrigger("ChargePreparation");  
        enemy.animator.ResetTrigger("Charge"); 
        StartCooldown(enemy, chargeAbilityData.cooldown);
        hitObstacles.Clear();
        chargeCoroutine = null;
        isCharging = false;  
    }
}
