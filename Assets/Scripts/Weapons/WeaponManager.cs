
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    public GameObject leftHand; // attach left weapon here
    public GameObject rightHand; // attach right weapon here
    public WeaponBase activeLeftWeapon;
    public WeaponBase activeRightWeapon;
    public GameObject itemDropPrefab;

    private Dictionary<int, PlayerLoadout> loadoutByID = new Dictionary<int, PlayerLoadout>();

    [HideInInspector] public PlayerLoadout currentLoadout;
    private PlayerController playerController;
    private AttributeManager playerAttributes;
    private EnemyDetector enemyDetector;
    private ItemTracker itemTracker;

    private int currentLoadoutID = 0;
    private int nextLoadoutID = 0;
    private bool isSwapping = false;
    private bool isDisabled = false;
    private bool isInitialized = false;
    private bool isHandlingUI = false;
    private int handSlots = 0;
    
    public PlayerLoadout GetLoadoutByID(int id) => loadoutByID.TryGetValue(id, out var loadout) ? loadout : null;
    public int GetCurrentLoadoutID() => currentLoadoutID;
    public void UnassignItemFromLoadout(PlayerLoadout loadout, ItemLocation location) => loadout.UnassignItemFromLoadout(location);
    public bool IsWeaponInHand(bool isLeft) => isLeft ? activeLeftWeapon != null : activeRightWeapon != null;
    public WeaponBase GetActiveWeapon(bool isLeft) => isLeft ? activeLeftWeapon : activeRightWeapon;
    
    public bool isThereSpaceInLoadouts()
    {
        foreach (var loadout in loadoutByID)
        {
            return loadout.Value.IsHandFree(true) ||loadout.Value.IsHandFree(false);
        }
        return false;
    }

    public void AttachActiveWeapon(WeaponBase weapon, bool isLeft = false)
    {
        if (isLeft) activeLeftWeapon = weapon;
        else activeRightWeapon = weapon;
    }
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        ResetLoadouts();
    }
    
    private void OnSceneLoaded(string scene)
    {
        Debug.Log($"Scene loaded: leftHand = {leftHand}, rightHand = {rightHand}");
        if (scene == "GameScene")
        {
            GameData gameData = SaveManager.LoadGame();
            if (gameData.isNewGame) 
            {
                Debug.Log("New Game!");
                InitializeComponents();
                
                if (loadoutByID.Count == 0)
                {
                    AddNewLoadout(2);
                    Debug.Log("Default loadout created at game start.");
                }
            }
        }
    }
    
    public void InitializeComponents()
    {
        if (isInitialized) return;
        isInitialized = true;
        playerController = GetComponent<PlayerController>();
        playerAttributes = GetComponent<AttributeManager>();
        enemyDetector = GetComponent<EnemyDetector>();
        itemTracker = ItemTracker.Instance;
        
        leftHand = GameObject.FindWithTag("LeftHand");
        rightHand = GameObject.FindWithTag("RightHand");
    }
    
    public void AddNewLoadout(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int id = currentLoadoutID++;
            var loadout = new PlayerLoadout();
            loadout.loadoutIndex = id;
            loadoutByID[id] = loadout;
            Debug.Log($"Added new empty loadout! loadout: {id}");
        }
        currentLoadoutID = 1; //reset, just for player reference, NOT ACTUAL ID.
        currentLoadout = GetLoadoutByID(currentLoadoutID);
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public int GetIndexByLoadoutID(int id)
    {
        for (int i = 0; i < loadoutByID.Count; i++)
        {
            if (loadoutByID[i].loadoutIndex == id)
                return i;
        }
        return -1;
    }
    
    public void TrySwitchToNextLoadout()
    {
        int currentIndex = GetIndexByLoadoutID(currentLoadoutID);
        if (loadoutByID.Count <= 1) return;

        int nextIndex = (currentIndex + 1) % loadoutByID.Count;
        SwitchLoadoutByID(loadoutByID[nextIndex].loadoutIndex);
    }
    
    public void TrySwitchToPreviousLoadout()
    {
        int currentIndex = GetIndexByLoadoutID(currentLoadoutID);
        if (loadoutByID.Count <= 1) return;

        int prevIndex = (currentIndex - 1 + loadoutByID.Count) % loadoutByID.Count;
        SwitchLoadoutByID(loadoutByID[prevIndex].loadoutIndex);
    }
    
    public void SwitchLoadoutByID(int targetID)
    {
        if (isSwapping || currentLoadoutID == targetID) return;
        Debug.Log($"Switching loadout!{targetID}, {currentLoadoutID}");
        StartCoroutine(SwitchLoadout(targetID));
        Debug.Log($"Switching loadout!{targetID}");
    }
    
    private void Update()
    {
        if (loadoutByID.Count <= 1 || isDisabled || isSwapping || UIManager.Instance.IsDraggingItem) return;

        if (Input.GetKeyDown(KeyCode.E)) TrySwitchToNextLoadout();
        if (Input.GetKeyDown(KeyCode.Q)) TrySwitchToPreviousLoadout();
    }
    
    public void AddItemToLoadout(ItemPayload item)
    {
        if (currentLoadout.IsHandFree(true)) Equip(ItemLocation.LeftHand, item);
        else if (currentLoadout.IsHandFree(false)) Equip(ItemLocation.RightHand, item);
        else FindSpaceInStoredLoadouts(item);
    }

    private void FindSpaceInStoredLoadouts(ItemPayload item)
    { 
        foreach (var loadout in loadoutByID.Values)
        {
            if (loadout.loadoutIndex == currentLoadoutID) continue;
            if (loadout.IsHandFree(true))
            {
                StartCoroutine(SwitchLoadoutAndEquip(ItemLocation.LeftHand, item, loadout.loadoutIndex));
            }
            else if (loadout.IsHandFree(false))
            {
                StartCoroutine(SwitchLoadoutAndEquip(ItemLocation.RightHand, item, loadout.loadoutIndex));
            }
        }
    }
    
    public void Equip(ItemLocation location, ItemPayload item)
    {
        if (item.isWeapon)
        {
            InitializeWeaponInLoadout(item, location); //visuals
            Debug.Log($"Equipped new weapon: {item.weaponScript.weaponTitle}");
        }
        else
        {
            item.itemScript.Apply();
            InitializeItemInLoadout(item, location); //visuals
            Debug.Log($"Equipped new item: {item.itemScript.itemName}");
        }
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    private IEnumerator SwitchLoadoutAndEquip(ItemLocation location, ItemPayload item, int loadoutID)
    {
        yield return StartCoroutine(SwitchLoadout(loadoutID));
        Equip(location, item);

        ItemUI.draggedItem = null;
    }
    
    private IEnumerator SwitchLoadout(int targetID)
    {
        isSwapping = true;
        ToggleWeaponVisibility(false);
        if (!currentLoadout.IsHandFree(true))
        {
            bool isWeapon = currentLoadout.leftHandItem.isWeapon;
            if (isWeapon) DestroyHandPrefab(ItemLocation.LeftHand);
        }
        if (!currentLoadout.IsHandFree(false))
        {
            bool isWeapon = currentLoadout.rightHandItem.isWeapon;
            if (isWeapon) DestroyHandPrefab(ItemLocation.RightHand);
        }
        
        yield return new WaitForSeconds(0.25f);

        currentLoadoutID = targetID;
        var newLoadout = GetLoadoutByID(currentLoadoutID);
        if (newLoadout == null)
        {
            Debug.LogError($"Target loadout (ID: {currentLoadoutID}) not found!");
            isSwapping = false;
            yield break;
        }

        if (!newLoadout.IsHandFree(true))
        {
            if (newLoadout.leftHandItem.isWeapon)
            {
                InitializeWeaponInLoadout(newLoadout.leftHandItem, ItemLocation.LeftHand);
            }
            else
            {
                InitializeItemInLoadout(newLoadout.leftHandItem, ItemLocation.LeftHand);
            }
        }
        if (!newLoadout.IsHandFree(false))
        {
            if (newLoadout.rightHandItem.isWeapon)
            {
                InitializeWeaponInLoadout(newLoadout.rightHandItem, ItemLocation.RightHand);
            }
            else
            {
                InitializeItemInLoadout(newLoadout.rightHandItem, ItemLocation.RightHand);
            }
        }
        
        isSwapping = false;
        currentLoadout = newLoadout;
        ToggleWeaponVisibility(true);
        if (TooltipManager.Instance.isHoverTooltipActive()) TooltipManager.Instance.ClearHoverTooltip();
        LoadoutUIManager.Instance.UpdateLoadoutUI();
        Debug.Log($"Switched to loadout {currentLoadoutID}. New weapons: Left - {newLoadout.leftHandItem}, Right - {newLoadout.rightHandItem}");
    }
    
    private void InitializeWeaponInLoadout(ItemPayload payload, ItemLocation location)
    {
        currentLoadout.AssignItemToLoadout(payload, location);
        itemTracker.AssignItemToTracker(payload, location, -1, currentLoadoutID);
        
        Transform equippedLocation = location == ItemLocation.LeftHand ? leftHand.transform : rightHand.transform;
        GameObject weaponObject = Instantiate(payload.weaponScript.weaponPrefab, equippedLocation);
        WeaponBase newWeapon = weaponObject.GetComponent<WeaponBase>();
        
        if (newWeapon == null) return;
        
        payload.weaponScript.weaponBase = newWeapon;
        newWeapon.weaponInstance = payload.weaponScript;
        newWeapon.equippedLocation = equippedLocation;
        newWeapon.enemyDetector = enemyDetector;
        newWeapon.playerController = playerController;
        newWeapon.playerAttributes = playerAttributes;
        newWeapon.weaponData.enemyLayer = LayerMask.GetMask("Enemy");
        newWeapon.InitializeWeapon();
    }
    
    private void InitializeItemInLoadout(ItemPayload payload, ItemLocation location)
    {
        currentLoadout.AssignItemToLoadout(payload, location);
        itemTracker.AssignItemToTracker(payload, location, -1, currentLoadoutID);
    }
    
    public void HandleLoadoutPayload(UISwapPayload payload, bool targetIsLeft, bool isFromInventory = false, bool toEmptySlot = false)
    {
        if (currentLoadout == null || isHandlingUI) return;
        ItemLocation sourceOldLocation = targetIsLeft ? ItemLocation.RightHand : ItemLocation.LeftHand;
        ItemLocation targetOldLocation = targetIsLeft ? ItemLocation.LeftHand : ItemLocation.RightHand;
        ItemPayload sourcePayload = payload.sourceItem;
        ItemPayload targetPayload = payload.targetItem;
        bool isSourceAWeapon = sourcePayload.isWeapon;
        bool isTargetAWeapon = targetPayload.isWeapon;
        string sourceID = isSourceAWeapon ? sourcePayload.weaponScript?.uniqueID : sourcePayload.itemScript?.uniqueID;
        string targetID = isTargetAWeapon ? targetPayload.weaponScript?.uniqueID : targetPayload.itemScript?.uniqueID;
        
        if (toEmptySlot) 
        {
            if (isFromInventory)
            {
                Debug.Log("Placing item from INVENTORY into empty hand slot");
                InventoryManager.Instance.DeleteItemFromInventory(isSourceAWeapon ? sourcePayload.weaponScript : null,
                    isSourceAWeapon ? null : sourcePayload.itemScript);
                Equip(targetOldLocation, sourcePayload);
            }
            else
            {
                Debug.Log("Placing item from HAND into empty hand slot");
                if (isSourceAWeapon) DestroyHandPrefab(sourceOldLocation);
                if (!isSourceAWeapon) sourcePayload.itemScript?.Remove();
                itemTracker.UnassignItemFromTracker(sourceID);
                UnassignItemFromLoadout(currentLoadout, sourceOldLocation);
                Equip(targetOldLocation, sourcePayload);
            }
        }
        else
        {
            if (isFromInventory)
            {
                Debug.Log("Trading hand item w inventory item");
                if (isTargetAWeapon) DestroyHandPrefab(targetOldLocation);
                if (!isTargetAWeapon) targetPayload.itemScript?.Remove();
                itemTracker.UnassignItemFromTracker(targetID);
                UnassignItemFromLoadout(currentLoadout, targetOldLocation);
                
                int sourceSlotIndex = InventoryUI.Instance.GetInventorySlotByItemID(isSourceAWeapon ? sourcePayload.weaponScript?.uniqueID : sourcePayload.itemScript?.uniqueID).slotIndex;
                InventoryManager.Instance.DeleteItemFromInventory(isSourceAWeapon ? sourcePayload.weaponScript : null,
                    isSourceAWeapon ? null : sourcePayload.itemScript);
                
                Equip(targetOldLocation, sourcePayload);
                InventoryManager.Instance.AddItemToInventory(targetPayload, sourceSlotIndex);
            }
            else
            {
                Debug.Log("Swapping hand item w hand item");
                if (isSourceAWeapon) DestroyHandPrefab(sourceOldLocation);
                if (!isSourceAWeapon) sourcePayload.itemScript?.Remove();
                itemTracker.UnassignItemFromTracker(sourceID);
                UnassignItemFromLoadout(currentLoadout, sourceOldLocation);
                
                if (isTargetAWeapon) DestroyHandPrefab(targetOldLocation);
                if (!isTargetAWeapon) targetPayload.itemScript?.Remove();
                itemTracker.UnassignItemFromTracker(targetID);
                UnassignItemFromLoadout(currentLoadout, targetOldLocation);
                
                Equip(targetOldLocation, sourcePayload);
                Equip(sourceOldLocation, targetPayload);
            }
        }
    }
    
    public void DestroyHandPrefab(ItemLocation targetLocation)
    {
        bool isLeftHand = targetLocation == ItemLocation.LeftHand;
        Transform parent = isLeftHand ? leftHand.transform : rightHand.transform;
        int childCount = parent.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Debug.Log($"Destroying: {parent.GetChild(i).gameObject.name}");
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    
    public void DeleteItemFromLoadouts(WeaponInstance weapon = null, BaseItem item = null)
    {
        bool isWeapon = item == null;
        string uniqueID = isWeapon ? weapon.uniqueID : item.uniqueID;
        ItemLocation location = itemTracker.GetLocationByID(uniqueID);
        itemTracker.UnassignItemFromTracker(uniqueID);
        
        if (isWeapon)
        {
            Debug.Log($"Deleting weapon from loadout: {weapon?.weaponTitle}, location {location.ToString()}");
            UnassignItemFromLoadout(currentLoadout, location);
            DestroyHandPrefab(location);
        }
        else
        {
            Debug.Log($"Deleting item from loadout: {item?.itemName}");
            UnassignItemFromLoadout(currentLoadout, location);
            item.Remove();
        }
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public void DropHandObject(WeaponInstance selectedWeapon = null, BaseItem selectedItem = null)
    {
        var loadout = GetLoadoutByID(currentLoadoutID);
        bool isWeapon = selectedItem == null;
        string uniqueID = isWeapon ? selectedWeapon.uniqueID : selectedItem.uniqueID;
        
        ItemLocation location = itemTracker.GetLocationByID(uniqueID);
        Vector3 dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject dropObject = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
        ItemDrop droppedItem = dropObject.GetComponent<ItemDrop>();
        itemTracker.UnassignItemFromTracker(uniqueID);
        UnassignItemFromLoadout(loadout, location);
        
        var droppedPayload = new ItemPayload()
        {
            weaponScript =  isWeapon ? selectedWeapon : null,
            itemScript = isWeapon ? null : selectedItem,
            isWeapon = isWeapon
        };
        
        if (isWeapon && selectedWeapon != null)
        {
            Debug.Log($"Dropping weapon from loadout: {selectedWeapon.weaponTitle}");
            DestroyHandPrefab(location);
        }
        else
        {
            Debug.Log($"Dropping item from loadout: {selectedItem.itemName}");
            selectedItem.Remove();
        }
        
        droppedItem.DropItemReward(PlayerController.Instance.transform, droppedPayload);
        itemTracker.AssignItemToTracker(droppedPayload, ItemLocation.World, -1, -1, dropObject);
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public void ToggleWeaponVisibility(bool active)
    {
        Debug.Log($"Toggle Weapon Visibility:{active}");   
        if (currentLoadout != null || !currentLoadout.IsEmpty())
        {
            var leftItem = currentLoadout.leftHandItem;
            var rightItem = currentLoadout.rightHandItem;
            
            if (leftItem != null && leftItem.isWeapon)
            {
                if (leftItem.weaponScript.weaponBase != null)
                {
                    leftItem.weaponScript.weaponBase?.ToggleWeaponState(!active);
                }
            }

            if (rightItem != null && rightItem.isWeapon)
            {
                if (rightItem.weaponScript.weaponBase != null)
                {
                    rightItem.weaponScript.weaponBase?.ToggleWeaponState(!active);
                }
            }
        }
        leftHand.gameObject.SetActive(active);
        rightHand.gameObject.SetActive(active);
    }

    public void ApplyLoadoutBonuses()
    {
        Debug.Log("LOADSAVE = Equipping and Applying Loadout bonuses");
        foreach (var loadout in loadoutByID.Values)
        {
            if (loadout == currentLoadout)
            {
                if (loadout.leftHandItem != null) Equip(ItemLocation.LeftHand, loadout.leftHandItem);
                if (loadout.rightHandItem != null) Equip(ItemLocation.RightHand, loadout.rightHandItem);
            }
        }
    }

    public void LoadWeaponsFromSave(PlayerState state)
    {
        InitializeComponents();
        
        Debug.Log("LOADSAVE = Applying saved items in Loadout");
        if (state.savedWeaponLoadouts.Count > 0)
        {
            ItemPayload leftHandItem;
            ItemPayload rightHandItem;
            
            foreach (var loadoutData in state.savedWeaponLoadouts)
            {
                if (loadoutData.leftItemName != "EMPTY")
                {
                    leftHandItem = new ItemPayload()
                    {
                        weaponScript = loadoutData.isLeftAWeapon ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.leftItemName) : null,
                        itemScript = loadoutData.isLeftAWeapon ? null : ItemDatabase.CreateItem(loadoutData.leftItemName),
                        isWeapon = loadoutData.isLeftAWeapon
                    };
                    itemTracker.AssignItemToTracker(leftHandItem, loadoutData.isLeftInActiveHand ? ItemLocation.LeftHand : ItemLocation.RightHand);
                }
                else leftHandItem = null;
                
                if (loadoutData.rightItemName != "EMPTY")
                {
                    rightHandItem = new ItemPayload()
                    {
                        weaponScript = loadoutData.isRightAWeapon ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.rightItemName) : null,
                        itemScript = loadoutData.isRightAWeapon ? null : ItemDatabase.CreateItem(loadoutData.leftItemName),
                        isWeapon = loadoutData.isRightAWeapon
                    };
                    itemTracker.AssignItemToTracker(leftHandItem, loadoutData.isRightInActiveHand ? ItemLocation.LeftHand : ItemLocation.RightHand);
                }
                else rightHandItem = null;
                    
                var newLoadout = new PlayerLoadout(leftHandItem, rightHandItem);
                newLoadout.loadoutIndex = loadoutData.loadoutID;
                loadoutByID.Add(newLoadout.loadoutIndex, newLoadout);
            }
            
            currentLoadoutID = state.currentLoadoutID;
            currentLoadout = GetLoadoutByID(currentLoadoutID);
        }
    }
    
    public List<PlayerLoadout> GetPlayerLoadouts()
    {
        List<PlayerLoadout> savedPlayerLoadouts = new List<PlayerLoadout>();
        foreach (var savedLoadout in loadoutByID.Values)
        {
            savedPlayerLoadouts.Add(savedLoadout);
        }
        return savedPlayerLoadouts;
    }

    public void ResetLoadouts()
    {
        var currentLoadout = GetLoadoutByID(currentLoadoutID);
        if (!currentLoadout.IsHandFree(true))
        {
            if (currentLoadout.leftHandItem.isWeapon) DestroyHandPrefab(ItemLocation.LeftHand);
        }
        if (!currentLoadout.IsHandFree(false))
        {
            if (currentLoadout.rightHandItem.isWeapon) DestroyHandPrefab(ItemLocation.RightHand);
        }
        loadoutByID.Clear();
        Debug.Log("All loadouts have been reset.");
    }
}

/* public WeaponInstance GetWeaponByID(string id) => weaponByID.GetValueOrDefault(id);
       public BaseItem GetItemByID(string id) => itemByID.GetValueOrDefault(id);
       public PlayerLoadout GetLoadoutByID(int id) => loadoutByID.TryGetValue(id, out var loadout) ? loadout : null;
       public List<PlayerLoadout> GetPlayerLoadouts() => playerLoadouts;
       public int GetCurrentLoadoutID() => currentLoadoutID;
       public PlayerLoadout GetCurrentLoadout() => currentLoadout;
       public bool HasWeapon(string name) => weaponsByName.ContainsKey(name);
       //public bool HasItem(string name) => itemsByName.ContainsKey(name);
       public bool IsLoadoutActive(int loadoutID) => currentLoadoutID == loadoutID;
       public void UnassignItemFromLoadout(PlayerLoadout loadout, bool isLeftHand) => loadout.UnassignWeapon(isLeftHand);
       
       public bool HasWeaponMatch(string weaponName, Rarity rarity)
       {
           if (weaponsByName.TryGetValue(name, out var weaponList))
           {
               var match = weaponList.FirstOrDefault(w => w.weaponTitle == weaponName && w.rarity == rarity);
               if (match != null)
                   return true;
           }
           return false;
       }
       
       public bool HasItemMatch(string itemName, Rarity rarity)
       {
           if (itemsByName.TryGetValue(name, out var itemList))
           {
               var match = itemList.FirstOrDefault(w => w.itemName == itemName && w.currentRarity == rarity);
               if (match != null)
                   return true;
           }
           return false;
       }
       
       public string GetWeaponMatchID(string weaponName, Rarity rarity)
       {
           if (weaponsByName.TryGetValue(name, out var weaponList))
           {
               var match = weaponList.FirstOrDefault(w => w.weaponTitle == weaponName && w.rarity == rarity);
               if (match != null)
                   return match.uniqueID;
           }
           return null;
       }
       
       public string GetItemMatchID(string itemName, Rarity rarity)
       {
           if (itemsByName.TryGetValue(name, out var itemList))
           {
               var match = itemList.FirstOrDefault(w => w.itemName == itemName && w.currentRarity == rarity);
               if (match != null)
                   return match.uniqueID;
           }
           return null;
       }
       
       public Rarity GetOwnedItemRarity(string checkedItem, bool isWeapon)
       {
           if (isWeapon)
           {
               if (weaponsByName.TryGetValue(checkedItem, out var weaponList))
               {
                   var match = weaponList.FirstOrDefault(w => w.weaponTitle == checkedItem);
                   if (match != null)
                       return match.rarity;
               }
           }
           else
           {
               if (itemsByName.TryGetValue(checkedItem, out var itemList))
               {
                   var match = itemList.FirstOrDefault(w => w.itemName == checkedItem);
                   if (match != null)
                       return match.currentRarity;
               }
           }
           return Rarity.Common;
       }
       
       public bool isThereSpaceInLoadouts()
       {
           foreach (var loadout in playerLoadouts)
           {
               if (loadout.IsHandFree(true) || loadout.IsHandFree(false))
                   return true;
           }
           return false;
       }
       
       void Awake()
       {
           if (Instance == null) Instance = this;
           else Destroy(gameObject);
           if (leftHand == null || rightHand == null)
           {
               Debug.LogError("Left or right hand is not set in WeaponManager!");
           }
           else
           {
               Debug.LogError("Hands are fine");
           }
           EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
       }
   
       private void OnDestroy()
       {
           EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
       }
       
       private void OnSceneLoaded(string scene)
       {
           Debug.Log($"Scene loaded: leftHand = {leftHand}, rightHand = {rightHand}");
           if (scene == "GameScene")
           {
               GameData gameData = SaveManager.LoadGame();
               if (gameData.isNewGame) 
               {
                   Debug.Log("New Game!");
                   InitializeComponents();
                   
                   if (playerLoadouts.Count == 0)
                   {
                       AddNewLoadout(2);
                       Debug.Log("Default loadout created at game start.");
                   }
               }
           }
       }
       
       public void InitializeComponents()
       {
           if (isInitialized) return;
           isInitialized = true;
           playerController = GetComponent<PlayerController>();
           playerAttributes = GetComponent<AttributeManager>();
           enemyDetector = GetComponent<EnemyDetector>();
           
           leftHand = GameObject.FindWithTag("LeftHand");
           rightHand = GameObject.FindWithTag("RightHand");
       }
       
       public void AddNewLoadout(int amount)
       {
           for (int i = 0; i < amount; i++)
           {
               int id = currentLoadoutID++;
               loadoutAmount++;
               var loadout = new PlayerLoadout();
               loadout.loadoutID = id;
               playerLoadouts.Add(loadout);
               loadoutByID[id] = loadout;
               Debug.Log($"Added new empty loadout! loadout: {id}");
           }
           currentLoadoutID = 1; //reset, just for player reference, NOT ACTUAL ID.
           currentLoadout = GetLoadoutByID(currentLoadoutID);
           LoadoutUIManager.Instance.UpdateLoadoutUI();
       }
       
       public int GetIndexByLoadoutID(int id)
       {
           for (int i = 0; i < playerLoadouts.Count; i++)
           {
               if (playerLoadouts[i].loadoutID == id)
                   return i;
           }
           return -1;
       }
       
       public void TrySwitchToNextLoadout()
       {
           int currentIndex = GetIndexByLoadoutID(currentLoadoutID);
           if (playerLoadouts.Count <= 1) return;
   
           int nextIndex = (currentIndex + 1) % playerLoadouts.Count;
           SwitchLoadoutByID(playerLoadouts[nextIndex].loadoutID);
       }
       
       public void TrySwitchToPreviousLoadout()
       {
           int currentIndex = GetIndexByLoadoutID(currentLoadoutID);
           if (playerLoadouts.Count <= 1) return;
   
           int prevIndex = (currentIndex - 1 + playerLoadouts.Count) % playerLoadouts.Count;
           SwitchLoadoutByID(playerLoadouts[prevIndex].loadoutID);
       }
       
       public void SwitchLoadoutByID(int targetID)
       {
           if (isSwapping || currentLoadoutID == targetID) return;
           Debug.Log($"Switching loadout!{targetID}, {currentLoadoutID}");
           StartCoroutine(SwitchLoadout(targetID));
           Debug.Log($"Switching loadout!{targetID}");
       }
       
       private void Update()
       {
           if (playerLoadouts.Count <= 1 || isDisabled || isSwapping) return;
   
           if (Input.GetKeyDown(KeyCode.E)) TrySwitchToNextLoadout();
           if (Input.GetKeyDown(KeyCode.Q)) TrySwitchToPreviousLoadout();
       }
       
       public void AddItemToLoadout(ItemPayload item)
       {
           if (currentLoadout.IsHandFree(true)) Equip(true, currentLoadout, item);
           else if (currentLoadout.IsHandFree(false)) Equip(false, currentLoadout, item);
           else FindSpaceInStoredLoadouts(item);
       }
       
       public void UpgradeItemInLoadouts(string itemID, bool isWeapon)
       {
           if (isWeapon)
           {
               if (weaponByID.TryGetValue(itemID, out WeaponInstance weapon))
               {
                   if (weapon.loadoutID == currentLoadoutID)
                   {
                       Debug.Log($"Upgrading ACTIVE Weapon: {weapon.weaponTitle}");
                       LoadoutUIManager.Instance.UpgradeActiveItem(weapon.isLeftHand);
                       return;
                   }
                   Debug.Log($"Upgrading STORED Weapon: {weapon.weaponTitle}");
                   LoadoutUIManager.Instance.UpgradeItemInStoredLoadout(weapon.loadoutID, weapon.isLeftHand);
               }
           }
           else
           {
               if (itemByID.TryGetValue(itemID, out BaseItem item))
               {
                   if (item.loadoutID == currentLoadoutID)
                   {
                       Debug.Log($"Upgrading ACTIVE Item: {item.itemName}");
                       LoadoutUIManager.Instance.UpgradeActiveItem(item.isLeftHand);
                       return;
                   }
                   Debug.Log($"Upgrading STORED Item: {item.itemName}");
                   LoadoutUIManager.Instance.UpgradeItemInStoredLoadout(item.loadoutID, item.isLeftHand);
               }
           }
       }
   
       private void FindSpaceInStoredLoadouts(ItemPayload item)
       { 
           foreach (var loadout in playerLoadouts)
           {
               if (loadout.loadoutID == currentLoadoutID) continue;
               if (loadout.IsHandFree(true))
               {
                   StartCoroutine(SwitchLoadoutAndEquip(item, loadout.loadoutID, true));
               }
               else if (loadout.IsHandFree(false))
               {
                   StartCoroutine(SwitchLoadoutAndEquip(item, loadout.loadoutID, false));
               }
           }
       }
       
       private IEnumerator SwitchLoadoutAndEquip(ItemPayload item, int loadoutID, bool isLeftHand)
       {
           yield return StartCoroutine(SwitchLoadout(loadoutID));
           var loadout = GetLoadoutByID(currentLoadoutID);
           Equip(isLeftHand, loadout, item);
   
           ItemUI.draggedItem = null;
       }
       
       public void Equip(bool isLeftHand, PlayerLoadout loadout, ItemPayload item)
       {
           if (item.isWeapon)
           {
               WeaponInstance weaponInstance = item.weaponScript;
               if (!weaponByID.ContainsKey(weaponInstance.uniqueID)) //check if new or if swapping from currently owned items
               {
                   weaponByID.Add(weaponInstance.uniqueID, weaponInstance);
                   if (!weaponsByName.ContainsKey(weaponInstance.weaponTitle))
                   {
                       Debug.Log($"No existing group for {weaponInstance.weaponTitle}, creating new.");
                       weaponsByName[weaponInstance.weaponTitle] = new List<WeaponInstance>();
                   }
                   else
                   {
                       Debug.Log($"Found group for {weaponInstance.weaponTitle}, adding to it.");
                       weaponsByName[weaponInstance.weaponTitle].Add(weaponInstance);
                   }
               }
               
               loadout.AssignWeapon(item, isLeftHand);
               InitializeWeaponInLoadout(weaponInstance, isLeftHand ? leftHand.transform : rightHand.transform, isLeftHand); //visuals
               EventManager.Instance.TriggerEvent("ItemInteraction", weaponInstance.weaponTitle);
               Debug.Log($"Equipped new item: {weaponInstance.weaponTitle}, weapon count:{weaponByID.Count}");
           }
           else
           {
               BaseItem baseItem  = item.itemScript;
               if (!itemByID.ContainsKey(baseItem.uniqueID))
               {
                   itemByID.Add(baseItem.uniqueID, baseItem);
                   if (!itemsByName.ContainsKey(baseItem.itemName))
                   {
                       Debug.Log($"No existing group for {baseItem.itemName}, creating new.");
                       itemsByName[baseItem.itemName] = new List<BaseItem>();
                   }
                   else
                   {
                       Debug.Log($"Found group for {baseItem.itemName}, adding to it.");
                       itemsByName[baseItem.itemName].Add(baseItem);
                   }
               }
               
               loadout.AssignWeapon(item, isLeftHand);
               InitializeItemInLoadout(baseItem, isLeftHand ? leftHand.transform : rightHand.transform, isLeftHand); //visuals
               EventManager.Instance.TriggerEvent("ItemInteraction", baseItem.itemName);
               Debug.Log($"Equipped new item: {baseItem.itemName}, item count:{itemByID.Count}");
           }
           
           LoadoutUIManager.Instance.UpdateLoadoutUI();
       }
       
       private IEnumerator SwitchLoadout(int targetID)
       {
           isSwapping = true;
           ToggleWeaponVisibility(false);
           if (!currentLoadout.IsHandFree(true) && currentLoadout.leftHandItem.isWeapon) DestroyHandPrefab(true);
           if (!currentLoadout.IsHandFree(false) && currentLoadout.rightHandItem.isWeapon) DestroyHandPrefab(false);
           
           yield return new WaitForSeconds(0.25f);
   
           currentLoadoutID = targetID;
           var newLoadout = GetLoadoutByID(currentLoadoutID);
           if (newLoadout == null)
           {
               Debug.LogError($"Target loadout (ID: {currentLoadoutID}) not found!");
               isSwapping = false;
               yield break;
           }
   
           if (!newLoadout.IsHandFree(true))
           {
               if (newLoadout.leftHandItem.isWeapon)
               {
                   InitializeWeaponInLoadout(newLoadout.leftHandItem.weaponScript, leftHand.transform, true);
               }
               else
               {
                   InitializeItemInLoadout(newLoadout.leftHandItem.itemScript, leftHand.transform, true);
               }
           }
           if (!newLoadout.IsHandFree(false))
           {
               if (newLoadout.rightHandItem.isWeapon)
               {
                   InitializeWeaponInLoadout(newLoadout.rightHandItem.weaponScript, rightHand.transform, false);
               }
               else
               {
                   InitializeItemInLoadout(newLoadout.rightHandItem.itemScript, rightHand.transform, false);
               }
           }
           
           isSwapping = false;
           currentLoadout = newLoadout;
           ToggleWeaponVisibility(true);
           LoadoutUIManager.Instance.UpdateLoadoutUI();
           Debug.Log($"Switched to loadout {currentLoadoutID}. New weapons: Left - {newLoadout.leftHandItem}, Right - {newLoadout.rightHandItem}");
       }
   
       private void EquipLoadoutItems(PlayerLoadout newLoadout)
       {
           //if (LoadoutUIManager)
           if (!newLoadout.IsHandFree(true)) Equip(true, newLoadout, newLoadout.leftHandItem);
           if (!newLoadout.IsHandFree(false)) Equip(false, newLoadout, newLoadout.rightHandItem);
       }
       
       private void InitializeWeaponInLoadout(WeaponInstance weaponInstance, Transform equippedLocation, bool isLeftHand)
       {
           Debug.Log($"Initializing weapon: {weaponInstance.weaponTitle} in left?: {isLeftHand}");
           weaponInstance.loadoutID = currentLoadoutID;
           weaponInstance.isLeftHand = isLeftHand;
           
           GameObject weaponObject = Instantiate(weaponInstance.weaponPrefab, equippedLocation);
           WeaponBase newWeapon = weaponObject.GetComponent<WeaponBase>();
           if (newWeapon == null) return;
           
           weaponInstance.weaponBase = newWeapon;
           newWeapon.weaponInstance = weaponInstance;
           newWeapon.equippedLocation = equippedLocation;
           newWeapon.enemyDetector = enemyDetector;
           newWeapon.playerController = playerController;
           newWeapon.playerAttributes = playerAttributes;
           newWeapon.InitializeWeapon(isLeftHand);
           
           Debug.Log($"total weapons: {weaponByID.Count}");
       }
       
       private void InitializeItemInLoadout(BaseItem item, Transform equippedLocation, bool isLeftHand)
       {
           Debug.Log($"Initializing item: {item.itemName}");
           item.loadoutID = currentLoadoutID;
           item.isLeftHand = isLeftHand;
           item.Apply(playerAttributes);
           if (item.itemType == ItemType.Hand)
           {
               //Trigger active item initializations
           }
           Debug.Log($"NOW total items: {itemByID.Count}");
       }
       
       public void HandleLoadoutPayload(UISwapPayload payload, bool targetIsLeft, bool isFromInventory = false, bool toEmptySlot = false)
       {
           if (currentLoadout == null || isHandlingUI) return;
           isHandlingUI = true;
           ItemPayload sourcePayload = payload.sourceItem;
           ItemPayload targetPayload = payload.targetItem;
           bool isSourceAWeapon = sourcePayload.isWeapon;
           bool isTargetAWeapon = targetPayload.isWeapon;
   
           Debug.Log($"HandleUIPayload: Assigning to {(targetIsLeft ? "Left Hand" : "Right Hand")} | " +
                     $"Source: {(sourcePayload.isWeapon ? sourcePayload.weaponScript?.weaponTitle : sourcePayload.itemScript?.itemName)} | " +
                     $"Target: {(targetPayload.isWeapon ? targetPayload.weaponScript?.weaponTitle : targetPayload.itemScript?.itemName)}");
           
           if (toEmptySlot) 
           {
               if (isFromInventory)
               {
                   Debug.Log("Placing item from inventory into empty hand slot");
                   InventoryManager.Instance.DeleteItemFromInventory(isSourceAWeapon ? sourcePayload.weaponScript : null,
                       isSourceAWeapon ? null : sourcePayload.itemScript);
                   Equip(targetIsLeft, currentLoadout, sourcePayload);
               }
               else
               {
                   if (isSourceAWeapon) DestroyHandPrefab(!targetIsLeft);
                   if (isSourceAWeapon) InitializeWeaponInLoadout(sourcePayload.weaponScript, targetIsLeft ? leftHand.transform : rightHand.transform, targetIsLeft);
                   if (!isSourceAWeapon) InitializeItemInLoadout(sourcePayload.itemScript, targetIsLeft ? leftHand.transform : rightHand.transform, targetIsLeft);
                   
                   currentLoadout.leftHandItem = targetIsLeft ? sourcePayload : null;
                   currentLoadout.rightHandItem = targetIsLeft ? null : sourcePayload;
                   LoadoutUIManager.Instance.UpdateLoadoutUI();
               }
           }
           else
           {
               if (isFromInventory)
               {
                   Debug.Log("Trading hand item w ivnentory item");
                   if (isTargetAWeapon) DestroyHandPrefab(targetIsLeft);
                   ClearHandData(targetPayload.isWeapon ? targetPayload.weaponScript : null, targetPayload.isWeapon ? null : targetPayload.itemScript);
                   
                   int sourceSlotIndex = InventoryUI.Instance.GetInventorySlotByItemID(isSourceAWeapon ? sourcePayload.weaponScript?.uniqueID : sourcePayload.itemScript?.uniqueID).slotIndex;
                   InventoryManager.Instance.DeleteItemFromInventory(isSourceAWeapon ? sourcePayload.weaponScript : null,
                       isSourceAWeapon ? null : sourcePayload.itemScript);
                   
                   Equip(targetIsLeft, currentLoadout, sourcePayload);
                   InventoryManager.Instance.AddItemToInventory(targetPayload, sourceSlotIndex);
               }
               else
               {
                   if (isSourceAWeapon) DestroyHandPrefab(!targetIsLeft);
                   if (isTargetAWeapon) DestroyHandPrefab(targetIsLeft);
                   if (isSourceAWeapon) InitializeWeaponInLoadout(sourcePayload.weaponScript, targetIsLeft ? leftHand.transform : rightHand.transform, targetIsLeft);
                   if (isTargetAWeapon) InitializeWeaponInLoadout(targetPayload.weaponScript, targetIsLeft ? rightHand.transform : leftHand.transform, targetIsLeft);
                   if (!isSourceAWeapon) InitializeItemInLoadout(sourcePayload.itemScript, targetIsLeft ? leftHand.transform : rightHand.transform, targetIsLeft);
                   if (!isTargetAWeapon) InitializeItemInLoadout(targetPayload.itemScript, targetIsLeft ? rightHand.transform : leftHand.transform, targetIsLeft);
                   
                   Debug.Log($"source going to:{(targetIsLeft ? "Left Hand" : "Right Hand")}");
                   currentLoadout.leftHandItem = targetIsLeft ? sourcePayload : targetPayload;
                   currentLoadout.rightHandItem = targetIsLeft ? targetPayload : sourcePayload;
                   LoadoutUIManager.Instance.UpdateLoadoutUI();
               }
           }
           isHandlingUI = false;
       }
       
       public void DestroyHandPrefab(bool isLeftHand)
       {
           Transform parent = isLeftHand ? leftHand.transform : rightHand.transform;
           int childCount = parent.childCount;
   
           for (int i = childCount - 1; i >= 0; i--)
           {
               Debug.Log($"Destroying: {parent.GetChild(i).gameObject.name}");
               Destroy(parent.GetChild(i).gameObject);
           }
       }
       
       public void ClearHandData(WeaponInstance weapon = null, BaseItem item = null)
       {
           if (weapon != null)
           {
               weaponByID.Remove(weapon.uniqueID); // Remove unique ID identifier
               weaponsByName[weapon.weaponTitle]?.Remove(weapon); // Remove name from any existing group
               if (weaponsByName[weapon.weaponTitle].Count == 0)
                   weaponsByName.Remove(weapon.weaponTitle); // Clean empty group
           }
           else if (item != null)
           {
               itemByID.Remove(item.uniqueID); // Remove unique ID identifier
               itemsByName[item.itemName]?.Remove(item); // Remove name from any existing group
               if (itemsByName[item.itemName].Count == 0)
                   itemsByName.Remove(item.itemName); // Clean empty group
           }
       }
       
       public void DeleteItemFromLoadouts(WeaponInstance weapon = null, BaseItem item = null)
       {
           bool isWeapon = item == null;
           if (isWeapon)
           {
               Debug.Log($"Deleting weapon from loadout: {weapon?.weaponTitle}");
               WeaponInstance targetWeapon = GetWeaponByID(weapon?.uniqueID);
               ClearHandData(targetWeapon);
               UnassignItemFromLoadout(currentLoadout, targetWeapon.isLeftHand);
               DestroyHandPrefab(targetWeapon.isLeftHand);
           }
           else
           {
               Debug.Log($"Deleting item from loadout: {item?.itemName}");
               BaseItem targetItem = GetItemByID(item.uniqueID);
               ClearHandData(null, targetItem);
               UnassignItemFromLoadout(currentLoadout, targetItem.isLeftHand);
               targetItem.Remove(AttributeManager.Instance);
           }
           LoadoutUIManager.Instance.UpdateLoadoutUI();
       }
       
       public void DropHandObject(WeaponInstance selectedWeapon = null, BaseItem selectedItem = null)
       {
           isHandlingUI = true;
           var loadout = GetLoadoutByID(currentLoadoutID);
           bool isWeapon = selectedItem == null;
           
           Vector2 dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
           GameObject dropObject = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
           ItemDrop droppedItem = dropObject.GetComponent<ItemDrop>();
           
           if (isWeapon && selectedWeapon != null)
           {
               ClearHandData(selectedWeapon);
               UnassignItemFromLoadout(loadout, selectedWeapon.isLeftHand);
               DestroyHandPrefab(selectedWeapon.isLeftHand);
               
               droppedItem.DropItemReward(PlayerController.Instance.transform, null, selectedWeapon);
               selectedWeapon.WeaponDiscarded();
               EventManager.Instance.TriggerEvent("ItemInteraction", selectedWeapon.weaponTitle);
               WeaponDatabase.Instance.RegisterActiveWeapon(selectedWeapon.weaponTitle);
           }
           else if (!isWeapon)
           {
               ClearHandData(null, selectedItem);
               UnassignItemFromLoadout(loadout, selectedItem.isLeftHand);
               
               droppedItem.DropItemReward(PlayerController.Instance.transform, selectedItem);
               selectedItem.Remove(AttributeManager.Instance);
               EventManager.Instance.TriggerEvent("ItemInteraction", selectedItem.itemName);
               ItemDatabase.Instance.RegisterActiveItem(selectedItem.itemName);
           }
           LoadoutUIManager.Instance.UpdateLoadoutUI();
           isHandlingUI = false;
       }
       
       public List<string> GetWeaponNames()
       {
           List<string> weaponKeys = new List<string>();
           foreach (var weapon in weaponByID)
           {
               if (weapon.Key != null)
               {
                   weaponKeys.Add(weapon.Key);
               }
           }
           return weaponKeys;
       }
       
       public void LoadWeaponsFromSave(PlayerState state)
       {
           InitializeComponents();
           ResetLoadouts();
           
           if (state.savedWeaponLoadouts.Count > 0)
           {
               ItemPayload leftHandItem;
               ItemPayload rightHandItem;
               
               foreach (var loadoutData in state.savedWeaponLoadouts)
               {
                   if (loadoutData.leftItemName != null)
                   {
                       leftHandItem = new ItemPayload()
                       {
                           weaponScript = loadoutData.isLeftAWeapon ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.leftItemName, loadoutData.leftItemRarity) : null,
                           itemScript = loadoutData.isLeftAWeapon ? null : ItemFactory.CreateItem(loadoutData.leftItemName, ItemDatabase.Instance.LoadItemIcon(loadoutData.leftItemName), loadoutData.leftItemRarity),
                           rarity = loadoutData.leftItemRarity,
                           isWeapon = loadoutData.isLeftAWeapon
                       };
                   }
                   else leftHandItem = null;
                   
                   if (loadoutData.rightItemName != null)
                   {
                       rightHandItem = new ItemPayload()
                       {
                           weaponScript = loadoutData.isRightAWeapon ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.rightItemName, loadoutData.rightItemRarity) : null,
                           itemScript = loadoutData.isRightAWeapon ? null : ItemFactory.CreateItem(loadoutData.leftItemName, ItemDatabase.Instance.LoadItemIcon(loadoutData.rightItemName), loadoutData.rightItemRarity),
                           rarity = loadoutData.rightItemRarity,
                           isWeapon = loadoutData.isRightAWeapon
                       };
                   }
                   else
                   {
                       rightHandItem = null;
                   }
                       
                   var newLoadout = new PlayerLoadout(leftHandItem, rightHandItem);
                   newLoadout.loadoutID = loadoutData.loadoutID;
                   playerLoadouts.Add(newLoadout);
                   loadoutByID[newLoadout.loadoutID] = newLoadout;
               }
               var activeID = state.currentLoadoutID;
               currentLoadoutID = playerLoadouts.Exists(l => l.loadoutID == activeID) ? activeID : playerLoadouts[0].loadoutID;
               EquipLoadoutItems(GetLoadoutByID(currentLoadoutID));
           }
       }
       
       public void ResetLoadouts()
       {
           playerLoadouts.Clear();
           loadoutByID.Clear();
           weaponByID.Clear();
           itemByID.Clear();
           weaponsByName.Clear();
           itemsByName.Clear();
           var currentLoadout = GetLoadoutByID(currentLoadoutID);
           if (currentLoadout.leftHandItem.isWeapon) DestroyHandPrefab(currentLoadout.leftHandItem.weaponScript.isLeftHand);
           if (currentLoadout.rightHandItem.isWeapon) DestroyHandPrefab(currentLoadout.rightHandItem.weaponScript.isLeftHand);
           Debug.Log("All loadouts and weapons have been reset.");
       }
   
       public bool GetHandPositionByID(string id, bool isWeapon)
       {
           if (isWeapon)
           {
               if (weaponByID.TryGetValue(id, out var weaponInstance))
               {
                   if (id == weaponInstance.uniqueID) return weaponInstance.isLeftHand;
               }
           }
           else
           {
               if (itemByID.TryGetValue(id, out var itemBase))
               {
                   if (id == itemBase.uniqueID) return itemBase.isLeftHand;
               }
           }
           return false;
       }
       
       public void ToggleWeaponVisibility(bool active)
       {
           Debug.Log($"Toggle :{active}");   
           if (currentLoadout != null || !currentLoadout.IsEmpty())
           {
               var leftItem = currentLoadout.leftHandItem;
               var rightItem = currentLoadout.rightHandItem;
               
               if (leftItem != null && leftItem.isWeapon)
               {
                   if (leftItem.weaponScript.weaponBase != null)
                   {
                       leftItem.weaponScript.weaponBase?.ToggleWeaponState(!active);
                   }
               }
   
               if (rightItem != null && rightItem.isWeapon)
               {
                   if (rightItem.weaponScript.weaponBase != null)
                   {
                       rightItem.weaponScript.weaponBase?.ToggleWeaponState(!active);
                   }
               }
           }
           
           leftHand.gameObject.SetActive(active);
           rightHand.gameObject.SetActive(active);
       }
   }*/