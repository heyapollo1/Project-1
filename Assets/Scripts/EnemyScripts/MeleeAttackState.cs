/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : IEnemyState
{
    private bool hasAttacked; // To track if the attack has been performed
    
    private bool isMovingDuringPrep;
    private bool isMovingDuringAttack;
    private float prepDuration;
    private float attackDuration;
    private float meleeDamage;
    private float meleeRange;
    private float knockbackForce;
    private Vector2 attackColliderSize;

    public MeleeAttackState(bool isMovingDuringPrep, bool isMovingDuringAttack, float prepDuration, float attackDuration, float meleeDamage, float meleeRange, Vector2 attackColliderSize, float knockbackForce)
    {
        this.isMovingDuringPrep = isMovingDuringPrep;
        this.isMovingDuringAttack = isMovingDuringAttack;
        this.prepDuration = prepDuration;
        this.attackDuration = attackDuration;
        this.meleeDamage = meleeDamage;
        this.meleeRange = meleeRange;
        this.knockbackForce = knockbackForce;
        this.attackColliderSize = attackColliderSize;
    }

    public void EnterState(EnemyAI enemy)
    {
        hasAttacked = false;

        //PerformAttack(enemy);

        // If the enemy is allowed to move during the preparation phase, keep movement active
        if (isMovingDuringPrep)
        {
            enemy.enemyMovement.StartMovement();  // Continue movement/pathfinding
        }
        else
        {
            enemy.enemyMovement.StopMovement();  // Stop movement if needed
        }

        // Trigger the melee preparation animation
        enemy.animator.SetTrigger("MeleePreparation");
        enemy.FaceDirection(enemy.directionToPlayer);
        enemy.StartCoroutine(HandleMeleePreparation(enemy));
    }

    public void UpdateState(EnemyAI enemy)
    {
        // Check for attack cooldown, or any special logic for canceling the attack, like knockback
        if (hasAttacked && !enemy.CanAttack()) // Just a safeguard, can remove if handled in coroutines
        {
            TransitionAfterAttack(enemy);
        }

        enemy.FaceDirection(enemy.directionToPlayer);
    }

    private IEnumerator HandleMeleePreparation(EnemyAI enemy)
    {
        // Attack preparation logic or animation can occur here
        yield return new WaitForSeconds(prepDuration);  // Wait for preparation

        if (isMovingDuringAttack)
        {
            enemy.enemyMovement.StartMovement();  // Continue movement/pathfinding
        }
        else
        {
            enemy.enemyMovement.StopMovement();  // Continue movement/pathfinding
        }
        Debug.Log("Starting bite anim");
        enemy.animator.SetTrigger("MeleeAttack");
        enemy.StartCoroutine(HandleMeleeAttackDuration(enemy));
    }

    private IEnumerator HandleMeleeAttackDuration(EnemyAI enemy)
    {
        // Attack preparation logic or animation can occur here
        yield return new WaitForSeconds(attackDuration);  // Wait for preparation

        // Check for hitbox collisions during the attack
        PerformMeleeAttack(enemy);
        // Trigger cooldown and switch state
        enemy.ResetMeleeAttackCooldown();
        hasAttacked = true;
    }

    private void PerformMeleeAttack(EnemyAI enemy)
    {
        Vector2 attackPosition = enemy.transform.position + (enemy.player.position - enemy.transform.position).normalized * meleeRange;

        // Detect if the player or destructible objects are hit within the attack range
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackPosition, attackColliderSize, 0f);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Deal damage to the player
                PlayerHealthManager playerHealth = collider.GetComponent<PlayerHealthManager>();
                if (playerHealth != null)
                {
                    Vector2 knockbackDirection = (collider.transform.position - enemy.transform.position).normalized;
                    playerHealth.TakeDamage(meleeDamage, knockbackDirection, knockbackForce, KnockbackType.Light);
                }
            }
            else if (collider.CompareTag("BreakableObstacle"))
            {
                // Deal damage to breakable objects
                DestructibleProp destructible = collider.GetComponent<DestructibleProp>();
                if (destructible != null)
                {
                    destructible.TakeDamage(meleeDamage);
                }
            }
        }
    }

    private void TransitionAfterAttack(EnemyAI enemy)
    {
        if (enemy.IsPlayerTooClose())
        {
            enemy.TransitionToState(enemy.idleState);  // Transition to idle state if the player is close
        }
        else
        {
            enemy.TransitionToState(enemy.movingState);  // Resume chasing if the player is farther away
        }
    }

    public void ExitState(EnemyAI enemy)
    {
        enemy.enemyMovement.StopMovement();
        // Optionally reset movement or cleanup after attack
        //enemy.enemyMovement.StopMovement();  // Ensure movement is stopped when exiting attack
    }
}*/
