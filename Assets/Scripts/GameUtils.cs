using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class ModifierUtils
    {
        public static float GetPercentBonus(List<StatModifier> modifiers, StatType stat)
        {
            return modifiers.Find(m => m.statType == stat)?.percentBonus ?? 0f;
        }

        public static float GetFlatBonus(List<StatModifier> modifiers, StatType stat)
        {
            return modifiers.Find(m => m.statType == stat)?.flatBonus ?? 0f;
        }
    }
}
