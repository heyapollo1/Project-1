using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyItemEffect : ICombatEffect
{
    private readonly float damageMultiplier;
    private bool isBuffActive = false;
    
    public RubyItemEffect(float damageMultiplier)
    {
        this.damageMultiplier = damageMultiplier;
    }

    public void OnHit(CombatContext context){}
    public void OnHurt(float currentHealth, float maxHealth)
    {
        float currentHealthPercentage = PlayerHealthManager.Instance.GetHealthPercentage();
        if (currentHealthPercentage <= 0.5f && !isBuffActive)
        {
            Debug.Log("Applying Ruby buff...");
            isBuffActive = true;
            AttributeManager.Instance.ApplyModifier(new StatModifier(StatType.Damage, damageMultiplier));
        }
        else if (currentHealth / maxHealth > 0.5 && isBuffActive)
        {
            Debug.Log("Removing Ruby buff...");
            AttributeManager.Instance.RemoveModifier(new StatModifier(StatType.Damage, damageMultiplier));
            isBuffActive = false;
        }
    }
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
}
