using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : UpgradeData
{
    public HealthUpgrade(Sprite healthIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Max Health Boost";
        description = "Increase max health by 50";
        icon = healthIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier maxHealthModifier = new StatModifier(
            StatType.MaxHealth, flatBonus: 50
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(maxHealthModifier);

        Debug.Log($"Health upgraded. Current Health: {playerStats.GetStatValue(StatType.MaxHealth, AttributeManager.Instance.baseMaxHealth)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the health boost at the new level
        Debug.Log("Health upgrade leveled up to: " + upgradeLevel);
    }
}
