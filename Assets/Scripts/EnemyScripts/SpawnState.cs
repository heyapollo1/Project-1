using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        Debug.Log($"SpawnState Entered for {enemy.gameObject.name}");
        Debug.Log($"Animator Current State: {enemy.animator.GetCurrentAnimatorStateInfo(0).fullPathHash}");
        if (enemy.animator == null)
        {
            Debug.LogError($"Animator not initialized for {enemy.gameObject.name}");
            return;
        }
        enemy.animator.ResetTrigger("Power");
        enemy.animator.Play("SpawnAnimation");

        AudioManager.TriggerSound(clipPrefix: "Enemy_Spawn", position: enemy.transform.position);
        enemy.StartCoroutine(SpawnTransition(enemy));
    }

    public void UpdateState(EnemyAI enemy)
    {

    }

    public void ExitState(EnemyAI enemy)
    {

    }

    private IEnumerator SpawnTransition(EnemyAI enemy)
    {
        AnimatorStateInfo stateInfo = enemy.animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;

        yield return new WaitForSeconds(animationDuration);

        enemy.TransitionToState(enemy.idleState);
    }
}