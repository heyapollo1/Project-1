/*using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemFactory
{
    private static readonly Dictionary<string, Func<Sprite, Rarity, BaseItem>> itemRegistry =
        new Dictionary<string, Func<Sprite, Rarity, BaseItem>>();
    private static readonly Dictionary<string, Rarity> rarityRegistry =
        new Dictionary<string, Rarity>();
    
    public static void RegisterItem(string itemName, Func<Sprite, Rarity, BaseItem> factoryMethod, Rarity rarity)
    {
        if (!itemRegistry.ContainsKey(itemName))
        {
            itemRegistry[itemName] = factoryMethod;
            rarityRegistry[itemName] = rarity;
            Debug.LogWarning($"Item '{itemName}' registered in the factory.");
        }
        else
        {
            Debug.LogWarning($"Item '{itemName}' is already registered in the factory.");
        }
    }
    
    public static BaseItem CreateItem(string itemName)
    {
        if (itemRegistry.TryGetValue(itemName, out var factoryMethod))
        {
            return factoryMethod.Invoke(LoadItemIcon(itemName), GetItemRarity(itemName));
        }
        Debug.LogError($"Item '{itemName}' not found in factory.");
        return null;
    }
    
    public static void ClearFactory() // Reset
    {
        itemRegistry.Clear();
        Debug.Log("ItemFactory cleared.");
    }
    
    public static List<string> GetAllRegisteredItems()
    {
        return itemRegistry.Keys.ToList(); 
    }
    
    public static Sprite LoadItemIcon(string itemName)
    {
        return Resources.Load<Sprite>($"Items/{itemName.Replace(" ", "")}Icon");
    }

}*/
