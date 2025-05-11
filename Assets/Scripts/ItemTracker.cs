using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemLocation
{
    World,
    Inventory,
    LeftHand,
    RightHand,
    Shop
}

public class TrackedItemInfo
{
    public bool isWeapon;
    public string uniqueID;
    public string itemName;
    public int loadoutIndex;
    public int slotIndex;
    
    public ItemLocation location;
    public BaseItem itemScript;
    public WeaponInstance weaponScript;
    public GameObject worldObjectRef;
}

public class ItemTracker : MonoBehaviour
{
    public static ItemTracker Instance { get; private set; }
    
    private Dictionary<string, TrackedItemInfo> trackedItems = new();
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AssignItemToTracker(ItemPayload item, ItemLocation location, int slotIndex = -1, int loadoutIndex = -1, GameObject worldObject = null)
    {
        if (item.isWeapon)
        {
            WeaponInstance weapon = item.weaponScript;
            trackedItems[weapon.uniqueID] = new TrackedItemInfo
            {
                isWeapon = true,
                uniqueID = weapon.uniqueID,
                itemName = weapon.weaponTitle,
                location = location,
                loadoutIndex = loadoutIndex,
                slotIndex = slotIndex,
                worldObjectRef = worldObject,
                weaponScript = weapon,
                itemScript = null
            };
        }
        else
        {
            BaseItem baseItem  = item.itemScript;
            trackedItems[baseItem.uniqueID] = new TrackedItemInfo
            {
                isWeapon = false,
                uniqueID = baseItem.uniqueID,
                itemName = baseItem.itemName,
                location = location,
                loadoutIndex = loadoutIndex,
                slotIndex = slotIndex,
                worldObjectRef = worldObject,
                weaponScript = null,
                itemScript = baseItem
            };
        }
    }
    
    public void UnassignItemFromTracker(string uniqueID)
    {
        if (trackedItems.ContainsKey(uniqueID))
        {
            trackedItems.Remove(uniqueID);
        }
    }
    
    public bool DoesItemExist(string name)
    {
        if (trackedItems.TryGetValue(name, out var trackedInfo))
        {
            return trackedInfo.itemName.Contains(name);
        }
        Debug.LogWarning($"Item {name} is not being tracked.");
        return false;
    }
    
    public ItemLocation GetLocationByID(string id)
    {
        if (trackedItems.TryGetValue(id, out var trackedInfo))
        {
            return trackedInfo.location;
        }
        Debug.LogWarning($"Item with ID {id} is not being tracked.");
        return ItemLocation.World;
    }
    
    public int GetInventoryItemCount()
    {
        int count = 0;
        foreach (var trackedInfo in trackedItems.Values)
        {
            if (trackedInfo.location == ItemLocation.Inventory)
            {
                count++;
            }
        }
        return count;
    }
    
    public int GetLoadoutItemCount()
    {
        int count = 0;
        foreach (var trackedInfo in trackedItems.Values)
        {
            if (trackedInfo.location == ItemLocation.LeftHand || trackedInfo.location == ItemLocation.RightHand)
            {
                count++;
            }
        }
        return count;
    }
    
    public List<BaseItem> GetPlayerItems() //For applying item effects AFTER loading in data
    {
        List<BaseItem> playerItems = new List<BaseItem>();
        foreach (var item in trackedItems.Values)
        {
            if (item.location != ItemLocation.World || item.location != ItemLocation.Shop)
            {
                if (!item.isWeapon) playerItems.Add(item.itemScript);
            }
        }
        return playerItems;
    }
    
    public List<string> GetWorldItemData() //save state
    {
        Debug.Log($"Saving World item data");
        List<string> savedWorldItems = new List<string>();

        foreach (var trackedInfo in trackedItems.Values)
        {
            if (trackedInfo.location == ItemLocation.World)
            {
                Debug.Log($"Saving World item data: {trackedInfo.itemName} at {trackedInfo.worldObjectRef.transform.position}");
                bool isWeapon = trackedInfo.isWeapon;
                string savedItemName = trackedInfo.itemName;
                string id = trackedInfo.uniqueID;
                Vector3 pos = trackedInfo.worldObjectRef.transform.position;
                string location = $"{pos.x},{pos.y},{pos.z}";
                string data = $"{(isWeapon ? "WEAPON":"ITEM")}:{id}:{savedItemName}:{location}";

                savedWorldItems.Add(data);
            }
        }
        return savedWorldItems;
    }
    
