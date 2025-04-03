/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadDogEffect : ICombatEffect
{
    private readonly float bleedChance;

    public MadDogEffect(float chance)
    {
        bleedChance = chance;
        StatModifier madDogBleedChanceModifier = new StatModifier(
            StatType.BleedChance, flatBonus: bleedChance);

        AttributeManager.Instance.ApplyModifier(madDogBleedChanceModifier);
        Debug.Log($"bleed upgraded. Current bleedchance: {AttributeManager.Instance.GetStatValue(StatType.BleedChance, 1f)}");
    }
    
    public void OnHit(GameObject target, float damageDealt)
    {
        if (AttributeManager.Instance.AttemptToApplyStatusEffect(StatType.BleedChance))
        {
            StatusEffectManager.Instance.ApplyBleedEffect(target, 5f, damageDealt / 2);
            Debug.Log($"Applying burn effect to {target.name}, for {damageDealt/2} damage per tick for 5 seconds");
        }
    }

    public void OnHurt(float currentHealth, float maxHealth) {}
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
    public float ModifyDamage(float damage, GameObject target) => damage;
}*/