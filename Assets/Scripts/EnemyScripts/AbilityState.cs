using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState : IEnemyState
{
    private EnemyAbilityBase currentAbility;
    private bool isAbilityFinished = false;

    public AbilityState(EnemyAbilityBase ability)
    {
        currentAbility = ability;
    }

    public void EnterState(EnemyAI enemy)
    {
        // Set the `currentAbility` on the `EnemyAI` so that it can be accessed or interrupted globally
        enemy.SetCurrentAbility(currentAbility);
        enemy.isPerformingAbility = true;

        currentAbility.onAbilityCompleted = OnAbilityCompleted;

        currentAbility.UseAbility(enemy);
    }

    public void UpdateState(EnemyAI enemy)
    {
        if (isAbilityFinished)
        {
            if (enemy.IsPlayerTooClose())
            {
                enemy.TransitionToState(enemy.idleState);
            }
            else
            {
                enemy.TransitionToState(enemy.movingState);
            }
        }
    }

    public void ExitState(EnemyAI enemy)
    {
        //currentAbility.AbilityReset(enemy);
        enemy.isPerformingAbility = false;
        isAbilityFinished = false;
        currentAbility.onAbilityCompleted = null;
    }

    // Finish ability = call this
    private void OnAbilityCompleted()
    {
        isAbilityFinished = true;
    }
}
