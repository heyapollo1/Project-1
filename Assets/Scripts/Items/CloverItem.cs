using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloverItem : ItemData
{
    float criticalStrikeDamageIncrease = 50f;

    public CloverItem(Sprite cloverIcon)
    {
        itemName = "Clover";
        description = "Critical strikes deal 50% increased damage";
        icon = cloverIcon;
        price = 50;
        itemType = ItemType.Simple;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        StatModifier cloverModifier = new StatModifier(
             StatType.CriticalHitDamage, flatBonus: 0f, percentBonus: criticalStrikeDamageIncrease
         );

        playerStats.ApplyModifier(cloverModifier);
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        StatModifier cloverModifier = new StatModifier(
            StatType.CriticalHitDamage, flatBonus: 0f, percentBonus: criticalStrikeDamageIncrease
        );

        playerStats.RemoveModifier(cloverModifier);
    }
}
