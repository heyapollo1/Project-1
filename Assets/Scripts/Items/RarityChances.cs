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
                { Rarity.Common, 0.80f },
                { Rarity.Rare, 0.15f },
                { Rarity.Epic, 0.05f },
                { Rarity.Legendary, 0f }
            },
            StageBracket.B => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0.50f },
                { Rarity.Rare, 0.30f },
                { Rarity.Epic, 0.15f },
                { Rarity.Legendary, 0.05f }
            },
            StageBracket.C => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0f },
                { Rarity.Rare, 0.30f },
                { Rarity.Epic, 0.50f },
                { Rarity.Legendary, 0.20f }
            },
            StageBracket.D => new Dictionary<Rarity, float>
            {
                { Rarity.Common, 0f },
                { Rarity.Rare, 0f },
                { Rarity.Epic, 0.40f },
                { Rarity.Legendary, 0.60f }
            },
            _ => throw new ArgumentOutOfRangeException(nameof(bracket), "Invalid bracket.")
        };
    }
}
