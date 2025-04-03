using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullMetalJacketEffect : ICombatEffect
{
    private readonly float damageMultiplier;

    public FullMetalJacketEffect(float multiplier)
    {
        damageMultiplier = multiplier;
    }

    public void OnHit(CombatContext context){}
    public void OnHurt(float currentHealth, float maxHealth) {}
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
    public List<StatModifier> ModifyStats(CombatContext context)
    {
        var modifiers = new List<StatModifier>();
        if (context.target.TryGetComponent(out EnemyHealthManager enemyHealth) &&
            enemyHealth.GetHealthPercentage() >= 0.9f)
        {
            modifiers.Add(new StatModifier(StatType.Damage, 0f, damageMultiplier));
        }
        return modifiers;
    }
}
