using System.Collections.Generic;
using UnityEngine;

public static class TagDatabase
{
    public static Dictionary<TagType, string> ClassTags = new Dictionary<TagType, string>
    {
        { TagType.Weapon, "Weapon" },
        { TagType.Equipment, "Equipment" },
        { TagType.Food, "Food" },
        { TagType.Junk, "Junk" },
        { TagType.Tool, "Tool" },
    };
}

public enum TagType
{
    Weapon,
    Equipment,
    Food,
    Junk,
    Tool,
}

