using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownRateUpgrade : UpgradeData
{
    public float percentIncrease = 20f;

    public CooldownRateUpgrade(Sprite cooldownRateIcon)
    {
        upgradeName = "Volley";
        description = "Abilities recharge 20% faster";
        icon = cooldownRateIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier cooldownRateModifier = new StatModifier(
            StatType.CooldownRate, flatBonus: 0f,percentBonus: percentIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(cooldownRateModifier);

        Debug.Log($"CooldownReduction upgraded. Current CooldownReduction: {playerStats.GetStatValue(StatType.CooldownRate)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the attack speed boost at the new level
        Debug.Log("Volley upgrade leveled up to: " + upgradeLevel);
    }
}