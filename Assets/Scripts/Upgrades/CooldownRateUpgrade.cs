using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownRateUpgrade : UpgradeData
{
    public float percentIncrease = 20f;  // Percentage attack speed increase

    public CooldownRateUpgrade(Sprite cooldownRateIcon)
    {
        // Set default values
        upgradeName = "Volley";
        description = "Abilities recharge 20% faster";
        icon = cooldownRateIcon;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier cooldownRateModifier = new StatModifier(
            StatType.CooldownRate, flatBonus: 0f,percentBonus: percentIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(cooldownRateModifier);

        Debug.Log($"CooldownReduction upgraded. Current CooldownReduction: {playerStats.GetStatValue(StatType.CooldownRate, 1f)}");
    }

    public override void ScaleUpgrade(PlayerStatManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the attack speed boost at the new level
        Debug.Log("Volley upgrade leveled up to: " + upgradeLevel);
    }
}