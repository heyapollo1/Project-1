using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Food
    };
    
    private float healthBonus;
    
    public AppleItem(Sprite appleIcon, Rarity rarity) : base(
        "Apple", appleIcon, 50, ItemType.Inventory, rarity,
        new List<StatModifier> { new (StatType.MaxHealth, 50f)}, ClassTags)
    {
        healthBonus = 50f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"Increase max HP by {healthBonus}.";
    }
}
