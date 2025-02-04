using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleItem : ItemData
{
    private float candleBurnChance = 50f;

    public CandleItem(Sprite candleItem)
    {
        itemName = "Candle";
        description = "Attacks have a 50% chance to inflict Burn";
        icon = candleItem;
        price = 50;
        itemType = ItemType.Simple;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        StatModifier candleModifier = new StatModifier(
             StatType.BurnChance, flatBonus: candleBurnChance);

        playerStats.ApplyModifier(candleModifier);
        Debug.Log($"burn upgraded. Current burnChance: {playerStats.GetStatValue(StatType.BurnChance, 0f)}");
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        StatModifier candleModifier = new StatModifier(
            StatType.BurnChance, flatBonus: candleBurnChance);

        playerStats.RemoveModifier(candleModifier);
    }
}
