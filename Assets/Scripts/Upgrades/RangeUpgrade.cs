using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUpgrade : UpgradeData
{
    public float RangeIncrease = 15f;  // Base attack speed increment

    public RangeUpgrade(Sprite rangeIcon)
    {
        // Set default values
        upgradeName = "Seeker";
        description = "Increase range by 15%";
        icon = rangeIcon;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier rangeModifier = new StatModifier(
            StatType.Range, flatBonus: 0f, percentBonus: RangeIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(rangeModifier);

        Debug.Log($"Attack Speed upgraded. Current Attack Speed: {playerStats.GetStatValue(StatType.Range, 1f)}");
    }

    public override void ScaleUpgrade(PlayerStatManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the attack speed boost at the new level
        Debug.Log("Attack speed upgrade leveled up to: " + upgradeLevel);
    }
}
