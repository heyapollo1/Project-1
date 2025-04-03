using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StatusEffectTrigger
{
    public StatType statusType;
    public float chance;

    public StatusEffectTrigger(StatType type, float chance)
    {
        statusType = type;
        this.chance = chance;
    }

    public bool ShouldTrigger()
    {
        return chance > 0f && Random.Range(0f, 100f) <= chance;
    }
}
