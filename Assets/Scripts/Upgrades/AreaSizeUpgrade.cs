using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSizeUpgrade : UpgradeData
{
    float areaSizeIncrease = 20f;

    public AreaSizeUpgrade(Sprite areaSizeIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Big Boom";
        description = "Increase size of explosions by 20%";
        icon = areaSizeIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier areaSizeModifier = new StatModifier(
            StatType.AreaSize, flatBonus: 0f, percentBonus: areaSizeIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(areaSizeModifier);

        Debug.Log($"AreaSize upgraded. Current AreaSize: {playerStats.GetStatValue(StatType.AreaSize, 1f)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the damage boost at the new level
        Debug.Log("AreaSize upgrade leveled up to: " + upgradeLevel);
    }
}

