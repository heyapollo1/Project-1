using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        Debug.Log("Entering STUN State");
        enemy.isStunned = true;

        enemy.InterruptCurrentAbility();

        enemy.abilityManager.enabled = false;
        enemy.pathfinding.enabled = false;
    }

    public void UpdateState(EnemyAI enemy)
    {
        enemy.animator.SetTrigger("Stunned");
    }

    public void ExitState(EnemyAI enemy)
    {
        enemy.isStunned = false;
        enemy.abilityManager.enabled = true;
        enemy.pathfinding.enabled = true;
        enemy.animator.ResetTrigger("Stunned");

        Debug.Log($"{enemy.name} is no longer stunned.");
    }
}