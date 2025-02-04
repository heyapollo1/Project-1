using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAbility : EnemyAbilityBase

{
    [SerializeField]
    public EnemyAbilityData healAbilityData;
    private Coroutine healCoroutine;

    private bool hasHealed;

    public override bool CanUseAbility(EnemyAI enemy)
    {
        // Check if there are allies within range and if the ability is ready
        return EnemyManager.Instance.HasAlliesInRange(enemy.transform.position, healAbilityData.detectRadius, enemy.gameObject) && IsAbilityReady();
    }

    public override void UseAbility(EnemyAI enemy)
    {
        if (healCoroutine == null && !hasHealed)
        {
            enemy.rb.velocity = Vector2.zero;
            healCoroutine = StartCoroutine(HealNearbyAllies(enemy));
        }
    }

    private IEnumerator HealNearbyAllies(EnemyAI enemy)
    {
        hasHealed = true;
        enemy.animator.SetTrigger("Power");
        yield return new WaitForSeconds(0.9f); // Small delay for the heal animation (adjust as needed)

        // Get list of nearby allies to heal
        List<GameObject> alliesToHeal = EnemyManager.Instance.GetEnemiesInRange(enemy.transform.position, healAbilityData.detectRadius, enemy.gameObject);

        // Apply healing to each ally
        foreach (var ally in alliesToHeal)
        {
            EnemyHealthManager allyAI = ally.GetComponent<EnemyHealthManager>();
            if (allyAI != null && allyAI.currentHealth < allyAI.maxHealth)
            {
                GameObject healingFX = FXManager.Instance.PlayFX("HealingFX", ally.transform.position);
                healingFX.transform.SetParent(ally.transform);
                healingFX.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                allyAI.Heal(healAbilityData.damage);
            }
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void InterruptAbility(EnemyAI enemy)
    {
        Debug.Log("Interrupting Heal Ability");

        if (healCoroutine != null)
        {
            enemy.StopCoroutine(healCoroutine);
            healCoroutine = null;
        }

        AbilityReset(enemy);
        CompleteAbility();
    }

    public override void AbilityReset(EnemyAI enemy)
    {
        enemy.animator.ResetTrigger("Power");
        StartCooldown(enemy, healAbilityData.cooldown);  // Start cooldown
        healCoroutine = null;
        hasHealed = false;
    }
}
