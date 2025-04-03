using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToothEffect : ICombatEffect
{
    private readonly float toothDamageIncrease;

    public ToothEffect(float damageBonus)
    {
        toothDamageIncrease = damageBonus;
    }
    
    public void OnHit(CombatContext context){}
    public void OnHurt(float currentHealth, float maxHealth) {}
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
    public List<StatModifier> ModifyStats(CombatContext context)
    {
        var modifiers = new List<StatModifier>();
        if (context.target.TryGetComponent(out EnemyHealthManager enemyHealth) &&
            enemyHealth.HasStatusEffect<BleedingEffect>())
        {
            modifiers.Add(new StatModifier(StatType.Damage, 0f, toothDamageIncrease));
        }
        return modifiers;
    }
}
