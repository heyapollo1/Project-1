using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemFactory
{
    private static readonly Dictionary<string, Func<Sprite, Rarity, BaseItem>> itemRegistry =
        new Dictionary<string, Func<Sprite, Rarity, BaseItem>>();
    
    private static readonly Dictionary<string, Rarity> defaultRarityRegistry =
        new Dictionary<string, Rarity>();
    
    public static void RegisterItem(string itemName, Func<Sprite, Rarity, BaseItem> factoryMethod, Rarity defaultRarity)
    {
        if (!itemRegistry.ContainsKey(itemName))
        {
            itemRegistry[itemName] = factoryMethod;
            defaultRarityRegistry[itemName] = defaultRarity; // Store default rarity separately
            Debug.LogWarning($"Item '{itemName}' registered in the factory with default rarity {defaultRarity}.");
        }
        else
        {
            Debug.LogWarning($"Item '{itemName}' is already registered in the factory.");
        }
    }

    // Create an item using its name
    public static BaseItem CreateItem(string itemName, Sprite icon, Rarity rarity)
    {
        if (itemRegistry.TryGetValue(itemName, out var factoryMethod))
        {
            return factoryMethod.Invoke(icon, rarity);
        }

        Debug.LogError($"Item '{itemName}' not found in factory.");
        return null;
    }
    
    public static void ClearFactory() // 🔥 Reset the factory
    {
        itemRegistry.Clear();
        Debug.Log("ItemFactory cleared.");
    }
    
    public static Rarity GetDefaultRarity(string itemName)
    {
        if (defaultRarityRegistry.TryGetValue(itemName, out var defaultRarity))
        {
            return defaultRarity;
        }
        Debug.LogError($"ItemFactory: Default rarity not found for {itemName}");
        return Rarity.Common; // Fallback
    }
    
    public static List<string> GetAllRegisteredItems()
    {
        return itemRegistry.Keys.ToList(); 
    }
}
