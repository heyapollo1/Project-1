using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    private float idleDurationMin = 3.0f;
    private float idleDurationMax = 5.0f;
    private float idleTimer;

    public void EnterState(EnemyAI enemy)
    {
        enemy.rb.velocity = Vector2.zero;
        enemy.animator.SetBool("isWalking", false);

        if (!enemy.isAlerted && !enemy.isDisabled)
        {
            idleTimer = Random.Range(idleDurationMin, idleDurationMax);
        }

    }
  
    public void UpdateState(EnemyAI enemy)
    {
        if (enemy.isDisabled) return;

        if (!enemy.isAlerted)
        {
            idleTimer -= Time.deltaTime;

            if (enemy.IsPlayerInSightRange())
            {
                enemy.isAlerted = true;
            }
            else if (idleTimer <= 0)
            {
                enemy.TransitionToState(enemy.roamingState);
            }
        }

        if (enemy.isAlerted)
        {
            if (enemy.TryUseSpecialAbility())
            {
                return;
            }

            if (enemy.IsPlayerAtEffectiveDistance())
            {
                enemy.TransitionToState(enemy.evadingState);
            }

            if (!enemy.IsPlayerTooClose() && !enemy.IsPlayerAtEffectiveDistance())
            {
                enemy.TransitionToState(enemy.movingState);
            }
        }
    }

    public void ExitState(EnemyAI enemy)
    {
        
    }
}
