using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeAbility : EnemyAbilityBase
{
    [SerializeField]
    public EnemyAbilityData kamikazeAbilityData;
    private Coroutine kamikazeCoroutine;

    public float fuseDuration = 2.5f;
    private bool hasLitFuse;

    public override bool CanUseAbility(EnemyAI enemy)
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        return distanceToPlayer < kamikazeAbilityData.detectRadius && IsAbilityReady();
    }

    public override void UseAbility(EnemyAI enemy)
    {
        if (!hasLitFuse)
        {
            kamikazeCoroutine = StartCoroutine(LightFuse(enemy));
        }
    }

    private IEnumerator LightFuse(EnemyAI enemy)
    {
        hasLitFuse = true;
        enemy.animator.SetTrigger("Power");
        yield return new WaitForSeconds(0.9f); // Small delay for the heal animation (adjust as needed)
        enemy.animator.ResetTrigger("Power");
        StatusEffectManager.Instance.ApplyKamikazeEffect(enemy.gameObject, fuseDuration, kamikazeAbilityData.areaOfEffect);

        DeathEffectManager.Instance.RegisterOnDeathEffect(enemy.gameObject, new KamikazeDeathEffect(kamikazeAbilityData.areaOfEffect, kamikazeAbilityData.damage, kamikazeAbilityData.knockbackForce, kamikazeAbilityData.abilityVFXPrefab));

        enemy.deathType = DeathType.Instant;

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void InterruptAbility(EnemyAI enemy)
    {
        Debug.Log("Interrupting Kamikaze Ability");

        if (kamikazeCoroutine != null)
        {
            enemy.StopCoroutine(kamikazeCoroutine);
            kamikazeCoroutine = null;
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void AbilityReset(EnemyAI enemy)
    {
        StartCooldown(enemy, kamikazeAbilityData.cooldown);
        Debug.Log("Ending.. Starting cooldown!");
        kamikazeCoroutine = null;
        hasLitFuse = false;
    }
}