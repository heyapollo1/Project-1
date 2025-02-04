using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class EnemyAbilityBase : MonoBehaviour
{
    protected bool isOnCooldown = false;

    public Action onAbilityCompleted;

    public abstract bool CanUseAbility(EnemyAI enemy);
    public abstract void UseAbility(EnemyAI enemy);
    public abstract void AbilityReset(EnemyAI enemy);
    public abstract void InterruptAbility(EnemyAI enemy);

    public void StartCooldown(EnemyAI enemy, float cooldownDuration)
    {
        if (!isOnCooldown)
        {
            isOnCooldown = true;
            enemy.StartCoroutine(CooldownCoroutine(cooldownDuration));
        }
    }

    private IEnumerator CooldownCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndCooldown();
    }

    private void EndCooldown()
    {
        isOnCooldown = false;
    }

    public bool IsAbilityReady()
    {
        return !isOnCooldown;
    }

    protected void CompleteAbility()
    {
        if (onAbilityCompleted != null)
        {
            onAbilityCompleted.Invoke();
        }
    }
}
