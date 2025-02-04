using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWindItem : ItemData
{
    float ghostWindMovementSpeedIncrease = 30f;

    public GhostWindItem(Sprite ghostWindIcon)
    {
        itemName = "Ghost Wind";
        description = "Increase movement speed by 30%.";
        icon = ghostWindIcon;
        price = 50;
        itemType = ItemType.Simple;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        StatModifier ghostWindMovementSpeedModifier = new StatModifier(
             StatType.MovementSpeed, flatBonus: 0f, percentBonus: ghostWindMovementSpeedIncrease);


        playerStats.ApplyModifier(ghostWindMovementSpeedModifier);
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        StatModifier ghostWindMovementSpeedModifier = new StatModifier(
           StatType.MovementSpeed, flatBonus: 0f, percentBonus: ghostWindMovementSpeedIncrease);

        playerStats.RemoveModifier(ghostWindMovementSpeedModifier);
    }
}