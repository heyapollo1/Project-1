using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleItem : ItemData
{
    float appleHealthIncrease = 20f;

    public AppleItem(Sprite appleIcon)
    {
        itemName = "Apple";
        description = "Increase maximum hp by 20%";
        icon = appleIcon;
        price = 50;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        StatModifier appleModifier = new StatModifier(
            StatType.MaxHealth, flatBonus: 0f, percentBonus: appleHealthIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(appleModifier);

        Debug.Log($"Apple applied. Current maxHealth: {playerStats.GetStatValue(StatType.MaxHealth, 1f)}");
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        StatModifier appleModifier = new StatModifier(
            StatType.MaxHealth, flatBonus: 0f, percentBonus: appleHealthIncrease
        );

        playerStats.RemoveModifier(appleModifier);
        Debug.Log($"Apple removed. Current Max Health: {playerStats.GetStatValue(StatType.MaxHealth, 1f)}");
    }
}
