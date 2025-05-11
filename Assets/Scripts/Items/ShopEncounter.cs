using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ShopEncounter : BaseEncounter
{
    public static ShopEncounter Instance { get; private set; }
    
    public GameObject shopItemCardPrefab;
    public Pedestal[] pedestals;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void InitializeEncounter()
    {
        GameData gameData = SaveManager.LoadGame();
        if (gameData.isNewGame) 
        {
            Debug.Log("Starting a new game. Deactivating shop map.");
            entryPoint = TransitionManager.Instance.GetShopSpawnPoint();
            encounterIsActive = false;
            encounterMap.SetActive(encounterIsActive);
        }
    }
    
    public override void PrepareEncounter()
    {
        if (encounterIsActive) return;
        encounterIsActive = true;
        stageBracket = CurrentStageBracket();
        encounterMap.SetActive(true);
        EncounterManager.Instance.SaveEncounterState(encounterData);
        
        GeneratePedestalItems();
    }
    
    public override void EnterEncounter()
    {
        Debug.Log($"Encounter entered: {encounterData.name}");
        MapUI.Instance.EnableMapUI();
        AudioManager.Instance.PlayBackgroundMusic("Music_Shopping");
        EventManager.Instance.TriggerEvent("ShowUI");
        
        GameData currentData = SaveManager.LoadGame();
        SaveManager.SaveGame(currentData);
    }

    private void GeneratePedestalItems()
    {
        foreach (var pedestal in pedestals)
        {
            object shopEntry = RollItemType(stageBracket);//Roll and create between weapon OR item
            
            if (shopEntry is BaseItem item)
            {
                var itemPayload = new ItemPayload()
                {
                    weaponScript =  null,
                    itemScript = item,
                    isWeapon = false
                };
                pedestal.SetItem(itemPayload);
                Debug.Log($"assigning ITEM {item} to {pedestal.name}");
            }
            else if (shopEntry is WeaponInstance weapon)
            {
                var weaponPayload = new ItemPayload()
                {
                    weaponScript =  weapon,
                    itemScript = null,
                    isWeapon = true
                };
                pedestal.SetItem(weaponPayload);
                Debug.Log($"assigning WEAPON {weapon.weaponTitle} to {pedestal.name}");
            }
        } 
    }
    
    private object RollItemType(StageBracket bracket)
    {
        float roll = Random.value;
        if (roll < 0.8f)
        {
            return ItemDatabase.Instance.CreateRandomItem(bracket);
        }
        return WeaponDatabase.Instance.CreateRandomWeapon(bracket);
    }
    
    public override void EndEncounter()
    {
        if (!encounterIsActive) return;
        encounterIsActive = false;
        ClearPedestals();
        EncounterManager.Instance.ExitEncounterState();
        encounterMap.SetActive(false);
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
        GeneratePedestalItems();
        Debug.Log("Rerolling pedestal items");
    }
    
    public override void LoadEncounterState(WorldState state) // load state
    {
        encounterIsActive = true;
        encounterMap.SetActive(true);
        EncounterManager.Instance.SaveEncounterState(encounterData);
        
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
            string type = parts[0];
            string name = parts[1];

            if (type == "ITEM")
            {
                var item = ItemDatabase.CreateItem(name);
                var itemPayload = new ItemPayload()
                {
                    weaponScript =  null,
                    itemScript = item,
                    isWeapon = false
                };
                if (item != null) pedestals[i].SetItem(itemPayload);
                Debug.Log($"SHOP - Loading ITEM: {item.itemName} to {pedestals[i].name}");
            }
            else if (type == "WEAPON")
            {
                var weapon = WeaponDatabase.Instance.CreateWeaponInstance(name);
                var weaponPayload = new ItemPayload()
                {
                    weaponScript =  weapon,
                    itemScript = null,
                    isWeapon = true
                };
                if (weapon != null) pedestals[i].SetItem(weaponPayload);
                Debug.Log($"SHOP - Loading WEAPON: {weapon.weaponTitle} to {pedestals[i].name}");
            }
            else
            {
                Debug.LogError($"Unknown pedestal type: {type}");
            }
        }
    }
    
    public List<string> GetPedestalData() //Saving State
    {
        List<string> pedestalItemData = new List<string>();
        foreach (var pedestal in pedestals)
        {
            if (pedestal.IsActive)
            {
                string savedName = pedestal.displayItem.name;
                pedestalItemData.Add($"{(pedestal.isWeapon ? "WEAPON":"ITEM")}:{savedName}");
            }
            else
            {
                pedestalItemData.Add("EMPTY");
            }
        }
        return pedestalItemData;
    }
}