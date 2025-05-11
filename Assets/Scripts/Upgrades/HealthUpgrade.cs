using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : UpgradeData
{
    public HealthUpgrade(Sprite healthIcon)
    {
        upgradeName = "Max Health Boost";
        description = "Increase max health by 50";
        icon = healthIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        StatModifier maxHealthModifier = new StatModifier(
            StatType.MaxHealth, flatBonus: 50
        );

        playerStats.ApplyModifier(maxHealthModifier);
        Debug.Log($"Health upgraded. Current Health: {playerStats.GetStatValue(StatType.MaxHealth)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);
        Debug.Log("Health upgrade leveled up to: " + upgradeLevel);
    }
}
