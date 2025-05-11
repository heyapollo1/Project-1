/*sing System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemLocation
{
    Hand,
    Inventory,
    Dropped,
    Shop
}

public class PlayerItemStorage : MonoBehaviour
{
    public static PlayerItemStorage Instance { get; private set; }

    private Dictionary<string, WeaponInstance> allWeaponsByID = new();
    private Dictionary<string, BaseItem> allItemsByID = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void RegisterWeapon(WeaponInstance weapon, ItemLocation location)
    {
        weapon.currentLocation = location;
        allWeaponsByID[weapon.uniqueID] = weapon;
    }

    public void RegisterItem(BaseItem item, ItemLocation location)
    {
        item.currentLocation = location;
        allItemsByID[item.uniqueID] = item;
    }

    public IEnumerable<WeaponInstance> GetWeaponsInLocation(ItemLocation loc) =>
        allWeaponsByID.Values.Where(w => w.currentLocation == loc);

    public IEnumerable<BaseItem> GetItemsInLocation(ItemLocation loc) =>
        allItemsByID.Values.Where(i => i.currentLocation == loc);

    public WeaponInstance GetWeaponByID(string id) =>
        allWeaponsByID.TryGetValue(id, out var weapon) ? weapon : null;

    public BaseItem GetItemByID(string id) =>
        allItemsByID.TryGetValue(id, out var item) ? item : null;

    public void MoveWeapon(string id, ItemLocation newLoc)
    {
        if (allWeaponsByID.TryGetValue(id, out var weapon))
            weapon.currentLocation = newLoc;
    }

    public void MoveItem(string id, ItemLocation newLoc)
    {
        if (allItemsByID.TryGetValue(id, out var item))
            item.currentLocation = newLoc;
    }
}*/
