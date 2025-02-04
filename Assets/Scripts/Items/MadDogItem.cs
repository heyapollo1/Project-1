using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadDogItem : ItemData
{
    private float madDogBleedChance = 100f;

    public MadDogItem(Sprite madDogItem)
    {
        itemName = "Mad Dog";
        description = "Attacks have a 30% chance to inflict Bleed";
        icon = madDogItem;
        price = 50;
        itemType = ItemType.Simple;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        StatModifier madDogBleedChanceModifier = new StatModifier(
             StatType.BleedChance, flatBonus: madDogBleedChance);

        playerStats.ApplyModifier(madDogBleedChanceModifier);
        Debug.Log($"bleed upgraded. Current bleedchance: {playerStats.GetStatValue(StatType.BleedChance, 1f)}");
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        StatModifier madDogBleedChanceModifier = new StatModifier(
            StatType.BleedChance, flatBonus: madDogBleedChance);

        playerStats.RemoveModifier(madDogBleedChanceModifier);
    }
}