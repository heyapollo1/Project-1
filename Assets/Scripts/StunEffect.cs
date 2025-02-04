using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : StatusEffect
{
    private EnemyAI enemy;

    public StunEffect(float duration, EnemyHealthManager targetEnemy, MonoBehaviour owner)
        : base(duration, 0, owner, isEternal: false)
    {
        enemy = targetEnemy.GetComponent<EnemyAI>();
    }

    protected override IEnumerator EffectCoroutine()
    {
        if (enemy != null)
        {
            enemy.TransitionToState(enemy.stunnedState);

            Debug.Log($"{enemy.gameObject.name} is stunned for {Duration} seconds.");

            yield return new WaitForSeconds(Duration);

            enemy.TransitionToState(enemy.idleState);
        }

        Remove(enemy.gameObject, false);
    }

    public override void ResetDuration(float newDuration, float newDamage)
    {
        Duration = newDuration;
        Debug.Log("Stun duration refreshed to " + newDuration);
    }
}