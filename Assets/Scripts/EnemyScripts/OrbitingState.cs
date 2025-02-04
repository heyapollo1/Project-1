using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        enemy.animator.SetBool("isWalking", true);
        //enemy.enemyMovement.StartOrbiting();
    }

    public void UpdateState(EnemyAI enemy)
    {
        if (enemy.TryUseSpecialAbility())
        {
            return;
        }

        if (enemy.IsPlayerAtEffectiveDistance())
        {
            enemy.TransitionToState(enemy.evadingState);
        }

        if (enemy.IsPlayerTooClose() && !enemy.IsPlayerAtEffectiveDistance())
        {
            enemy.TransitionToState(enemy.idleState);
        }
    }

    public void ExitState(EnemyAI enemy)
    {
        enemy.animator.SetBool("isOrbiting", false);
    }
}