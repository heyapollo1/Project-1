using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloverItem : BaseItem
{

    private float critChanceBonus;
    
    public CloverItem(Sprite cloverIcon, Rarity rarity) : base(
        "Clover", cloverIcon, 50, ItemType.Inventory, rarity,
        new List<StatModifier> {new(StatType.CriticalHitChance, 15f)})
    {
        critChanceBonus = 15f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"Increase critical hit chance by {critChanceBonus}%.";
    }
}
