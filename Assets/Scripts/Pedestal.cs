
using Unity.VisualScripting;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [Header("Item Reward")]
    public BaseItem displayItem;
    public GameObject itemPrefab;
    
    [Header("Weapon Reward")]
    public WeaponInstance displayWeapon;
    public GameObject weaponPrefab;
    
    [Header("General")]
    public Transform displaySpawnPosition;
    public GameObject pedestalItem;
    
    private bool isDisplaying = false;
    
    public void SetItem(BaseItem item)
    {
        if (item == null || isDisplaying) return;
        isDisplaying = true;
        displayItem = item;
        pedestalItem = Instantiate(itemPrefab, displaySpawnPosition);
        pedestalItem.transform.localPosition = Vector3.zero;
        pedestalItem.transform.localScale = Vector3.one; 
        
        Debug.Log($"Item set: {item}");
        ItemDrop itemComponent = pedestalItem.GetComponent<ItemDrop>();
        if (itemComponent != null)
        {
            itemComponent.Initialize(item);
            itemComponent.SetPedestal(this);
        }
    }
    
    public void SetWeapon(WeaponInstance weapon)
    {
        if (weapon == null || isDisplaying) return;
        isDisplaying = true;
        displayWeapon = weapon;
        pedestalItem = Instantiate(weaponPrefab, displaySpawnPosition);
        pedestalItem.transform.localPosition = Vector3.zero;
        pedestalItem.transform.localScale = Vector3.one; 
        
        Debug.Log($"Weapon set: {weapon.weaponTitle}");
        WeaponDrop weaponComponent = pedestalItem.GetComponent<WeaponDrop>();
        if (weaponComponent != null)
        {
            weaponComponent.Initialize(weapon);
            weaponComponent.SetPedestal(this);
        }
    }
    
    public Rarity GetDisplayItemRarity()
    {
        if (displayItem == null) return Rarity.Common; // default?
        return displayItem.currentRarity;
    }

    public string GetDisplayItemName()
    {
        if (displayItem == null) return null;
        return displayItem.itemName;
    }
    
    public bool IsDisplaying()
    {
        return isDisplaying;
    }
    
    public void ResetPedestal()
    {
        if (!isDisplaying) return;
        if (displayItem != null)  ItemDatabase.Instance.UnregisterActiveItem(displayItem.itemName);
        if (displayWeapon != null)  WeaponDatabase.Instance.UnregisterActiveWeapon(displayWeapon.weaponTitle);
        Destroy(pedestalItem);
        isDisplaying = false;
        Debug.Log($"Pedestal reset and item/weapon unregistered");
    }

    public void RemoveDisplayItem()
    {
        Destroy(pedestalItem);
    }

    public void RerollItem(StageBracket stageBracket)
    {
        Debug.Log("Rerolling pedestal item");
        if (isDisplaying) ResetPedestal();
        int attempts = 0;
        if (displayItem != null)
        {
            BaseItem rerolledItem;
            do
            {
                attempts++;
                rerolledItem = ItemDatabase.Instance.GetRandomItem(stageBracket);
                if (attempts > 50)
                {
                    Debug.Log($"Rerolled too many times, breaking");
                    break;
                }
            } while (rerolledItem.itemName == displayItem.itemName);
            ItemDatabase.Instance.RegisterActiveItem(rerolledItem.itemName);
            SetItem(rerolledItem);
            Debug.Log($"Rerolled Pedestal Item: {rerolledItem.itemName}");
        }
        else if (displayWeapon != null)
        {
            WeaponInstance rerolledWeapon;
            do
            {
                attempts++;
                rerolledWeapon = WeaponDatabase.Instance.GetRandomWeapon(stageBracket);
                if (attempts > 50)
                {
                    Debug.Log($"Rerolled too many times, breaking");
                    break;
                }
            } while (rerolledWeapon.weaponTitle == displayWeapon.weaponTitle);
            WeaponDatabase.Instance.RegisterActiveWeapon(rerolledWeapon.weaponTitle);
            SetWeapon(rerolledWeapon);
            Debug.Log($"Rerolled Pedestal Weapon: {rerolledWeapon.weaponTitle}");
        }
    }
}