    public List<string> GetInventoryItemData() //save state
    {
        Debug.Log("Saving inventory items");
        List<string> savedInventoryItems = new List<string>();

        foreach (var trackedInfo in trackedItems.Values)
        {
            if (trackedInfo.location == ItemLocation.Inventory)
            {
                bool isWeapon = trackedInfo.isWeapon;
                int savedSlotIndex = trackedInfo.slotIndex;
                string savedItemName = trackedInfo.itemName;
                string id = trackedInfo.uniqueID;
                savedInventoryItems.Add($"{(isWeapon ? "WEAPON":"ITEM")}:{savedSlotIndex}:{id}:{savedItemName}");
            }
        }
        return savedInventoryItems;
    }

    public Dictionary<string, Vector3> GetWorldItemPositions() //save world dropped item positions
    {
        Dictionary<string, Vector3> saveditemPositions = new();
        foreach (var trackedInfo in trackedItems.Values)
        {
            if (trackedInfo.location == ItemLocation.World)
                saveditemPositions.Add(trackedInfo.itemName, trackedInfo.worldObjectRef.transform.position);
        }
        return null;
    }
    
    public TrackedItemInfo GetTrackedItemData(string id)
    {
        foreach (var trackedInfo in trackedItems.Values)
        {
            if (trackedInfo.uniqueID == id)
                return trackedInfo;
        }
        return null;
    }
}

/*
public void AssignItemToTracker(ItemPayload item, ItemLocation location, int slotIndex = -1, int loadoutIndex = -1, GameObject worldObject = null)
    {
        if (item.isWeapon)
        {
            WeaponInstance weapon = item.weaponScript;
            if (!trackedItems.ContainsKey(weapon.uniqueID)) //check if new or if swapping from currently owned items
            {
                if (!weaponsByID.ContainsKey(weapon.uniqueID))
                {
                    Debug.Log($"No existing group for {weaponInstance.weaponTitle}, creating new.");
                    weaponsByID[weapon.uniqueID] = new List<WeaponInstance>();
                }
                Debug.Log($"Adding {weaponInstance.weaponTitle} to group.");
                weaponsByName[weaponInstance.weaponTitle].Add(weaponInstance);
            }
            //if (location == ItemLocation.World) worldItemDrops.Add(item.weaponScript.uniqueID, droppedObject);
            trackedItems[weaponInstance.uniqueID] = new TrackedItemInfo
            {
                isWeapon = true,
                uniqueID = weaponInstance.uniqueID,
                itemName = weaponInstance.weaponTitle,
                location = location,
                loadoutIndex = loadoutIndex,
                slotIndex = slotIndex,
                worldObjectRef = worldObject
            };
        }
        else
        {
            BaseItem baseItem  = item.itemScript;
            if (!trackedItems.ContainsKey(baseItem.uniqueID))
            {
                if (!itemsByName.ContainsKey(baseItem.itemName))
                {
                    Debug.Log($"No existing group for {baseItem.itemName}, creating new.");
                    itemsByName[baseItem.itemName] = new List<BaseItem>();
                }
                Debug.Log($"Found group for {baseItem.itemName}, adding to it.");
                itemsByName[baseItem.itemName].Add(baseItem);
            }
            
            trackedItems[baseItem.uniqueID] = new TrackedItemInfo
            {
                isWeapon = false,
                uniqueID = baseItem.uniqueID,
                itemName = baseItem.itemName,
                location = location,
                loadoutIndex = loadoutIndex,
                slotIndex = slotIndex,
                worldObjectRef = worldObject
            };*/
