using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCollection
{
    public Dictionary<StatType, float> flatBonuses = new();
    public Dictionary<StatType, float> percentBonuses = new();
    
    public float Get(StatType type, float baseValue)
    {
        float flat = flatBonuses.GetValueOrDefault(type, 0f);
        float percent = percentBonuses.GetValueOrDefault(type, 0f);
        return (baseValue + flat) * (1 + percent / 100f);
    }

    public void Apply(StatModifier mod)
    {
        if (mod == null || !System.Enum.IsDefined(typeof(StatType), mod.statType))
        {
            Debug.LogWarning("StatModifier is null or has undefined StatType.");
            return;
        }
        Debug.Log($"Mod stat Type: {mod.statType}");
        flatBonuses[mod.statType] += mod.flatBonus;
        percentBonuses[mod.statType] += mod.percentBonus;
    }

    public void Remove(StatModifier mod)
    {
        flatBonuses[mod.statType] -= mod.flatBonus;
        percentBonuses[mod.statType] -= mod.percentBonus;
    }

    public void Reset()
    {
        flatBonuses.Clear();
        percentBonuses.Clear();
    }
}