using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier
{
    public StatType statType;   // The type of stat this modifier affects
    public float flatBonus;     // Flat bonus (e.g., +10 damage)
    public float percentBonus;  // Percentage bonus (e.g., +20% attack speed)
    public float duration;      // Optional duration (for temporary buffs)

    public StatModifier(StatType statType, float flatBonus = 0f, float percentBonus = 0f, float duration = 0f)
    {
        this.statType = statType;
        this.flatBonus = flatBonus;
        this.percentBonus = percentBonus;
        this.duration = duration;
    }
}
