using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "MeleeAbility", menuName = "Abilities/Melee")]
public class MeleeAbility : EnemyAbilityBase
{
    [SerializeField]
    public EnemyAbilityData meleeAbilityData;
    private Coroutine meleeCoroutine;

    private bool hasAttacked; 
    public float attackDuration = 0.6f;

    private Vector2 attackColliderSize = new Vector2(1, 1);

    public override bool CanUseAbility(EnemyAI enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        return distanceToPlayer < meleeAbilityData.detectRadius && IsAbilityReady(); // Check range and cooldown
    }

    public override void UseAbility(EnemyAI enemy)
    {
        if (!hasAttacked)
        {
            HandleMelee(enemy); 
        }
    }

    private void HandleMelee(EnemyAI enemy)
    {
        hasAttacked = true;
        enemy.animator.SetTrigger("MeleeAttack");
        meleeCoroutine = StartCoroutine(MeleePreparation(enemy));
    }

    private IEnumerator MeleePreparation(EnemyAI enemy)
    {
        // Attack preparation logic or animation can occur here
        yield return new WaitForSeconds(attackDuration);  // Wait for preparation
        PerformMeleeAttack(enemy);
        AbilityReset(enemy);
        CompleteAbility();
    }

    private void PerformMeleeAttack(EnemyAI enemy)
    {
        Vector2 attackPosition = enemy.transform.position + (enemy.player.position - enemy.transform.position).normalized * meleeAbilityData.attackRange;
        AudioManager.TriggerSound("Enemy_Ability_Bite", transform.position);

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackPosition, attackColliderSize, 0f);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                
                PlayerHealthManager playerHealth = collider.GetComponent<PlayerHealthManager>();
                if (playerHealth != null)
                {
                    Vector2 knockbackDirection = (collider.transform.position - enemy.transform.position).normalized;
                    playerHealth.TakeDamage(meleeAbilityData.damage, knockbackDirection, meleeAbilityData.knockbackForce);
                }
            }
            else if (collider.CompareTag("BreakableObstacle"))
            {
                // Deal damage to breakable objects
                ObjectHealthManager destructible = collider.GetComponent<ObjectHealthManager>();
                if (destructible != null)
                {
                    destructible.TakeDamage(meleeAbilityData.damage);
                }
            }
        }
    }

    public override void InterruptAbility(EnemyAI enemy)
    {
        Debug.Log("Interrupting Melee Ability");

        if (meleeCoroutine != null)
        {
            enemy.StopCoroutine(meleeCoroutine);
            meleeCoroutine = null;
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void AbilityReset(EnemyAI enemy)
    {
        enemy.animator.ResetTrigger("MeleeAttack"); 
        StartCooldown(enemy, meleeAbilityData.cooldown);
        meleeCoroutine = null;
        hasAttacked = false;  // Reset flag to ensure ability can be reused cleanly
    }
}
