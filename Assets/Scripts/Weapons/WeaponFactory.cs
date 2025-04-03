using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WeaponFactory
{
    private static readonly Dictionary<string, Func<Sprite, Rarity, WeaponBase>> weaponRegistry =
        new Dictionary<string, Func<Sprite, Rarity, WeaponBase>>();
    
    private static readonly Dictionary<string, Rarity> defaultRarityRegistry =
        new Dictionary<string, Rarity>();
    
    public static void RegisterWeapon(string weaponName, Func<Sprite, Rarity, WeaponBase> factoryMethod, Rarity defaultRarity)
    {
        if (!weaponRegistry.ContainsKey(weaponName))
        {
            weaponRegistry[weaponName] = factoryMethod;
            defaultRarityRegistry[weaponName] = defaultRarity; // Store default rarity separately
            Debug.LogWarning($"Weapon '{weaponName}' registered in the factory with default rarity {defaultRarity}.");
        }
        else
        {
            Debug.LogWarning($"Item '{weaponName}' is already registered in the factory.");
        }
    }

    // Create weapon using its name
    public static WeaponBase CreateWeapon(string weaponName, Sprite icon, Rarity rarity)
    {
        if (weaponRegistry.TryGetValue(weaponName, out var factoryMethod))
        {
            return factoryMethod.Invoke(icon, rarity);
        }

        Debug.LogError($"Weapon '{weaponName}' not found in factory.");
        return null;
    }
    
    public static void ClearFactory() // 🔥 Reset the factory
    {
        weaponRegistry.Clear();
        Debug.Log("WeaponFactory cleared.");
    }
    
    public static Rarity GetDefaultRarity(string weaponName)
    {
        if (defaultRarityRegistry.TryGetValue(weaponName, out var defaultRarity))
        {
            return defaultRarity;
        }
        Debug.LogError($"ItemFactory: Default rarity not found for {weaponName}");
        return Rarity.Common; // Fallback
    }
    
    public static List<string> GetAllRegisteredWeapons()
    {
        return weaponRegistry.Keys.ToList(); 
    }
}
