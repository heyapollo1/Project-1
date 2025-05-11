using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorchUpgrade : UpgradeData
{
    public float scorchWidthIncrease = 0.5f;  // Base health increase

    public ScorchUpgrade(Sprite scorchIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Scorch";
        description = "Increase the width of Fire Breath's flame by 20%.";
        icon = scorchIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier scorchModifier = new StatModifier(
            StatType.BurnChance, flatBonus: 10, percentBonus: scorchWidthIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(scorchModifier);

        Debug.Log($"Scorch upgraded. Current width: {playerStats.GetStatValue(StatType.BurnChance)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the health boost at the new level
        Debug.Log("Health upgrade leveled up to: " + upgradeLevel);
    }
}