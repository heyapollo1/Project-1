using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWindItem : BaseItem
{

    private float moveSpeedBonus;
    
    public GhostWindItem(Sprite ghostWindIcon, Rarity rarity) : base(
        "Ghost Wind", ghostWindIcon, 50, ItemType.Inventory, rarity, 
        new List<StatModifier> { new (StatType.MovementSpeed, 0f, 20f)})
    {
        moveSpeedBonus = 20f;
        UpdateDescription();
    }

    public override string UpdateDescription()
    {
        return description = $"Increase movement speed by {moveSpeedBonus}%.";
    }
}