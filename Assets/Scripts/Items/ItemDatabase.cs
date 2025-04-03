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

    public List<BaseItem> availableItems = new List<BaseItem>();
    public HashSet<string> activeItems = new HashSet<string>();
    
    protected override void OnInitialize()
    {
        RegisterItems();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        ResetItems();
    }

    private void RegisterItems()
    {
        ItemFactory.RegisterItem("Apple", (icon, rarity) => new AppleItem(icon, rarity), Rarity.Common);
        ItemFactory.RegisterItem("Witch Point", (icon, rarity) => new WitchPointItem(icon, rarity), Rarity.Epic);
        ItemFactory.RegisterItem("Ghost Wind", (icon, rarity) => new GhostWindItem(icon, rarity), Rarity.Common);
        ItemFactory.RegisterItem("Axe", (icon, rarity) => new AxeItem(icon, rarity), Rarity.Common);
        ItemFactory.RegisterItem("Ruby", (icon, rarity) => new RubyItem(icon, rarity), Rarity.Common);
        ItemFactory.RegisterItem("Clover", (icon, rarity) => new CloverItem(icon, rarity), Rarity.Common);
        ItemFactory.RegisterItem("Mad Dog", (icon, rarity) => new MadDogItem(icon, rarity), Rarity.Rare);
        ItemFactory.RegisterItem("Tooth", (icon, rarity) => new ToothItem(icon, rarity), Rarity.Epic);
        ItemFactory.RegisterItem("Sword", (icon, rarity) => new SwordItem(icon, rarity), Rarity.Common);
        ItemFactory.RegisterItem("Full Metal Jacket", (icon, rarity) => new FullMetalJacketItem(icon, rarity), Rarity.Common);
        ItemFactory.RegisterItem("Topaz", (icon, rarity) => new TopazItem(icon, rarity), Rarity.Epic);
        ItemFactory.RegisterItem("Candle", (icon, rarity) => new CandleItem(icon, rarity), Rarity.Rare);
    }

    public BaseItem GetRandomItem(StageBracket bracket)
    {
        Rarity rolledRarity = RollRarity(bracket);
        Rarity itemDefaultRarity;
        List<string> allItemNames = ItemFactory.GetAllRegisteredItems();

        string randomItemName;
        BaseItem selectedItem = null;
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
            randomItemName = allItemNames[Random.Range(0, allItemNames.Count)];
            itemDefaultRarity = ItemFactory.GetDefaultRarity(randomItemName);
            Debug.Log($"Attempt {attempts}: Trying item {randomItemName} (Default Rarity: {itemDefaultRarity})");

            if (!activeItems.Contains(randomItemName) && IsItemAllowed(randomItemName, itemDefaultRarity, rolledRarity))
            {
                break; // Found a valid item, exit loop
            }
        }
        while (true);
        Sprite itemIcon = LoadItemIcon(randomItemName);
        if (InventoryManager.Instance.HasItem(randomItemName))
        {
            Rarity? playerRarity = InventoryManager.Instance.GetOwnedItemRarity(randomItemName);
            rolledRarity = playerRarity.Value;
            selectedItem = ItemFactory.CreateItem(randomItemName, itemIcon, rolledRarity);
        }
        else
        {
            Debug.LogWarning($"{randomItemName}, rarity: ({itemDefaultRarity}) is not owned by player");
            selectedItem = ItemFactory.CreateItem(randomItemName, itemIcon, rolledRarity);
        }
        
        Debug.LogWarning($"{selectedItem.itemName} gotten from the database");
        return selectedItem;
    }
    
    public Sprite LoadItemIcon(string itemName)
    {
        return Resources.Load<Sprite>($"Items/{itemName.Replace(" ", "")}Icon");
    }
    
    private Rarity RollRarity(StageBracket bracket)
    {
        Debug.LogWarning("RollingRarity");
        Dictionary<Rarity, float> chances = RarityChances.GetRarityBracketChances(bracket);
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var rarityChance in chances)
        {
            cumulativeProbability += rarityChance.Value;
            if (randomValue <= cumulativeProbability)
            {
                return rarityChance.Key;
            }
        }
        Debug.LogWarning("Rarity roll failed, defaulting to Bronze.");
        return Rarity.Common; // Fallback rarity
    }
    
    private bool IsItemAllowed(string itemName, Rarity itemDefaultRarity, Rarity rolledRarity)
    {
        if (rolledRarity < itemDefaultRarity)
        {
            Debug.LogWarning($"{itemName}: Default Rarity ({itemDefaultRarity}) is higher than rolled rarity ({rolledRarity}).");
            return false;
        }
        Debug.LogWarning($"{itemName} found, accessing..");
        return true;
    }
    
    public void RegisterActiveItem(string itemName)
    {
        if (!activeItems.Contains(itemName))
        {
            activeItems.Add(itemName);
        }
    }
    
    public void UnregisterActiveItem(string itemName)
    {
        if (activeItems.Count <= 0) return;
        
        List<string> itemsToRemove = new List<string>();

        foreach (string item in activeItems)
        {
            if (item == itemName)
            {
                itemsToRemove.Add(item);
            }
        }
        
        foreach (string item in itemsToRemove)
        {
            activeItems.Remove(item);
        }
    }

    public List<string> GetActiveItemNames() //save state
    {
        List<string>itemsInWorld = new List<string>();
        
        foreach (var item in activeItems)
        {
            itemsInWorld.Add(item);
        }
        activeItems.Clear();
        availableItems.Clear();
        return itemsInWorld;
    }
    
    public void LoadActiveItemState(WorldState state)
    {
        //LoadItems();
        if (state != null)
        {
            Debug.Log("Loading saved item states...");
            foreach (var itemName in state.activeItemNames)
            {
                activeItems.Add(itemName);
            }
        }
        else
        {
            Debug.LogWarning("No active item data found. ");
        }
    }
    
    public void ResetItems()
    {
        activeItems.Clear();
        availableItems.Clear();
        //ItemFactory.ClearFactory(); 
        Debug.LogWarning($"Itemdatabase reset");
    }

}

