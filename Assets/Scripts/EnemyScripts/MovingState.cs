using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        enemy.animator.SetBool("isWalking", true); 
        enemy.enemyMovement.StartMovement();
    }

    public void UpdateState(EnemyAI enemy)
    {
        // Check if an ability should be triggered
        if (enemy.TryUseSpecialAbility())
        {
            return;
        }

        if (enemy.IsPlayerAtEffectiveDistance())
        {
            enemy.TransitionToState(enemy.evadingState);
        }

        if (enemy.IsPlayerTooClose())
        {
            enemy.TransitionToState(enemy.idleState);
        }
    }

    public void ExitState(EnemyAI enemy)
    {
        enemy.animator.SetBool("isWalking", false);
        enemy.enemyMovement.StopMovement();  // Stop movement when exiting chasing state
    }
}
