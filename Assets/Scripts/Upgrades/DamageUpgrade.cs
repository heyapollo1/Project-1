using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpgrade : UpgradeData
{
    float damageIncrease = 2f;

    public DamageUpgrade(Sprite damageIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Smokin Barrel";
        description = "Increase damage by 2";
        icon = damageIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier damageModifier = new StatModifier(
            StatType.Damage, flatBonus: damageIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(damageModifier);

        Debug.Log($"CriticalHitChance upgraded. Current CriticalHitChance: {playerStats.GetStatValue(StatType.Damage)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the damage boost at the new level
        Debug.Log("Damage upgrade leveled up to: " + upgradeLevel);
    }
}
