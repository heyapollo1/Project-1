using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourUpgrade : UpgradeData
{
    float armourIncrease = 10f;

    public ArmourUpgrade(Sprite armourIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Iron Skin";
        description = "Increase Armour by 10";
        icon = armourIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier armourModifier = new StatModifier(
            StatType.Armour, flatBonus: armourIncrease
        );

        playerStats.ApplyModifier(armourModifier);

        Debug.Log($"AreaSize upgraded. Current AreaSize: {playerStats.GetStatValue(StatType.AreaSize)}");
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats); 
        Debug.Log("AreaSize upgrade leveled up to: " + upgradeLevel);
    }
}
