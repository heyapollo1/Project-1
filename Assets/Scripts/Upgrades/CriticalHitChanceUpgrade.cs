using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHitChanceUpgrade : UpgradeData
{
     public float CriticalHitChanceIncrease = 10f;  // Base movement speed increment

    public CriticalHitChanceUpgrade(Sprite criticalHitChanceIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Brain Destroyer";
        description = "Increase critical hit chance by 10%";
        icon = criticalHitChanceIcon;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier criticalHitChanceModifier = new StatModifier(
            StatType.CriticalHitChance, flatBonus: CriticalHitChanceIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(criticalHitChanceModifier);

        Debug.Log($"CriticalHitChance upgraded. Current CriticalHitChance: {playerStats.GetStatValue(StatType.CriticalHitChance, 0f)}");
    }

    public override void ScaleUpgrade(PlayerStatManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the movement speed boost at the new level
        Debug.Log("CC upgrade leveled up to: " + upgradeLevel);
    }
}
