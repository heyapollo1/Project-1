using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToothItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    private float damageBonus;

    public ToothItem(Sprite toothIcon, Rarity rarity) 
        : base("Tooth", toothIcon, 50, ItemType.Inventory, rarity, null, ClassTags,
            new ToothEffect(0.2f))
    {
        damageBonus = 0.2f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"Increase damage against Burning enemies by {damageBonus * 100}%.";
    }
}