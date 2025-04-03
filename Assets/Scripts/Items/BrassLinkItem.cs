using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class BrassLinkItem : ItemData
{
    float brassLinkArmourIncrease = 20f;

    public BrassLinkItem(Sprite brassLinkIcon)
    {
        itemName = "Brass Link";
        description = "Increase armour by 20%";
        icon = brassLinkIcon;
        price = 50;
    }

    public override void Apply(AttributeManager playerStats)
    {
        StatModifier brassLinkModifier = new StatModifier(
            StatType.Armour, flatBonus: brassLinkArmourIncrease
        );

        // Apply the modifier to player stats
        playerStats.ApplyModifier(brassLinkModifier);

        Debug.Log($"Link applied. Current Armour: {playerStats.GetStatValue(StatType.Armour, 1f)}");
    }

    public override void Remove(AttributeManager playerStats)
    {
        StatModifier brassLinkModifier = new StatModifier(
            StatType.Armour, flatBonus: 0f, percentBonus: brassLinkArmourIncrease
        );

        playerStats.RemoveModifier(brassLinkModifier);
        Debug.Log($"Link removed. Current Armour: {playerStats.GetStatValue(StatType.Armour, 1f)}");
    }
}*/

