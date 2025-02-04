using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadingState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        enemy.animator.SetBool("isWalking", true);
        enemy.enemyMovement.StartEvading();
    }

    public void UpdateState(EnemyAI enemy)
    {
        if (enemy.TryUseSpecialAbility())
        {
            return;
        }

        if (enemy.IsPlayerTooClose() && !enemy.IsPlayerAtEffectiveDistance())
        {
            enemy.TransitionToState(enemy.idleState);
        }
        else if (!enemy.IsPlayerTooClose())
        {
            enemy.TransitionToState(enemy.movingState);
        }
    }

    public void ExitState(EnemyAI enemy)
    {
        enemy.animator.SetBool("isWalking", false);
        enemy.enemyMovement.StopMovement();
    }
}
