using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutUIManager : MonoBehaviour
{
    public static LoadoutUIManager Instance { get; private set; }
    
    [Header("Loadouts")]
    public GameObject activeLoadoutPrefab;  
    public GameObject storedLoadoutPrefab;
    
    [Header("UI References")]
    public Transform storedLoadoutPanel;
    
    private List<StoredLoadoutUI> storedWeaponLoadouts = new List<StoredLoadoutUI>();
    private Dictionary<int, StoredLoadoutUI> loadoutUIDictionary = new Dictionary<int, StoredLoadoutUI>();
    //private Dictionary<string, WeaponSlotUI> weaponSlots = new Dictionary<string, WeaponSlotUI>();
    
    public void Initialize()
    {
        UpdateLoadoutUI();
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateLoadoutUI()
    {
        Debug.Log("Updating LoadoutUI");
        ActiveLoadoutUI activeLoadout = activeLoadoutPrefab.GetComponent<ActiveLoadoutUI>();
        foreach (Transform child in storedLoadoutPanel) Destroy(child.gameObject);
        
        activeLoadout.ClearActiveLoadout();
        storedWeaponLoadouts.Clear();
        loadoutUIDictionary.Clear();
            
        List<WeaponLoadout> loadouts = WeaponManager.Instance.GetPlayerLoadouts();
        int currentID = WeaponManager.Instance.GetCurrentLoadoutID();
        
        Debug.Log($"Updating Loadout no.: {currentID}");
        if (loadouts.Count > 0)
        {
            Debug.Log("Updating Active loadoutUI");
            WeaponLoadout currentLoadout = WeaponManager.Instance.GetLoadoutByID(currentID);
            activeLoadout.UpdateActiveLoadout(currentLoadout);
        }
        
        foreach (var loadout in loadouts)
        {
            if (loadout.loadoutID == currentID) continue;
            Debug.Log($"Updating Stored loadoutUI {loadout.loadoutID}: {loadout}");
            GameObject storedLoadout = Instantiate(storedLoadoutPrefab, storedLoadoutPanel);
            StoredLoadoutUI storedUI = storedLoadout.GetComponent<StoredLoadoutUI>();
            storedUI.Initialize(loadout);
            storedWeaponLoadouts.Add(storedUI);
            loadoutUIDictionary[loadout.loadoutID] = storedUI;
        }
    }
    
    public void HighlightMatchingWeapon(WeaponInstance hoveredWeapon, bool highlight)
    {
        if (WeaponManager.Instance.TryFindWeaponInLoadouts(hoveredWeapon, out int loadoutID, out bool isLeftHand))
        {
            if (WeaponManager.Instance.IsLoadoutActive(loadoutID))
            {
                ActiveLoadoutUI activeLoadout = activeLoadoutPrefab.GetComponent<ActiveLoadoutUI>();
                activeLoadout.HighlightSlot(hoveredWeapon, isLeftHand, highlight);
            }
            else
            {
                StoredLoadoutUI storedUI = GetStoredLoadoutUIByID(loadoutID);
                storedUI?.HighlightSlot(hoveredWeapon, isLeftHand, highlight);
            }
        }
    }
    
    public ActiveWeaponUI GetActiveSlotUI(bool isLeftHand)
    {
        ActiveLoadoutUI activeLoadout = activeLoadoutPrefab.GetComponent<ActiveLoadoutUI>();
        return isLeftHand ? activeLoadout.GetLeftWeaponUI() : activeLoadout.GetRightWeaponUI();
    }
    
    public StoredLoadoutUI GetStoredLoadoutUIByID(int id)
    {
        return loadoutUIDictionary.TryGetValue(id, out var ui) ? ui : null;
    }
    
    public void UpgradeWeaponInActiveLoadout(bool isLeftHand)
    {
        ActiveLoadoutUI activeLoadout = activeLoadoutPrefab.GetComponent<ActiveLoadoutUI>();
        activeLoadout.UpgradeActiveWeapons(isLeftHand);
    }
    
    public void UpgradeWeaponInStoredLoadout(int loadoutID, bool isLeftHand)
    {
        StoredLoadoutUI storedUI = GetStoredLoadoutUIByID(loadoutID);
        storedUI?.UpgradeWeaponInStoredLoadout(isLeftHand);
        UpdateLoadoutUI();
    }
    
    /*private void CreateWeaponSlotUI(WeaponInstance weapon, bool isLeftHand, int loadoutIndex)
   {
       //GameObject weaponSlot = Instantiate(weaponSlotPrefab, currentLoadoutPanel);
       //WeaponSlotUI slotUI = weaponSlot.GetComponent<WeaponSlotUI>();
       ActiveLoadoutUI activeLoadout = activeLoadoutPrefab.GetComponent<ActiveLoadoutUI>();
       string weaponKey = GenerateWeaponKey(loadoutIndex, isLeftHand, weapon?.weaponTitle ?? "Empty");
       RegisterWeaponKey(weaponKey, weapon);

       Debug.Log($"Storing active weapon: {weaponKey}");
       if (weapon != null)
       {
           activeLoadout.Initialize();
       }
       //activeWeapons.Add();
       activeLoadout.transform.SetSiblingIndex(isLeftHand ? 0 : 1);
   }*/

    /*public string GenerateWeaponKey(int loadoutIndex, bool isLeftHand, string weaponTitle)
    {
        return $"Loadout{loadoutIndex}_{(isLeftHand ? "L" : "R")}_{weaponTitle}";
    }
    
    public void RegisterWeaponKey(string weaponKey, WeaponInstance weapon)
    {
        if (!weaponSlots.ContainsKey(weaponKey))
        {
            Debug.Log($"Registering new weapon key: {weaponKey}");
            WeaponSlotUI slot = GetWeaponSlot(weaponKey);
            weaponSlots[weaponKey] = slot;
        }
    }
    
    public void RemoveWeaponKey(string weaponKey)
    {
        if (weaponSlots.ContainsKey(weaponKey))
        {
            Debug.Log($"Removing old weapon key: {weaponKey}");
            weaponSlots.Remove(weaponKey);
        }
    }

    public WeaponSlotUI GetWeaponSlot(string weaponKey)
    {
        return weaponSlots.ContainsKey(weaponKey) ? weaponSlots[weaponKey] : null;
    }
    
    public StoredWeaponUI GetStoredWeaponSlot(string weaponKey)
    {
        foreach (StoredLoadoutUI loadout in storedWeaponLoadouts)
        {
            StoredWeaponUI weaponUI = loadout.GetStoredWeaponUI(weaponKey);
            if (weaponUI != null)
            {
                return weaponUI;
            }
        }
        return null; 
    }*/
}