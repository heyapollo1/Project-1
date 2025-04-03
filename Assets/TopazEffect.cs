using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class TopazEffect : ICombatEffect
{
    private readonly float stunChance;

    public TopazEffect(float chance)
    {
        stunChance = chance;
        StatModifier topazModifier = new StatModifier(
            StatType.StunChance, flatBonus: stunChance);
        AttributeManager.Instance.ApplyModifier(topazModifier);
    }
    
    public void OnHit(GameObject target, float damage)
    {
        if (AttributeManager.Instance.AttemptToApplyStatusEffect(StatType.StunChance))
        {
            StatusEffectManager.Instance.ApplyStunEffect(target, 2f);
            Debug.Log($"Applying stun effect to {target.name}");
        }
        Debug.Log($"stun upgraded. Current stunChance: {AttributeManager.Instance.GetStatValue(StatType.StunChance, 0f)}");
    }

    public void OnHurt(float currentHealth, float maxHealth) {}
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
    public float ModifyDamage(float damage, GameObject target) => damage;
}*/
