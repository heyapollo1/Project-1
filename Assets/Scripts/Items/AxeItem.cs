using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeItem : BaseItem
{
    private float criticalDamageBonus;
    
    public AxeItem(Sprite axeIcon, Rarity rarity) : base(
        "Axe", axeIcon, 50, ItemType.Inventory, rarity,
        new List<StatModifier> {new (StatType.CriticalHitDamage, 20f)})
    {
        criticalDamageBonus = 20f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"Increase Critical hit damage by {criticalDamageBonus}%.";
    }
}
