using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public int inventorySlots = 4;
    public GameObject itemDropPrefab;
    public ItemTracker itemTracker;
    
    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void Start()
    {
        itemTracker = ItemTracker.Instance;
    }
    
    public bool isThereSpaceInInventory()
    { 
        Debug.Log($"Inventory item count: {itemTracker.GetInventoryItemCount()}, slots left: {inventorySlots}");
        return itemTracker.GetInventoryItemCount() < inventorySlots;
    }
    
    public void AddItemToInventory(ItemPayload item, int slotIndex = -1)
    {
        bool toTargetSlot = slotIndex != -1;
        
        if (item.isWeapon)
        {
            WeaponInstance weaponInstance = item.weaponScript;
            InventoryUI.Instance.SetItemInSlot(toTargetSlot ? slotIndex : InventoryUI.Instance.GetFreeInventorySlot(), null, weaponInstance);
            itemTracker.AssignItemToTracker(item, ItemLocation.Inventory, toTargetSlot ? slotIndex : InventoryUI.Instance.GetFreeInventorySlot());
            EventManager.Instance.TriggerEvent("ItemInteraction", weaponInstance.weaponTitle);
            Debug.Log($"Added WEAPON: {weaponInstance.weaponTitle} to inventory");
        }
        else
        {
            BaseItem baseItem  = item.itemScript;
            item.itemScript.Apply();
            InventoryUI.Instance.SetItemInSlot(toTargetSlot ? slotIndex : InventoryUI.Instance.GetFreeInventorySlot(), baseItem);
            itemTracker.AssignItemToTracker(item, ItemLocation.Inventory, toTargetSlot ? slotIndex : InventoryUI.Instance.GetFreeInventorySlot());
            EventManager.Instance.TriggerEvent("ItemInteraction", baseItem.itemName);
            Debug.Log($"Added ITEM: {baseItem.itemName} to inventory");
        }
    }
    
     public void HandleInventoryPayload(UISwapPayload payload, int targetSlotIndex, int sourceSlotIndex, bool isFromHand = false, bool toEmptySlot = false)
     { 
        ItemPayload sourcePayload = payload.sourceItem;
        ItemPayload targetPayload = payload.targetItem;
        bool isSourceAWeapon = sourcePayload.isWeapon;
        bool isTargetAWeapon = targetPayload.isWeapon;

        var weaponManager = WeaponManager.Instance;
        var inventoryUI = InventoryUI.Instance;
        
        if (toEmptySlot) 
        {
            if (isFromHand)
            {
                Debug.Log("Adding item to inventory from HAND");
                weaponManager.DeleteItemFromLoadouts(isSourceAWeapon ? sourcePayload.weaponScript : null,
                    isSourceAWeapon ? null : sourcePayload.itemScript);
                AddItemToInventory(sourcePayload, targetSlotIndex);
            }
            else
            {
                inventoryUI.GetInventorySlotByIndex(sourceSlotIndex).Clear();
                inventoryUI.SetItemInSlot(targetSlotIndex, isSourceAWeapon ? null : sourcePayload.itemScript, isSourceAWeapon ? sourcePayload.weaponScript : null);
            }
        }
        else
        {
            if (isFromHand)
            {
                DeleteItemFromInventory(isTargetAWeapon ? targetPayload.weaponScript : null, isTargetAWeapon ? null : targetPayload.itemScript);
                ItemLocation location = itemTracker.GetLocationByID(isSourceAWeapon ? sourcePayload.weaponScript?.uniqueID : sourcePayload.itemScript?.uniqueID);
                
                weaponManager.DeleteItemFromLoadouts(isSourceAWeapon ? sourcePayload.weaponScript : null,
                    isSourceAWeapon ? null : sourcePayload.itemScript);
                
                AddItemToInventory(sourcePayload, targetSlotIndex);
                weaponManager.Equip(location, targetPayload);
            }
            else
            {
                inventoryUI.GetInventorySlotByIndex(sourceSlotIndex).Clear();
                inventoryUI.GetInventorySlotByIndex(targetSlotIndex).Clear();
                inventoryUI.SetItemInSlot(targetSlotIndex, isSourceAWeapon ? null : sourcePayload.itemScript, isSourceAWeapon ? sourcePayload.weaponScript : null);
                inventoryUI.SetItemInSlot(sourceSlotIndex, isTargetAWeapon ? null : targetPayload.itemScript, isTargetAWeapon ? targetPayload.weaponScript : null);
            }
        }
    }
     
    public void DropInventoryObject(WeaponInstance selectedWeapon = null, BaseItem selectedItem = null)
    {
        bool isWeapon = selectedItem == null;
        string itemID = isWeapon ? selectedWeapon?.uniqueID : selectedItem.uniqueID;
        
        if (itemID != null)
        {
            Vector2 dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject dropObj = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
            ItemDrop droppedItem = dropObj.GetComponent<ItemDrop>();
            if (isWeapon)
            {
                var weaponPayload = new ItemPayload()
                {
                    weaponScript =  selectedWeapon,
                    itemScript = null,
                    isWeapon = true
                };
                droppedItem.DropItemReward(PlayerController.Instance.transform, weaponPayload);
                ItemTracker.Instance.AssignItemToTracker(weaponPayload, ItemLocation.World, -1, -1, dropObj);
            }
            else
            {
                var itemPayload = new ItemPayload()
                {
                    weaponScript =  null,
                    itemScript = selectedItem,
                    isWeapon = false
                };
                
                droppedItem.DropItemReward(PlayerController.Instance.transform, itemPayload);
                selectedItem.Remove();
                ItemTracker.Instance.AssignItemToTracker(itemPayload, ItemLocation.World, -1, -1, dropObj);
            }
            Debug.Log($"Dropped {itemID} into world at {dropPosition}.");
        }
        itemTracker.UnassignItemFromTracker(itemID);
    }

    public void DeleteItemFromInventory(WeaponInstance weapon = null, BaseItem item = null)
    {
        bool isWeapon = item == null;
        if (isWeapon)
        {
            InventoryUI.Instance.GetInventorySlotByItemID(weapon.uniqueID).Clear();
        }
        else
        {
            item.Remove();
            InventoryUI.Instance.GetInventorySlotByItemID(item.uniqueID).Clear();
        }
        itemTracker.UnassignItemFromTracker(isWeapon ? weapon.uniqueID : item.uniqueID);
    }
    
    public void ApplyInventoryBonuses()//Add game/stat logic
    {
        if (ItemTracker.Instance.GetPlayerItems().Count > 0)
        {
            foreach (var item in itemTracker.GetPlayerItems())
            {
                item.Apply();
                Debug.LogWarning($"Item: '{item.itemName}' applied on load");
            }
        }
    }
    
    public void LoadInventoryFromSave(PlayerState playerState)
    {
        Debug.LogWarning("No saved inventory found!");
        if (playerState.inventoryItemData.Count == 0)
        {
            Debug.LogWarning("No saved inventory found!");
            return;
        }


        string type;
        int slotIndex;
        string uniqueID;
        string name;
        
        inventorySlots = playerState.inventorySlots;
        itemTracker = ItemTracker.Instance;
        InventoryUI.Instance.SetupInitialSlots(inventorySlots);
        Debug.LogWarning("No saved inventory found!");
        foreach (var itemData in playerState.inventoryItemData)
        {
            string[] parts = itemData.Split(':');
            Debug.LogWarning("Testing - pass");
            if (parts.Length == 4) // slotindex, isWeapon, name
            {
                type = parts[0];
                slotIndex = int.Parse(parts[1]);
                uniqueID = parts[2];
                name = parts[3];
    
                if (type == "ITEM")
                {
                    BaseItem savedItem = ItemDatabase.CreateItem(name);
                    savedItem.uniqueID = uniqueID;
                    InventoryUI.Instance.SetItemInSlot(slotIndex, savedItem);
                        
                    var savedItemPayload = new ItemPayload()
                    {
                        weaponScript =  null,
                        itemScript = savedItem,
                        isWeapon = false
                    };
                    itemTracker.AssignItemToTracker(savedItemPayload, ItemLocation.Inventory, slotIndex);
                    Debug.Log($"INVENTORY LOAD - Item: {savedItem.itemName}.");
                }
                else
                {
                    Debug.LogWarning("No saved inventory found!");
                    WeaponInstance savedWeapon = WeaponDatabase.Instance.CreateWeaponInstance(name);
                    savedWeapon.uniqueID = uniqueID;
                    InventoryUI.Instance.SetItemInSlot(slotIndex, null, savedWeapon);
                       
                    var savedWeaoponPayload = new ItemPayload()
                    {
                        weaponScript =  savedWeapon,
                        itemScript = null,
                        isWeapon = true
                    };
                    itemTracker.AssignItemToTracker(savedWeaoponPayload, ItemLocation.Inventory, slotIndex);
                    Debug.Log($"INVENTORY LOAD - Weapon: {savedWeapon.weaponTitle}.");
                }
            }
        }
        Debug.Log("Inventory successfully loaded from save.");
    }
}