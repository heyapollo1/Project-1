using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstFireUpgrade : UpgradeData
{
    public float burstFireProjectileIncrease = 1f;  // Base movement speed increment

    public BurstFireUpgrade(Sprite burstFireIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Burst Fire";
        description = "Increase the number of shots fired in a burst.";
        icon = burstFireIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier burstFireModifier = new StatModifier(
            StatType.BurstFire, flatBonus: burstFireProjectileIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(burstFireModifier);

        Debug.Log($"CriticalHitChance upgraded. BurstFire: {playerStats.GetStatValue(StatType.BurstFire, 1f)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the movement speed boost at the new level
        Debug.Log("CC upgrade leveled up to: " + upgradeLevel);
    }
}
