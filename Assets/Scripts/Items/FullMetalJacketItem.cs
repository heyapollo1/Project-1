using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullMetalJacketItem : ItemData, ICombatEffect
{
    private float lifeThreshold = 0.9f; // 90% health threshold
    private float damageIncreasePercent = 90f; // +90% damage

    public FullMetalJacketItem(Sprite fullMetalJacketIcon)
    {
        itemName = ".308";
        description = "Deal +90% damage against enemies above 90% health.";
        icon = fullMetalJacketIcon;
        price = 50;
        itemType = ItemType.Combat;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        // No need to apply immediate stat modifiers, handled via OnHit
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        // Clean up if needed
    }

    public void OnHit(GameObject target, PlayerStatManager playerStats) { }

    public void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats) { }

    public void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats) { }

    public float ModifyDamage(float damage, GameObject target)
    {
        EnemyHealthManager enemyHealth = target.GetComponent<EnemyHealthManager>();

        if (enemyHealth != null)
        {
            // Calculate the health percentage of the enemy
            float healthPercentage = enemyHealth.currentHealth / enemyHealth.maxHealth;

            // If the enemy is above 90% health, apply the damage increase
            if (healthPercentage >= lifeThreshold)
            {
                return Mathf.Ceil(damage * (1 + damageIncreasePercent / 100f));
            }
        }

        // Return the original damage if the condition is not met
        return damage;
    }
}