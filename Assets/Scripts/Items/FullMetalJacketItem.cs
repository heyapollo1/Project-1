using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullMetalJacketItem : BaseItem
{
    private float damageMultiplier;

    public FullMetalJacketItem(Sprite fullMetalJacketIcon, Rarity rarity) 
        : base("Full Metal Jacket", fullMetalJacketIcon, 50, ItemType.Inventory, rarity, null, null, 
            new FullMetalJacketEffect(1.0f))
    {
        damageMultiplier = 1.0f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"Deal +{damageMultiplier * 100}% bonus damage against enemies above 90% health.";
    }
}