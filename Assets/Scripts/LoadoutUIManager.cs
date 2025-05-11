using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutUIManager : MonoBehaviour
{
    public static LoadoutUIManager Instance { get; private set; }
    
    [Header("UI References")]
    public WeaponSlotUI leftWeaponSlot;
    public WeaponSlotUI rightWeaponSlot;
    public ItemUI activeLeftItem;
    public ItemUI activeRightItem;
    public Transform storedLoadoutPanel;
    
    [Header("Item UI Prefab")]
    public GameObject itemUIPrefab;
    
    [Header("Stored Loadout Prefab")]
    public GameObject storedLoadoutPrefab;

    private Dictionary<int, StoredLoadoutUI> storedLoadoutDictionary = new Dictionary<int, StoredLoadoutUI>();
    
    
    public void Initialize()
    {
        UpdateLoadoutUI();
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public void UpdateLoadoutUI()
    {
        foreach (Transform child in storedLoadoutPanel) Destroy(child.gameObject);
        storedLoadoutDictionary.Clear();
        
        List<PlayerLoadout> playerLoadouts = WeaponManager.Instance.GetPlayerLoadouts();
        PlayerLoadout currentLoadout = WeaponManager.Instance.currentLoadout;
        UpdateActiveLoadout(currentLoadout);
        
        foreach (var loadout in playerLoadouts) // create stored loadouts
        {
            if (loadout == WeaponManager.Instance.currentLoadout) continue;
            
            GameObject storedLoadout = Instantiate(storedLoadoutPrefab, storedLoadoutPanel);
            StoredLoadoutUI storedUI = storedLoadout.GetComponent<StoredLoadoutUI>();
            storedUI.UpdateStoredLoadout(loadout);
            storedLoadoutDictionary[loadout.loadoutIndex] = storedUI;
        }
    }
    
    public void UpdateActiveLoadout(PlayerLoadout loadout)
    {
        if (loadout == null) return;
        ClearActiveLoadout(); //remove visual objects and set null to all references
        
        if (!loadout.IsHandFree(true))
        {
            if (loadout.leftHandItem == null)
            {
                WeaponManager.Instance.AttachActiveWeapon(null);
                return;
            }
            
            bool isWeapon = loadout.leftHandItem.isWeapon;
            GameObject leftObj = Instantiate(itemUIPrefab, leftWeaponSlot.transform);
            activeLeftItem = leftObj.GetComponent<ItemUI>();
            activeLeftItem.InitializeItemUI(isWeapon ? null : loadout.leftHandItem.itemScript, isWeapon ? loadout.leftHandItem.weaponScript : null, true);
            leftWeaponSlot.AssignItemToLoadoutSlot(activeLeftItem, isWeapon);
            WeaponManager.Instance.AttachActiveWeapon(isWeapon ? loadout.leftHandItem.weaponScript.weaponBase : null, true);
        }
        
        if (!loadout.IsHandFree(false))
        {
            if (loadout.rightHandItem == null)
            {
                WeaponManager.Instance.AttachActiveWeapon(null);
                return;
            }
            
            bool isWeapon = loadout.rightHandItem.isWeapon;
            GameObject rightObj = Instantiate(itemUIPrefab, rightWeaponSlot.transform);
            activeRightItem = rightObj.GetComponent<ItemUI>();
            activeRightItem.InitializeItemUI(isWeapon ? null : loadout.rightHandItem.itemScript, isWeapon ? loadout.rightHandItem.weaponScript : null, true);
            rightWeaponSlot.AssignItemToLoadoutSlot(activeRightItem, isWeapon);
            WeaponManager.Instance.AttachActiveWeapon(isWeapon ? loadout.rightHandItem.weaponScript.weaponBase : null);
        }
    }
    
    public void ClearActiveLoadout()
    {
        if (activeLeftItem != null)
        {
            Destroy(activeLeftItem.gameObject);
            leftWeaponSlot.SetEmpty();
            activeLeftItem = null;
        }

        if (activeRightItem != null)
        {
            Destroy(activeRightItem.gameObject);
            rightWeaponSlot.SetEmpty();
            activeRightItem = null;
        }
    }
    
    public void HighlightSlot(bool isLeftHand, bool active)
    {
        if (isLeftHand && activeLeftItem != null)
        {
            activeLeftItem.HighlightSlot(active);
        }
        else if (!isLeftHand && activeRightItem != null)
        {
            activeRightItem.HighlightSlot(active);
        }
    }
    
    public StoredLoadoutUI GetStoredLoadoutUIByID(int id)
    {
        return storedLoadoutDictionary.TryGetValue(id, out var ui) ? ui : null;
    }
}