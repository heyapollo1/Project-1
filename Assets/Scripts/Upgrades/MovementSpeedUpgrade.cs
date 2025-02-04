using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeedUpgrade : UpgradeData
{
    public float MoveSpeedIncrease = 20f;  // Base movement speed increment

    public MovementSpeedUpgrade(Sprite movementSpeedIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Movement Speed Boost";
        description = "Increase move speed by 20%";
        icon = movementSpeedIcon;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        // Create a new modifier for attack speed
        StatModifier movementSpeedModifier = new StatModifier(
            StatType.MovementSpeed, flatBonus: 0, percentBonus: MoveSpeedIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(movementSpeedModifier);

        Debug.Log($"MovementSpeed upgraded. Current Movespeed: {playerStats.GetStatValue(StatType.MovementSpeed, 1f)}");
    }

    public override void ScaleUpgrade(PlayerStatManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);  // Re-apply the movement speed boost at the new level
        Debug.Log("Movement speed upgrade leveled up to: " + upgradeLevel);
    }
}
