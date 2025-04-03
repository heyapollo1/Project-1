using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public int inventorySlots = 8;
    public GameObject itemDropPrefab;
    private Dictionary<string, BaseItem> itemsInInventory = new(); // Stores active item objects
    private Dictionary<string, Rarity> savedItemRarities = new(); //stores rarity for saving/loading

    private InventoryUI inventoryUI;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void AddItem(BaseItem selectedItem, bool isUpgrade)
    {
        if (isUpgrade)
        {
            InventoryUI.Instance.UpgradeItemInInventorySlot(selectedItem);
            itemsInInventory[selectedItem.itemName] = selectedItem;
            savedItemRarities[selectedItem.itemName] = selectedItem.currentRarity;
            Debug.Log($"Upgraded {selectedItem.itemName} to {selectedItem.currentRarity}.");
        }
        else if (DoesInventoryHaveSpace())
        {
            savedItemRarities.Add(selectedItem.itemName, selectedItem.currentRarity);
            itemsInInventory.Add(selectedItem.itemName, selectedItem);
            //itemsInInventory[selectedItem.itemName] = selectedItem;
            //savedItemRarities[selectedItem.itemName] = selectedItem.currentRarity;
            selectedItem.Apply(AttributeManager.Instance);
            InventoryUI.Instance.PlaceItemIntoInventory(selectedItem);
            Debug.Log($"Added new item: {selectedItem.itemName} ({selectedItem.currentRarity}).");
            EventManager.Instance.TriggerEvent("ItemInteraction", selectedItem.itemName);
        }
        else
        {
            Debug.Log("Not enough inventory space.");
            return;
        }
    }
    
    public void DiscardItem(BaseItem selectedItem)
    {
        if (itemsInInventory.ContainsKey(selectedItem.itemName))
        {
            itemsInInventory.Remove(selectedItem.itemName);
            savedItemRarities.Remove(selectedItem.itemName);
            selectedItem.Remove(AttributeManager.Instance);

            Vector2 dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Spawn item in the world
            GameObject dropped = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
            ItemDrop droppedItem = dropped.GetComponent<ItemDrop>();
            if (droppedItem != null)
            {
                droppedItem.DropItemReward(selectedItem, PlayerController.Instance.transform);
            }

            Debug.Log($"Dropped {selectedItem.itemName} into world at {dropPosition}.");
        }
        
        if (itemsInInventory.ContainsKey(selectedItem.itemName))
        {
            itemsInInventory.Remove(selectedItem.itemName);
            savedItemRarities.Remove(selectedItem.itemName);
            selectedItem.Remove(AttributeManager.Instance);
            Debug.Log($"Removed {selectedItem.itemName} from inventory.");
        }
        
        EventManager.Instance.TriggerEvent("ItemInteraction", selectedItem.itemName);
    }

    public bool DoesInventoryHaveSpace()
    {
        return itemsInInventory.Count < inventorySlots;
    }
    
    public bool HasItem(string itemName)
    {
        Debug.Log($"Checking for item: '{itemName}' in inventory.");
    
        foreach (var key in itemsInInventory.Keys)
        {
            Debug.Log($"Inventory contains: '{key}'");
        }

        bool exists = itemsInInventory.ContainsKey(itemName);
        return exists;
    }
    
    public Rarity? GetOwnedItemRarity(string itemName)
    {
        foreach (var item in savedItemRarities)
        {
            if (item.Key == itemName) // Check if the item exists in inventory
            {
                return item.Value; // Return its rarity
            }
        }
        return null;
    }
    
    public void ApplyItemBonuses()
    {
        foreach (var item in itemsInInventory.Values)
        {
            item.Apply(AttributeManager.Instance);
            Debug.LogWarning($"Item: '{item.itemName}' (Rarity: {item.currentRarity}) applied");
        }
    }
    
    public void LoadInventoryFromSave(PlayerState playerState) //load state
    {
        Debug.Log("Loading Items In Inventory");
        if (playerState.inventoryWithRarity.Count == 0)
        {
            Debug.LogWarning("No saved inventory found! Inventory might not be saving correctly.");
            return;
        }
        
        inventorySlots = playerState.inventorySlots;
        InventoryUI.Instance.SetupInitialSlots(inventorySlots);
        
        foreach (var itemNameWithRarity in playerState.inventoryWithRarity)
        {
            int slotIndex;
            string itemName;
            Rarity itemRarity;
            BaseItem savedItem;
            
            string[] parts = itemNameWithRarity.Split(':');
            
            if (parts.Length == 3) // slotindex, name, rarity
            {
                slotIndex = int.Parse(parts[0]);
                itemName = parts[1];
                if (itemName == "EMPTY")
                {
                    Debug.Log($"Skipping empty slot {slotIndex}");
                    continue;
                }
                
                if (System.Enum.TryParse(parts[2], out itemRarity))
                {
                    savedItem = ItemFactory.CreateItem(itemName, ItemDatabase.Instance.LoadItemIcon(itemName), itemRarity);
                    InventoryUI.Instance.SetItemInSlot(slotIndex, savedItem);
                    //itemsInInventory[itemName] = savedItem; 
                    savedItemRarities.Add(itemName, itemRarity);
                    itemsInInventory.Add(itemName, savedItem);
                    Debug.Log($"Added new item: {savedItem.itemName} ({savedItem.currentRarity}).");
                }
                else
                {
                    Debug.LogError($"Failed to parse rarity for {itemName}");
                }
            }
            else
            {
                Debug.LogError("Invalid rarity format for item:");
                return;
            }
        }
        Debug.Log("Inventory loaded from save.");
    }
    
    public List<string> GetInventoryNamesWithRarity() //save state
    {
        //InventoryUI.Instance.SetupInitialSlots();
        Debug.Log($"Saving inventory items. Total items: {savedItemRarities.Count}");
        List<string> itemNamesWithRarity = new List<string>();
        var slots = InventoryUI.Instance.GetInventorySlots();
        
        for (int i = 0; i < slots.Count; i++) // Iterate over each slot index
        {
            if (slots[i].IsFilled())
            {
                BaseItem item = slots[i].GetItemInSlot();
                itemNamesWithRarity.Add($"{i}:{item.itemName}:{item.currentRarity}");
                Debug.Log($"Saved item at slot {i}: {item.itemName} ({item.currentRarity})");
            }
            else
            {
                itemNamesWithRarity.Add($"{i}:EMPTY"); // Mark empty slots
            }
        }
        
        if (savedItemRarities.Count == 0)
        {
            Debug.LogWarning("No items in savedItemRarities! Inventory might not be updating correctly.");
        }

        //ResetInventory();
        return itemNamesWithRarity;
    }
    
    public void ResetInventory()
    {
        itemsInInventory.Clear();
        //EventManager.Instance.TriggerEvent("InventoryUpdated", itemsInInventory);
    }
}