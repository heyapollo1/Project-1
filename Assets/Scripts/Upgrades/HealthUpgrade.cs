using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : UpgradeData
{
    public float baseHealthIncrease = 15f;  // Base health increase

    public HealthUpgrade(Sprite healthIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Max Health Boost";
        description = "Increase max health by 50";
        icon = healthIcon;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier maxHealthModifier = new StatModifier(
            StatType.MaxHealth, flatBonus: 50
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(maxHealthModifier);

        Debug.Log($"MovementSpeed upgraded. Current Health: {playerStats.GetStatValue(StatType.MovementSpeed, 1f)}");
    }

    public override void ScaleUpgrade(PlayerStatManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the health boost at the new level
        Debug.Log("Health upgrade leveled up to: " + upgradeLevel);
    }
}
