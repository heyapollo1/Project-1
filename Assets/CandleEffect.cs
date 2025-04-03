/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleEffect : ICombatEffect
{
    private readonly float burnChance;

    public CandleEffect(float chance)
    {
        burnChance = chance;
        StatModifier candleModifier = new StatModifier(StatType.BurnChance, flatBonus: burnChance);
        AttributeManager.Instance.ApplyModifier(candleModifier); //Update burn zchance to attribute Manager
    }
    
    public void OnHit(GameObject target, float damageDealt)
    {
        Debug.Log($"Applying burn effect to {target.name}, for {damageDealt/2} damage per tick for 5 seconds");
       if (AttributeManager.Instance.AttemptToApplyStatusEffect(StatType.BurnChance))
       {
           Debug.Log($"Applying burn effect to {target.name}, for {damageDealt/2} damage per tick for 5 seconds");
           StatusEffectManager.Instance.ApplyBurnEffect(target, 5f, damageDealt / 2);
           Debug.Log($"Applying burn effect to {target.name}, for {damageDealt/2} damage per tick for 5 seconds");
       }
    }

    public void OnHurt(float currentHealth, float maxHealth) {}
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
    public float ModifyDamage(float damage, GameObject target) => damage;
}*/
