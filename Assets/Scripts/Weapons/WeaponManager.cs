
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    private GameObject leftHand; // attach left weapon here
    private GameObject rightHand; // attach right weapon here
    
    public WeaponInstance activeLeftWeapon; // Left-hand weapon
    public WeaponInstance activeRightWeapon; // Right-hand weapon
    public GameObject weaponDropPrefab;
    
    [HideInInspector] public List<WeaponLoadout> playerLoadouts = new List<WeaponLoadout>();
    private Dictionary<int, WeaponLoadout> loadoutByID = new Dictionary<int, WeaponLoadout>();
    private List<WeaponInstance> playerWeapons = new List<WeaponInstance>();
    
    private PlayerController playerController;
    private AttributeManager playerAttributes;
    private EnemyDetector enemyDetector;
    
    private int currentLoadoutID = 0;
    private int nextLoadoutID = 0;
    private bool isSwapping = false;
    private bool isDisabled = false;
    private bool isInitialized = false;
    
    private int loadoutAmount = 0;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        EventManager.Instance.StartListening("EnablePlayerWeapons", EnableWeapons);
        EventManager.Instance.StartListening("DisablePlayerWeapons", DisableWeapons);
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
    }

    private void OnDestroy()
    {
        ResetWeaponLoadouts();
        EventManager.Instance.StopListening("EnablePlayerWeapons", EnableWeapons);
        EventManager.Instance.StopListening("DisablePlayerWeapons", DisableWeapons);
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
    }
    
    private void OnSceneLoaded(string scene)
    {
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
        
        if (leftHand == null || rightHand == null)
        {
            Debug.LogError("WeaponManager: LeftHand or RightHand pivot not found in scene!");
        }
    }
    
    public void AddNewLoadout(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int id = currentLoadoutID++;
            loadoutAmount++;
            var loadout = new WeaponLoadout(null, Rarity.Common, null, Rarity.Common);
            loadout.loadoutID = id;
            playerLoadouts.Add(loadout);
            loadoutByID[id] = loadout;
            Debug.Log($"Added new empty loadout! loadout: {id}");
        }

        currentLoadoutID = 1; //reset
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public WeaponLoadout GetLoadoutByID(int id)
    {
        return loadoutByID.TryGetValue(id, out var loadout) ? loadout : null;
    }
    
    public int GetCurrentLoadoutID() => currentLoadoutID;
    
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
    
    public bool TryFindWeaponInLoadouts(WeaponInstance weapon, out int loadoutID, out bool isLeft)
    {
        foreach (WeaponInstance weaponInstance in playerWeapons)
        {
            Debug.Log($"Found weapon: {weapon.weaponTitle} . Count: {playerWeapons.Count}");
            if (weapon.weaponTitle == weaponInstance.weaponTitle)
            {
                loadoutID = weaponInstance.loadoutID;
                isLeft = weaponInstance.isLeftHand;
                Debug.Log($"Found weapon: {weapon.weaponTitle}, loadout: {loadoutID}, isLeft: {isLeft}");
                return loadoutID >= 0;
            }
        }
        
        loadoutID = -1;
        isLeft = false;
        return false;  
    }
    
    public void AcquireWeapon(WeaponInstance selectedWeapon, bool isUpgrade = false)
    {
        Rarity weaponRarity = selectedWeapon.rarity;
        if (isUpgrade)
        {
            Debug.Log("Upgrading check");
            foreach (WeaponInstance weapon in playerWeapons)
            {
                Debug.Log("Upgrading check2");
                if (weapon.weaponTitle == selectedWeapon.weaponTitle && weapon.rarity == selectedWeapon.rarity)
                {
                    if (weapon.loadoutID == currentLoadoutID)
                    {
                        Debug.Log($"Upgrading ACTIVE Weapon");
                        LoadoutUIManager.Instance.UpgradeWeaponInActiveLoadout(weapon.isLeftHand);
                        return;
                    }
                    Debug.Log($"Upgrading STORED Weapon");
                    StoredLoadoutUI storedUI = LoadoutUIManager.Instance.GetStoredLoadoutUIByID(weapon.loadoutID);
                    storedUI?.UpgradeWeaponInStoredLoadout(weapon.isLeftHand);
                    return;
                }
            }
            return;
        }
        
        WeaponLoadout currentLoadout = GetLoadoutByID(currentLoadoutID);
        if (currentLoadout.leftWeapon == null)
        {
            EquipWeapon(selectedWeapon, weaponRarity, true);
            Debug.Log($"Equipped LEFT: {selectedWeapon.weaponTitle}");
            return;
        }

        if (currentLoadout.rightWeapon == null)
        {
            EquipWeapon(selectedWeapon, weaponRarity, false);
            Debug.Log($"Equipped RIGHT: {selectedWeapon.weaponTitle}");
            return;
        }
        
        foreach (var loadout in playerLoadouts)
        {
            if (loadout.loadoutID == currentLoadoutID) continue;

            if (loadout.leftWeapon == null)
            {
                Debug.Log($"Switching to Loadout {loadout.loadoutID} for LEFT hand.");
                StartCoroutine(SwitchLoadout(loadout.loadoutID, selectedWeapon, true));
                return;
            }

            if (loadout.rightWeapon == null)
            {
                Debug.Log($"Switching to Loadout {loadout.loadoutID} for RIGHT hand.");
                StartCoroutine(SwitchLoadout(loadout.loadoutID, selectedWeapon, false));
                return;
            }
        }
        Debug.LogWarning("No space to equip weapon.");
    }
    
    public void EquipWeapon(WeaponInstance weapon, Rarity rarity, bool isLeftHand)
    {
        if (weapon == null) return;

        if (isLeftHand)
        {
            Debug.Log($"Equipping active weapon {weapon.weaponTitle} in {isLeftHand}, LEFT");
            activeLeftWeapon = weapon;
            GetLoadoutByID(currentLoadoutID).leftWeapon = weapon;
            InitializeWeaponInstance(weapon, leftHand.transform, true);
        }
        else
        {
            Debug.Log($"Equipping active weapon {weapon.weaponTitle} in {isLeftHand}, RIGHT");
            activeRightWeapon = weapon;
            GetLoadoutByID(currentLoadoutID).rightWeapon = weapon;
            InitializeWeaponInstance(weapon, rightHand.transform, false);
        }
        Debug.Log($"weapon count B4:{playerWeapons.Count}");
        playerWeapons.Add(weapon);
        EventManager.Instance.TriggerEvent("WeaponInteraction", weapon.weaponTitle);
        Debug.Log($"Equipped new LEFT weapon: {weapon.weaponTitle}, weapon count:{playerWeapons.Count}");
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    private IEnumerator SwitchLoadout(int targetID, WeaponInstance newWeapon = null, bool isLeftHand = true)
    {
        //if (playerLoadouts.Count <= 1 || isSwapping || targetIndex == currentLoadoutIndex) yield break;
        isSwapping = true;
        
        var currentLoadout = GetLoadoutByID(currentLoadoutID);
        if (currentLoadout == null)
        {
            Debug.LogError($"Current loadout (ID: {currentLoadoutID}) not found!");
            isSwapping = false;
            yield break;
        }
        
        if (activeLeftWeapon != null)
        {
            activeLeftWeapon.isLeftHand = true;
            currentLoadout.AssignWeapon(activeLeftWeapon, true);
            if (activeLeftWeapon.weaponBase != null)
            {
                activeLeftWeapon.weaponBase.gameObject.SetActive(false);
                activeLeftWeapon.DisableWeapon();
            }
        }

        if (activeRightWeapon != null)
        {
            activeRightWeapon.isLeftHand = false;
            currentLoadout.AssignWeapon(activeRightWeapon, false);
            if (activeRightWeapon.weaponBase != null)
            {
                activeRightWeapon.weaponBase.gameObject.SetActive(false);
                activeRightWeapon.DisableWeapon();
            }
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

        activeLeftWeapon = null;
        activeRightWeapon = null;

        if (newWeapon != null)
        {
            if (isLeftHand)
            {
                Debug.Log($"weapon count B4:{playerWeapons.Count}");
                newLoadout.leftWeapon = newWeapon;
                playerWeapons.Add(newWeapon);
                EventManager.Instance.TriggerEvent("WeaponInteraction", newWeapon.weaponTitle);
                Debug.Log($"Equipped new LEFT weapon: {newWeapon.weaponTitle}, weapon count:{playerWeapons.Count}");
            }
            else
            {
                Debug.Log($"weapon count B4:{playerWeapons.Count}");
                newLoadout.rightWeapon = newWeapon;
                playerWeapons.Add(newWeapon);
                EventManager.Instance.TriggerEvent("WeaponInteraction", newWeapon.weaponTitle);
                Debug.Log($"Equipped new RIGHT weapon: {newWeapon.weaponTitle} weapon count:{playerWeapons.Count}");
            }
        }

        if (newLoadout.leftWeapon != null)
        {
            activeLeftWeapon = newLoadout.leftWeapon;
            InitializeWeaponInstance(activeLeftWeapon, leftHand.transform, true);
            activeLeftWeapon.weaponBase?.gameObject.SetActive(true);
            activeLeftWeapon.EnableWeapon();
        }

        if (newLoadout.rightWeapon != null)
        {
            activeRightWeapon = newLoadout.rightWeapon;
            InitializeWeaponInstance(activeRightWeapon, rightHand.transform, false);
            activeRightWeapon.weaponBase?.gameObject.SetActive(true);
            activeRightWeapon.EnableWeapon();
        }
        
        Debug.Log($"Switched to loadout {currentLoadoutID}. New weapons: Left - {newLoadout.leftWeapon?.weaponTitle}, Right - {newLoadout.rightWeapon?.weaponTitle}");
        LoadoutUIManager.Instance.UpdateLoadoutUI();
        isSwapping = false;
    }
    
    private void InitializeWeaponInstance(WeaponInstance weaponInstance, Transform equippedLocation, bool isLeftHand)
    {
        var currentLoadout = GetLoadoutByID(currentLoadoutID);
        currentLoadout.AssignWeapon(weaponInstance, isLeftHand);
        weaponInstance.loadoutID = currentLoadoutID;
        weaponInstance.isLeftHand = isLeftHand;
        if (isLeftHand) activeLeftWeapon = weaponInstance;
        else activeRightWeapon = weaponInstance;
        
        GameObject weaponObject = Instantiate(weaponInstance.weaponPrefab, equippedLocation);
        //Debug.Log($"Instantiated: {weaponInstance.weaponTitle} at {equippedLocation}, total weapons: {playerWeapons.Count}");
            
        WeaponBase newWeapon = weaponObject.GetComponent<WeaponBase>();
        if (newWeapon == null)
        {
            Debug.LogError($"{weaponInstance.weaponTitle} prefab has no WeaponBase component!");
            return;
        }
        weaponInstance.weaponBase = newWeapon;
            
        newWeapon.weaponInstance = weaponInstance;
        newWeapon.equippedLocation = equippedLocation;
        newWeapon.enemyDetector = enemyDetector;
        newWeapon.playerController = playerController;
        newWeapon.playerAttributes = playerAttributes;
        
        Debug.Log($"is Left? {isLeftHand}");
        newWeapon.InitializeWeapon(isLeftHand); 
        //playerWeapons.Add(weaponInstance);
        Debug.Log($"NOW total weapons: {playerWeapons.Count}");
    }
    
    public void SwapWeaponHands(WeaponInstance sourceWeapon, WeaponInstance targetWeapon, bool sourceHand, bool targetHand)
    {
        if (playerLoadouts.Count == 0 || sourceWeapon == null || targetWeapon == null) return;
        int loadoutID = sourceWeapon.loadoutID;
        var loadout = GetLoadoutByID(loadoutID);
        if (loadout == null)
        {
            Debug.LogError("SwapWeaponHands: Invalid loadoutID on weapon");
            return;
        }
        Debug.Log($"Swapping {sourceWeapon.weaponTitle}-{sourceHand} with {targetWeapon.weaponTitle}-{targetHand}");
        DestroyWeaponInstance(sourceWeapon);
        DestroyWeaponInstance(targetWeapon);
        
        if (sourceHand)
        {
            activeLeftWeapon = targetWeapon;
            activeRightWeapon = sourceWeapon;
            loadout.leftWeapon = targetWeapon;
            loadout.rightWeapon = sourceWeapon;
        }
        else
        {
            activeLeftWeapon = sourceWeapon;
            activeRightWeapon = targetWeapon;
            loadout.leftWeapon = sourceWeapon;
            loadout.rightWeapon = targetWeapon;
        }
        
        activeLeftWeapon.loadoutID = loadoutID;
        activeRightWeapon.loadoutID = loadoutID;
        activeLeftWeapon.isLeftHand = true;
        activeRightWeapon.isLeftHand = false;
        
        InitializeWeaponInstance(activeLeftWeapon, leftHand.transform, true);
        InitializeWeaponInstance(activeRightWeapon, rightHand.transform, false);
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public void PlaceWeaponInEmptyHand(WeaponInstance weapon, bool toLeftHand)
    {
        if (playerLoadouts.Count == 0 || weapon == null) return;
        Debug.Log($"Reassigning {weapon.weaponTitle} to {(toLeftHand ? "Left" : "Right")} Hand");
        int loadoutID = weapon.loadoutID;
        var loadout = GetLoadoutByID(weapon.loadoutID);
        
        //weapon.isLeftHand = !toLeftHand; //TEMP for proper cleanup
        DestroyWeaponInstance(weapon);
        //weapon.isLeftHand = toLeftHand; 
        
        if (toLeftHand)
        {
            activeLeftWeapon = weapon;
            loadout.leftWeapon = weapon;
        }
        else
        {
            activeRightWeapon = weapon;
            loadout.rightWeapon = weapon;
        }

        weapon.loadoutID = loadoutID;
        weapon.isLeftHand = toLeftHand;
    
        Transform handTransform = toLeftHand ? leftHand.transform : rightHand.transform;
        InitializeWeaponInstance(weapon, handTransform, toLeftHand);

        Debug.Log($"Equipped {weapon.weaponTitle} to {(toLeftHand ? "Left" : "Right")} Hand at {handTransform}");
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public void DiscardWeapon(WeaponInstance selectedWeapon)
    {
        if (selectedWeapon == null) return;
        DestroyWeaponInstance(selectedWeapon);
        Vector2 dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        GameObject dropped = Instantiate(weaponDropPrefab, dropPosition, Quaternion.identity);
        WeaponDrop droppedWeapon = dropped.GetComponent<WeaponDrop>();
        if (droppedWeapon != null)
        {
            droppedWeapon.DropWeaponReward(selectedWeapon, PlayerController.Instance.transform);
        }
        Debug.Log($"Dropped {selectedWeapon.weaponTitle} into world at {dropPosition}.");
        
        selectedWeapon.WeaponDiscarded();
        EventManager.Instance.TriggerEvent("WeaponInteraction", selectedWeapon.weaponTitle);
        WeaponDatabase.Instance.RegisterActiveWeapon(selectedWeapon.weaponTitle);
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public void DestroyWeaponInstance(WeaponInstance weapon)
    {
        var loadout = GetLoadoutByID(weapon.loadoutID);
        if (activeLeftWeapon == weapon) activeLeftWeapon = null;
        if (activeRightWeapon == weapon) activeRightWeapon = null;
        loadout.UnassignWeapon(weapon.isLeftHand);
        Debug.Log($" Count B4: {playerWeapons.Count}");
        playerWeapons.Remove(weapon);
        if (weapon.weaponBase?.gameObject != null) //Destroy object in hand
            Destroy(weapon.weaponBase.gameObject);
        Debug.Log($" Count: {playerWeapons.Count}");

        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public bool IsLoadoutActive(int loadoutID)
    {
        return currentLoadoutID == loadoutID;
    }
    
    public List<WeaponLoadout> GetPlayerLoadouts()
    {
        return playerLoadouts;
    }
    
    public void LoadWeaponsFromSave(PlayerState state)
    {
        InitializeComponents();
        ResetWeaponLoadouts();

        if (state.savedWeaponLoadouts.Count > 0)
        {
            foreach (var loadoutData in state.savedWeaponLoadouts)
            {
                WeaponInstance leftWeaponInstance = !string.IsNullOrEmpty(loadoutData.leftWeapon) 
                    ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.leftWeapon, loadoutData.leftWeaponRarity) 
                    : null;

                WeaponInstance rightWeaponInstance = !string.IsNullOrEmpty(loadoutData.rightWeapon) 
                    ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.rightWeapon, loadoutData.rightWeaponRarity) 
                    : null;

                var newLoadout = new WeaponLoadout(leftWeaponInstance, loadoutData.leftWeaponRarity, rightWeaponInstance, loadoutData.rightWeaponRarity);
                newLoadout.loadoutID = loadoutData.loadoutID;
            
                playerLoadouts.Add(newLoadout);
                loadoutByID[newLoadout.loadoutID] = newLoadout;

                Debug.Log($"Loaded Loadout ID {loadoutData.loadoutID}: L-{loadoutData.leftWeapon} | R-{loadoutData.rightWeapon}");
            }

            var activeID = state.currentLoadoutID;
            currentLoadoutID = playerLoadouts.Exists(l => l.loadoutID == activeID) ? activeID : playerLoadouts[0].loadoutID;
            EquipSavedLoadout(GetLoadoutByID(currentLoadoutID));
        }
    }

    private void EquipSavedLoadout(WeaponLoadout loadout)
    {
        if (loadout == null) return;
        if (loadout.leftWeapon != null)
        {
            EquipWeapon(loadout.leftWeapon, loadout.leftWeaponRarity, true);
        }

        if (loadout.rightWeapon != null)
        {
            EquipWeapon(loadout.rightWeapon, loadout.rightWeaponRarity, false);
        }
        LoadoutUIManager.Instance.UpdateLoadoutUI();
        Debug.Log($"saved loadout: Left:{loadout.leftWeapon?.weaponTitle}, Right:{loadout.rightWeapon?.weaponTitle}");
    }
    
    public void ResetWeaponLoadouts()
    {
        playerLoadouts.Clear();
        loadoutByID.Clear();
        playerWeapons.Clear();
        if (activeLeftWeapon != null) Destroy(activeLeftWeapon.weaponPrefab);
        if (activeRightWeapon != null) Destroy(activeRightWeapon.weaponPrefab);
        Debug.Log("All loadouts and weapons have been reset.");
    }
    
    public bool IsThereSpaceForWeapons()
    {
        if (activeLeftWeapon == null || activeRightWeapon == null)
        {
            return true;
        }

        if (loadoutAmount > 1)
        {
            foreach (var loadout in playerLoadouts)
            {
                if (loadout.leftWeapon == null || loadout.rightWeapon == null)
                {
                    return true; 
                }
            }
        }

        return false;
    }
    
    public List<WeaponLoadoutData> GetLoadoutsForSave()
    {
        List<WeaponLoadoutData> saveData = new List<WeaponLoadoutData>();
        foreach (var loadout in playerLoadouts)
        {
            saveData.Add(new WeaponLoadoutData(loadout));
        }
        return saveData;
    }
    
    public List<string> GetWeaponNames()
    {
        List<string> weaponNames = new List<string>();
        foreach (var weapon in playerWeapons)
        {
            if (weapon != null && weapon.weaponTitle != null)
            {
                weaponNames.Add(weapon.weaponTitle);
            }
        }
        return weaponNames;
    }
    
    public bool HasWeapon(string weaponName)
    {
        if (weaponName == null)
        {
            Debug.LogError("HasWeapon() received NULL weapon name!");
            return false;
        }
        Debug.Log($"Checking for weapon: '{weaponName}' in inventory.");
        foreach (WeaponInstance weapon in playerWeapons)
        {
            if (weapon.weaponTitle == weaponName)
            {
                return true;
            }
        }
        return false;
    }
    
    public Rarity? GetOwnedWeaponRarity(string weaponName)
    {
        foreach (WeaponInstance weapon in playerWeapons)
        {
            if (weapon.weaponTitle == weaponName)
            {
                return weapon.rarity;
            }
        }
        return null;
    }
    
    public void DisableWeapons()
    {
        isDisabled = true;
        
        if (activeLeftWeapon != null) activeLeftWeapon.DisableWeapon();
        if (activeRightWeapon != null) activeRightWeapon.DisableWeapon();

        if (leftHand != null) leftHand.SetActive(false);
        if (rightHand != null) rightHand.SetActive(false);
    }

    public void EnableWeapons()
    {
        isDisabled = false;

        if (activeLeftWeapon != null) activeLeftWeapon.EnableWeapon();
        if (activeRightWeapon != null) activeRightWeapon.EnableWeapon();

        if (leftHand != null) leftHand.SetActive(true);
        if (rightHand != null) rightHand.SetActive(true);
    }
}

/*using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    private GameObject leftHand; // attach left weapon here
    private GameObject rightHand; // attach right weapon here
    
    public WeaponInstance activeLeftWeapon; // Left-hand weapon
    public WeaponInstance activeRightWeapon; // Right-hand weapon
    
    [HideInInspector] public List<WeaponLoadout> playerLoadouts = new List<WeaponLoadout>();
    private Dictionary<int, WeaponLoadout> loadoutByID = new Dictionary<int, WeaponLoadout>();
    private List<WeaponInstance> playerWeapons = new List<WeaponInstance>();
    
    private PlayerController playerController;
    private AttributeManager playerAttributes;
    private EnemyDetector enemyDetector;
    
    private int currentLoadoutID = 0;
    private int nextLoadoutID = 0;
    private bool isSwapping = false;
    private bool isDisabled = false;
    private bool isInitialized = false;
    
    private int loadoutAmount = 0;
    //private int currentWeaponIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        EventManager.Instance.StartListening("EnablePlayerWeapons", EnableWeapons);
        EventManager.Instance.StartListening("DisablePlayerWeapons", DisableWeapons);
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
    }

    private void OnDestroy()
    {
        ResetWeaponLoadouts();
        EventManager.Instance.StopListening("EnablePlayerWeapons", EnableWeapons);
        EventManager.Instance.StopListening("DisablePlayerWeapons", DisableWeapons);
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
    }
    
    private void OnSceneLoaded(string scene)
    {
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
        
        if (leftHand == null || rightHand == null)
        {
            Debug.LogError("WeaponManager: LeftHand or RightHand pivot not found in scene!");
        }
    }
    
    public void AddNewLoadout(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int id = currentLoadoutID++;
            loadoutAmount++;
            var loadout = new WeaponLoadout(null, Rarity.Common, null, Rarity.Common);
            loadout.loadoutID = id;
            playerLoadouts.Add(loadout);
            Debug.Log($"Added new empty loadout! Total loadouts: {playerLoadouts.Count}");
        }
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public WeaponLoadout GetLoadoutByID(int id)
    {
        return loadoutByID.TryGetValue(id, out var loadout) ? loadout : null;
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
        StartCoroutine(SwitchLoadoutRoutine(targetID));
    }

    
    void Update()
    {
        if (playerLoadouts.Count <= 1 || isDisabled) return;
        
        int targetLoadoutIndex = -1;
        
        if (!isSwapping)
        {
            if (Input.GetKeyDown(KeyCode.E)) // Switch to Next Loadout
            {
                targetLoadoutIndex = (currentLoadoutIndex + 1) % playerLoadouts.Count;
                Debug.Log($"Swapping to next loadout: {targetLoadoutIndex}");
            }
            else if (Input.GetKeyDown(KeyCode.Q)) // Switch to Previous Loadout
            {
                targetLoadoutIndex = (currentLoadoutIndex - 1 + playerLoadouts.Count) % playerLoadouts.Count;
                Debug.Log($"Swapping to previous loadout: {targetLoadoutIndex}");
            }
        }
        
        if (targetLoadoutIndex != -1 && targetLoadoutIndex != currentLoadoutIndex)
        {
            StartCoroutine(SwitchLoadout(targetLoadoutIndex));
        }
    }
    
    public bool TryFindWeaponInLoadouts(WeaponInstance targetWeapon, out int loadoutIndex, out bool isLeftHand)
    {
        //List<WeaponLoadout> loadouts = GetPlayerLoadouts();

        for (int i = 0; i < playerLoadouts.Count; i++)
        {
            WeaponLoadout loadout = playerLoadouts[i];

            if (loadout.leftWeapon != null &&
                loadout.leftWeapon.weaponTitle == targetWeapon.weaponTitle)
            {
                loadoutIndex = i;
                isLeftHand = true;
                return true;
            }

            if (loadout.rightWeapon != null &&
                loadout.rightWeapon.weaponTitle == targetWeapon.weaponTitle)
            {
                loadoutIndex = i;
                isLeftHand = false;
                return true;
            }
        }

        loadoutIndex = -1;
        isLeftHand = false;
        return false;
    }
    
    public void AcquireWeapon(WeaponInstance selectedWeapon, bool isUpgrade = false)
    {
        Rarity weaponRarity = selectedWeapon.rarity;
        if (isUpgrade)
        {
            if (TryFindWeaponInLoadouts(selectedWeapon, out int loadoutIndex, out bool isLeftHand))
            {
                if (GetCurrentLoadoutIndex() != loadoutIndex)
                {
                    Debug.Log($"Current Loadout Index: {currentLoadoutIndex},{loadoutIndex}");
                    Debug.Log($"Upgraded {selectedWeapon.weaponTitle} to {selectedWeapon.rarity}.");
                    LoadoutUIManager.Instance.UpgradeWeaponInActiveLoadout(selectedWeapon, loadoutIndex, isLeftHand);
                }
                Debug.Log($"Current Loadout Index: {currentLoadoutIndex},{loadoutIndex}");
                Debug.Log($"Upgraded {selectedWeapon.weaponTitle} to {selectedWeapon.rarity}.");
                StoredLoadoutUI storedLoadout = LoadoutUIManager.Instance.GetStoredLoadoutUIByIndex(loadoutIndex);
                storedLoadout.UpgradeWeaponInStoredLoadout(selectedWeapon, isLeftHand);
                //LoadoutUIManager.Instance.UpgradeWeaponInStoredLoadout(selectedWeapon, loadoutIndex, isLeftHand);
            }
            Debug.Log($"Upgraded {selectedWeapon.weaponTitle} to {selectedWeapon.rarity}.");
            return;
        }
        
        if (playerLoadouts[currentLoadoutIndex].leftWeapon == null)
        {
            Debug.Log($"Acquired LEFT: {selectedWeapon.weaponTitle}, rarity:{weaponRarity}");
            EquipWeapon(selectedWeapon, weaponRarity, true);
            return;
        }
        if (playerLoadouts[currentLoadoutIndex].rightWeapon == null)
        {
            Debug.Log($"Acquired RIGHT: {selectedWeapon.weaponTitle}, rarity:{weaponRarity}");
            EquipWeapon(selectedWeapon, weaponRarity, false);
            return;
        }

        for (int i = 0; i < playerLoadouts.Count; i++)
        {
            if (i == currentLoadoutIndex) continue;

            if (playerLoadouts[i].leftWeapon == null)
            {
                Debug.Log($"Equipping weapon in Loadout {i} (Left Hand). Switching to it.");
                StartCoroutine(SwitchLoadout(i, selectedWeapon, true)); // Switch & equip
                return;
            }
            else if (playerLoadouts[i].rightWeapon == null)
            {
                Debug.Log($"Equipping weapon in Loadout {i} (Right Hand). Switching to it.");
                StartCoroutine(SwitchLoadout(i, selectedWeapon, false)); // Switch & equip
                return;
            }
        }
        
        Debug.LogWarning($"No space to place weapon.");
    }
    
    public void EquipWeapon(WeaponInstance weaponInstance, Rarity rarity, bool isLeftHand)
    {
        if (weaponInstance == null) return;
        Debug.Log($"Gun-{weaponInstance.weaponTitle} | rarity-{rarity}");
        
        if (isLeftHand)
        {
            Debug.Log($"Equipping active weapon {weaponInstance.weaponTitle} LEFT");
            activeLeftWeapon = weaponInstance;
            playerLoadouts[currentLoadoutIndex].leftWeapon = activeLeftWeapon;
        }
        else
        {
            Debug.Log($"Equipping active weapon {weaponInstance.weaponTitle} RIGHT");
            activeRightWeapon = weaponInstance;
            playerLoadouts[currentLoadoutIndex].rightWeapon = activeRightWeapon;
        }
        
        Transform equippedLocation = isLeftHand ? leftHand.transform : rightHand.transform;
        InitializeWeaponInstance(weaponInstance, equippedLocation, isLeftHand);
        LoadoutUIManager.Instance.UpdateLoadoutUI();
        Debug.Log($"Equipped {weaponInstance.weaponTitle} | Left: {activeLeftWeapon?.weaponTitle} | Right: {activeRightWeapon?.weaponTitle}");
    }
    
    private IEnumerator SwitchLoadout(int targetIndex, WeaponInstance newWeapon = null, bool isLeftHand = true)
    {
        //if (playerLoadouts.Count <= 1 || isSwapping || targetIndex == currentLoadoutIndex) yield break;
        isSwapping = true;
        foreach (WeaponLoadout loadout in playerLoadouts)
        {
            playerLoadouts[currentLoadoutIndex].AssignWeapon(activeLeftWeapon, true);
            playerLoadouts[currentLoadoutIndex].AssignWeapon(activeRightWeapon, false);
        }

        if (activeLeftWeapon != null)
        {
            Debug.Log($"Disabling current LEFT weapon: {activeLeftWeapon.weaponTitle} in current loadout: {currentLoadoutIndex}");
            activeLeftWeapon.weaponBase.gameObject.SetActive(false);
            activeLeftWeapon.DisableWeapon();
        }
        if (activeRightWeapon != null)
        {
            Debug.Log($"Disabling current RIGHT weapon: {activeRightWeapon.weaponTitle} in current loadout: {currentLoadoutIndex}");
            activeRightWeapon.weaponBase.gameObject.SetActive(false);
            activeRightWeapon.DisableWeapon();
        }
        
        yield return new WaitForSeconds(0.25f);
        
        currentLoadoutIndex = targetIndex;
        Debug.Log($"Switched to loadout {currentLoadoutIndex}");
        WeaponLoadout newLoadout = playerLoadouts[currentLoadoutIndex];
        activeLeftWeapon = null;
        activeRightWeapon = null;
        if (newWeapon != null)
        {
            if (isLeftHand)
            {
                activeLeftWeapon = newWeapon;
                playerLoadouts[currentLoadoutIndex].leftWeapon = newWeapon;
                InitializeWeaponInstance(activeLeftWeapon, leftHand.transform, true);
                Debug.Log($"Equipped new LEFT weapon: {newWeapon.weaponTitle}");
            }
            else
            {
                activeRightWeapon = newWeapon;
                playerLoadouts[currentLoadoutIndex].rightWeapon = newWeapon;
                InitializeWeaponInstance(activeRightWeapon, rightHand.transform, false);
                Debug.Log($"Equipped new RIGHT weapon: {newWeapon.weaponTitle}");
            }
        }
        else
        {
            if (newLoadout.leftWeapon != null)
            {
                activeLeftWeapon = newLoadout.leftWeapon;
                activeLeftWeapon.weaponBase.gameObject.SetActive(true);
                activeLeftWeapon.EnableWeapon();
            }
            if (newLoadout.rightWeapon != null)
            {
                activeRightWeapon = newLoadout.rightWeapon;
                activeRightWeapon.weaponBase.gameObject.SetActive(true);
                activeRightWeapon.EnableWeapon();
            }
        }
        
        Debug.Log($"Switched to loadout {currentLoadoutIndex}. New weapons: Left - {newLoadout.leftWeapon?.weaponTitle}, Right - {newLoadout.rightWeapon?.weaponTitle}");
        LoadoutUIManager.Instance.UpdateLoadoutUI();
        isSwapping = false;
    }
    
    private void InitializeWeaponInstance(WeaponInstance weaponInstance, Transform equippedLocation, bool isLeftHand)
    {
        playerLoadouts[currentLoadoutIndex].AssignWeapon(activeLeftWeapon, true);
        playerLoadouts[currentLoadoutIndex].AssignWeapon(activeRightWeapon, false);
        
        GameObject weaponObject = Instantiate(weaponInstance.weaponPrefab, equippedLocation);
        Debug.Log($"Instantiated: {weaponInstance.weaponTitle} at {equippedLocation}");
            
        WeaponBase newWeapon = weaponObject.GetComponent<WeaponBase>();
        weaponInstance.weaponBase = newWeapon;
            
        newWeapon.weaponInstance = weaponInstance;
        newWeapon.equippedLocation = equippedLocation;
        newWeapon.enemyDetector = enemyDetector;
        newWeapon.playerController = playerController;
        newWeapon.playerStats = playerAttributes;

        newWeapon.InitializeWeapon(isLeftHand); 
        playerWeapons.Add(weaponInstance);
    }
    
    public void SwapWeaponHands(WeaponInstance sourceWeapon, WeaponInstance targetWeapon, bool sourceHand, bool targetHand)
    {
        if (playerLoadouts.Count == 0) return;
        Debug.Log($"Swapping {sourceWeapon.weaponTitle}-{sourceHand} with {targetWeapon.weaponTitle}-{targetHand}");
        DestroyWeaponInHand(sourceHand);
        DestroyWeaponInHand(targetHand);
        
        if (sourceHand)
        {
            activeLeftWeapon = targetWeapon;
            activeRightWeapon = sourceWeapon;
        }
        else
        {
            activeLeftWeapon = sourceWeapon;
            activeRightWeapon = targetWeapon;
        }
        
        Transform sourceEquippedLocation = targetHand ? leftHand.transform : rightHand.transform;
        Transform targetEquippedLocation = sourceHand ? leftHand.transform : rightHand.transform;
        InitializeWeaponInstance(sourceWeapon, sourceEquippedLocation, targetHand);
        InitializeWeaponInstance(targetWeapon, targetEquippedLocation, sourceHand);
        Debug.Log($"New locations: {sourceWeapon.weaponTitle} at {sourceEquippedLocation} with {targetWeapon.weaponTitle} at {targetEquippedLocation}");
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public void PlaceWeaponInEmptyHand(WeaponInstance weapon, bool isLeftHand)
    {
        if (playerLoadouts.Count == 0) return;
        Debug.Log($"Re assigning {weapon.weaponTitle}-{isLeftHand}");
        DestroyWeaponInHand(isLeftHand);
        
        if (isLeftHand)
        {
            activeRightWeapon = weapon;
        }
        else
        {
            activeLeftWeapon = weapon;
        }
        
        Transform equippedLocation = isLeftHand ? rightHand.transform : leftHand.transform;
        InitializeWeaponInstance(weapon, equippedLocation, !isLeftHand);
        Debug.Log($"NEW WEAPON: {weapon.weaponTitle} - {equippedLocation}");
        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public void DestroyWeaponInHand(bool isLeftHand)
    {
        if (playerLoadouts.Count == 0) return;
        WeaponLoadout currentLoadout = playerLoadouts[currentLoadoutIndex];

        if (isLeftHand)
        {
            if (activeLeftWeapon != null)
            {
                Debug.Log($"Destroying LEFT weapon: {activeLeftWeapon.weaponTitle}");
                currentLoadout.leftWeapon = null;
                string oldKey = LoadoutUIManager.Instance.GenerateWeaponKey(currentLoadoutIndex, true, activeLeftWeapon.weaponTitle);
                LoadoutUIManager.Instance.RemoveWeaponKey(oldKey);
                Destroy(activeLeftWeapon.weaponBase.gameObject);
                activeLeftWeapon = null;
            }
        }
        else
        {
            if (activeRightWeapon != null)
            {
                Debug.Log($"Destroying RIGHT weapon: {activeRightWeapon.weaponTitle}");
                currentLoadout.rightWeapon = null;
                string oldKey = LoadoutUIManager.Instance.GenerateWeaponKey(currentLoadoutIndex, false, activeRightWeapon.weaponTitle);
                LoadoutUIManager.Instance.RemoveWeaponKey(oldKey);
                Destroy(activeRightWeapon.weaponBase.gameObject);
                activeRightWeapon = null;
            }
        }

        LoadoutUIManager.Instance.UpdateLoadoutUI();
    }
    
    public List<WeaponLoadout> GetPlayerLoadouts()
    {
        return playerLoadouts;
    }
    
    public void LoadWeaponsFromSave(PlayerState state)
    {
        InitializeComponents();
        ResetWeaponLoadouts();
       
        if (state.savedWeaponLoadouts.Count > 0)
        {
            foreach (var loadoutData in state.savedWeaponLoadouts)
            {
                WeaponInstance leftWeaponInstance = !string.IsNullOrEmpty(loadoutData.leftWeapon) 
                    ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.leftWeapon, loadoutData.leftWeaponRarity) 
                    : null;
            
                WeaponInstance rightWeaponInstance = !string.IsNullOrEmpty(loadoutData.rightWeapon) 
                    ? WeaponDatabase.Instance.CreateWeaponInstance(loadoutData.rightWeapon, loadoutData.rightWeaponRarity) 
                    : null;

                playerLoadouts.Add(new WeaponLoadout(leftWeaponInstance, loadoutData.leftWeaponRarity,
                    rightWeaponInstance, loadoutData.rightWeaponRarity));
                Debug.Log($"Loaded Loadout: Left-{loadoutData.leftWeapon} | Right-{loadoutData.rightWeapon}");
            }
            
            currentLoadoutIndex = Mathf.Clamp(state.currentLoadoutIndex, 0, playerLoadouts.Count - 1);
            Debug.Log($"✅ currentLoadoutIndex set to: {currentLoadoutIndex}");

            EquipSavedLoadout(playerLoadouts[currentLoadoutIndex]);
        }
    }
    
    private void EquipSavedLoadout(WeaponLoadout loadout)
    {
        if (loadout == null) return;
        if (loadout.leftWeapon != null)
        {
            EquipWeapon(loadout.leftWeapon, loadout.leftWeaponRarity, true);
        }

        if (loadout.rightWeapon != null)
        {
            EquipWeapon(loadout.rightWeapon, loadout.rightWeaponRarity, false);
        }
        LoadoutUIManager.Instance.UpdateLoadoutUI();
        Debug.Log($"saved loadout: Left:{loadout.leftWeapon?.weaponTitle}, Right:{loadout.rightWeapon?.weaponTitle}");
    }
    
    public void ResetWeaponLoadouts()
    {
        if (activeLeftWeapon != null) Destroy(activeLeftWeapon.weaponPrefab);
        if (activeRightWeapon != null) Destroy(activeRightWeapon.weaponPrefab);
        
        playerWeapons.Clear();
        playerLoadouts.Clear();
        Debug.Log("All loadouts and weapons have been reset.");
    }
    
    public bool IsThereSpaceForWeapons()
    {
        if (activeLeftWeapon == null || activeRightWeapon == null)
        {
            return true;
        }

        if (loadoutAmount > 1)
        {
            foreach (var loadout in playerLoadouts)
            {
                if (loadout.leftWeapon == null || loadout.rightWeapon == null)
                {
                    return true; 
                }
            }
        }

        return false;
    }
    
    public List<string> GetWeaponNames()
    {
        List<string> weaponNames = new List<string>();
        foreach (var weapon in playerWeapons)
        {
            if (weapon != null && weapon.weaponTitle != null)
            {
                weaponNames.Add(weapon.weaponTitle);
            }
        }
        return weaponNames;
    }
    
    public bool HasWeapon(WeaponInstance weaponInstance)
    {
        foreach (WeaponInstance weapon in playerWeapons)
        {
            if (weapon.weaponTitle == weaponInstance.weaponTitle)
            {
                return true;
            }
        }
        return false;
    }
    
    public int GetCurrentLoadoutIndex()
    {
        Debug.Log($"Current Loadout Index: {currentLoadoutIndex}");
        return currentLoadoutIndex;
    }
    
    public void DisableWeapons()
    {
        isDisabled = true;
        
        if (activeLeftWeapon != null) activeLeftWeapon.DisableWeapon();
        if (activeRightWeapon != null) activeRightWeapon.DisableWeapon();

        if (leftHand != null) leftHand.SetActive(false);
        if (rightHand != null) rightHand.SetActive(false);
    }

    public void EnableWeapons()
    {
        isDisabled = false;

        if (activeLeftWeapon != null) activeLeftWeapon.EnableWeapon();
        if (activeRightWeapon != null) activeRightWeapon.EnableWeapon();

        if (leftHand != null) leftHand.SetActive(true);
        if (rightHand != null) rightHand.SetActive(true);
    }
}*/
