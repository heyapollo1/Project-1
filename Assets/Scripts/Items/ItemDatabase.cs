using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class ItemDatabase : BaseManager
{
    public static ItemDatabase Instance { get; private set; }
    public override int Priority => 30;
    
    private static readonly Dictionary<string, Func<Sprite, Rarity, BaseItem>> itemRegistry = new Dictionary<string, Func<Sprite, Rarity, BaseItem>>();
    private static readonly Dictionary<string, Rarity> rarityRegistry = new Dictionary<string, Rarity>();
    
    protected override void OnInitialize()
    {
        LoadItemsIntoDatabase();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void LoadItemsIntoDatabase()
    {
        RegisterItem("Apple", (icon, rarity) => new AppleItem(icon, rarity), Rarity.Common);
        RegisterItem("Ghost Wind", (icon, rarity) => new GhostWindItem(icon, rarity), Rarity.Common);
        RegisterItem("Ruby", (icon, rarity) => new RubyItem(icon, rarity), Rarity.Common);
        RegisterItem("Clover", (icon, rarity) => new CloverItem(icon, rarity), Rarity.Common);
        RegisterItem("Sword", (icon, rarity) => new SwordItem(icon, rarity), Rarity.Common);
        RegisterItem("Axe", (icon, rarity) => new AxeItem(icon, rarity), Rarity.Rare);
        RegisterItem("Full Metal Jacket", (icon, rarity) => new FullMetalJacketItem(icon, rarity),Rarity.Rare);
        RegisterItem("Mad Dog", (icon, rarity) => new MadDogItem(icon, rarity), Rarity.Rare);
        RegisterItem("Witch Point", (icon, rarity) => new WitchPointItem(icon, rarity), Rarity.Rare);
        RegisterItem("Topaz", (icon, rarity) => new TopazItem(icon, rarity), Rarity.Epic);
        RegisterItem("Tooth", (icon, rarity) => new ToothItem(icon, rarity), Rarity.Epic);
        RegisterItem("Candle", (icon, rarity) => new CandleItem(icon, rarity), Rarity.Legendary);
    }
    
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
    
    public BaseItem CreateRandomItem(StageBracket bracket)
    {
        Debug.Log($"Creating random item: {itemRegistry.Keys.Count}");
        List<string> allItemNames = itemRegistry.Keys.ToList(); 
        RarityChances.GetRarityBracketChances(bracket);
        string randomItemName;
        BaseItem selectedItem;
        int attempts = 0;
        const int maxAttempts = 50;

        do
        {
            if (attempts >= maxAttempts)
            {
                Debug.LogError("Too many attempts, no viable items found.");
                return null;
            }

            attempts++;
            int randomIndex = Random.Range(0, allItemNames.Count);
            randomItemName = allItemNames[randomIndex];
            if (!ItemTracker.Instance.DoesItemExist(randomItemName))
            {
                break; // Found a valid item, exit loop
            }
        } while (true);
        
        selectedItem = CreateItem(randomItemName);

        Debug.LogWarning($"{selectedItem.itemName} gotten from the database");
        return selectedItem;
    }
    
    public static BaseItem CreateItem(string itemName)
    {
        Debug.Log($"Creating random item amount?  {itemRegistry.Count}");
        Debug.Log($"Creating random item:have?  {itemRegistry.ContainsKey(itemName)}");
        if (itemRegistry.TryGetValue(itemName, out var factoryMethod))
        {
            return factoryMethod.Invoke(LoadItemIcon(itemName), GetItemRarity(itemName));
        }
        Debug.LogError($"Item '{itemName}' not found in factory.");
        return null;
    }
    
    public static Rarity GetItemRarity(string itemName)
    {
        if (rarityRegistry.TryGetValue(itemName, out var itemRarity))
        {
            return itemRarity;
        }
        Debug.LogError($"ItemFactory: Default rarity not found for {itemName}");
        return Rarity.Common; // Fallback
    }
    
    public static Sprite LoadItemIcon(string itemName)
    {
        return Resources.Load<Sprite>($"Items/{itemName.Replace(" ", "")}Icon");
    }
}
