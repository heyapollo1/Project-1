using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    
    [Header("Shop Tester")]
    public GameObject shopItemCardPrefab;
    
    [Header("Shop Asset")]
    public GameObject shopMap;
    public Pedestal[] pedestals;
    
    [HideInInspector] public StageBracket stageBracket;
    public bool shopActive;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        GameData gameData = SaveManager.LoadGame();
        if (gameData.isNewGame) 
        {
            Debug.Log("Starting a new game. Deactivating shop map.");
            shopMap.SetActive(shopActive);
            shopActive = false;
        }
    }
    public void PrepareShop(StageBracket bracket)
    {
        if (shopActive) return;
        shopActive = true;
        stageBracket = bracket;
        shopMap.SetActive(true);

        AudioManager.Instance.PlayBackgroundMusic("Music_Shopping");
        EventManager.Instance.TriggerEvent("ShowUI");
        foreach (var pedestal in pedestals)
        {
            object shopEntry = GetRandomShopEntry(stageBracket);

            if (shopEntry is BaseItem item)
            {
                ItemDatabase.Instance.RegisterActiveItem(item.itemName);
                pedestal.SetItem(item);
                Debug.Log($"assigning ITEM {item} to {pedestal.name}");
            }
            else if (shopEntry is WeaponInstance weapon)
            {
                WeaponDatabase.Instance.RegisterActiveWeapon(weapon.weaponTitle);
                pedestal.SetWeapon(weapon);
                Debug.Log($"assigning WEAPON {weapon.weaponTitle} to {pedestal.name}");
            }
        }
    }
    
    private object GetRandomShopEntry(StageBracket bracket)
    {
        float roll = Random.value;
        if (roll < 0.5f)
        {
            return ItemDatabase.Instance.GetRandomItem(bracket);
        }
        else
        {
            return WeaponDatabase.Instance.GetRandomWeapon(bracket);
        }
    }
    
    public void LeaveShop()
    {
        if (!shopActive) return;
        shopActive = false;
        
        ClearPedestals();
        shopMap.SetActive(false);
    }
    
    public void LoadShopState(WorldState state) // load state
    {
        shopActive = true;
        shopMap.SetActive(true);
        
        if (state.pedestalItems.Count != pedestals.Length)
        {
            Debug.LogError("Mismatch between saved pedestal items and available pedestals!");
            return;
        }
        
        for (int i = 0; i < pedestals.Length; i++)
        {
            string pedestalItemData = state.pedestalItems[i];
            if (string.IsNullOrEmpty(pedestalItemData) || pedestalItemData == "EMPTY")
            {
                pedestals[i].ResetPedestal();
                continue;
            }
            
            string[] parts = pedestalItemData.Split(':');
            if (parts.Length != 3)
            {
                Debug.LogError($"Invalid pedestal entry format: {pedestalItemData}");
                continue;
            }
            
            string type = parts[0];
            string name = parts[1];
            Rarity rarity = Rarity.Common;
            System.Enum.TryParse(parts[2], out rarity);
            
            if (type == "ITEM")
            {
                var item = ItemFactory.CreateItem(name, ItemDatabase.Instance.LoadItemIcon(name), rarity);
                if (item != null) pedestals[i].SetItem(item);
                else Debug.LogError($"Failed to load item: {name}");
            }
            else if (type == "WEAPON")
            {
                var weapon = WeaponDatabase.Instance.CreateWeaponInstance(name, rarity);
                if (weapon != null) pedestals[i].SetWeapon(weapon);
                else Debug.LogError($"Failed to load weapon: {name}");
            }
            else
            {
                Debug.LogError($"Unknown pedestal type: {type}");
            }
        }

    }
    
    public List<string> GetPedestalItemNamesAndRarity() //Saving State
    {
        List<string> pedestalItems = new List<string>();
        foreach (var pedestal in pedestals)
        {
            if (pedestal.displayItem != null)
            {
                string name = pedestal.displayItem.itemName;
                Rarity rarity = pedestal.displayItem.currentRarity;
                pedestalItems.Add($"ITEM:{name}:{rarity}");
            }
            else if (pedestal.displayWeapon != null)
            {
                string name = pedestal.displayWeapon.weaponTitle;
                Rarity rarity = pedestal.displayWeapon.rarity;
                pedestalItems.Add($"WEAPON:{name}:{rarity}");
            }
            else
            {
                pedestalItems.Add("EMPTY");
            }
        }
        return pedestalItems;
    }
    
    private void ClearPedestals()
    {
        foreach (var pedestal in pedestals)
        {
            Debug.LogError("Clearing pedestal items");
            pedestal.ResetPedestal();
        }
    }
    
    public void RerollShop()
    {
        ClearPedestals();
        foreach (var pedestal in pedestals)
        {
            object shopEntry = GetRandomShopEntry(stageBracket);

            if (shopEntry is BaseItem item)
            {
                ItemDatabase.Instance.RegisterActiveItem(item.itemName);
                pedestal.SetItem(item);
                Debug.Log($"assigning ITEM {item} to {pedestal.name}");
            }
            else if (shopEntry is WeaponInstance weapon)
            {
                WeaponDatabase.Instance.RegisterActiveWeapon(weapon.weaponTitle);
                pedestal.SetWeapon(weapon);
                Debug.Log($"assigning WEAPON {weapon.weaponTitle} to {pedestal.name}");
            }
            Debug.Log("Rerolling pedestal items");
            //pedestal.RerollItem(stageBracket);
        }
    }
}