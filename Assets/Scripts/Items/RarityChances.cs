using System;
using System.Collections.Generic;
using UnityEngine;

public static class RarityChances
{
    public static Dictionary<Rarity, float> GetRarityBracketChances(StageBracket bracket)
    {
        return bracket switch
        {
            StageBracket.A => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 1.0f },
                { Rarity.Rare, 0.0f },
                { Rarity.Epic, 0.0f },
                { Rarity.Legendary, 0f }
            },
            StageBracket.B => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0.90f },
                { Rarity.Rare, 0.10f },
                { Rarity.Epic, 0.0f },
                { Rarity.Legendary, 0.0f }
            },
            StageBracket.C => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0.75f },
                { Rarity.Rare, 0.20f },
                { Rarity.Epic, 0.05f },
                { Rarity.Legendary, 0.0f }
            },
            StageBracket.D => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0.60f },
                { Rarity.Rare, 0.30f },
                { Rarity.Epic, 0.10f },
                { Rarity.Legendary, 0.05f }
            },
            StageBracket.E => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0.30f },
                { Rarity.Rare, 0.40f },
                { Rarity.Epic, 0.20f },
                { Rarity.Legendary, 0.10f }
            },
            StageBracket.F => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0.05f },
                { Rarity.Rare, 0.50f },
                { Rarity.Epic, 0.30f },
                { Rarity.Legendary, 0.15f }
            },
            _ => throw new ArgumentOutOfRangeException(nameof(bracket), "Invalid bracket.")
        };
    }
}
