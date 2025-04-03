using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHitDamageUpgrade : UpgradeData
{
    public float CriticalHitDamageIncrease = 25f;  // Base movement speed increment

    public CriticalHitDamageUpgrade(Sprite criticalHitDamageIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Rambo";
        description = "Increase critical hit damage by 25%";
        icon = criticalHitDamageIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier criticalHitDamageModifier = new StatModifier(
            StatType.CriticalHitDamage, flatBonus: CriticalHitDamageIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(criticalHitDamageModifier);

        Debug.Log($"CriticalHitChance upgraded. Current CriticalHitDamage: {playerStats.GetStatValue(StatType.CriticalHitDamage, 0f)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the movement speed boost at the new level
        Debug.Log("CC upgrade leveled up to: " + upgradeLevel);
    }
}
