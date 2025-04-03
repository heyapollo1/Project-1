using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponDatabase : BaseManager
{
    public static WeaponDatabase Instance { get; private set; }
    public override int Priority => 30;

    public List<WeaponBase> weaponPrefabs = new List<WeaponBase>();
    public List<WeaponData> weaponDataList = new List<WeaponData>();
    private HashSet<string> activeWeapons= new HashSet<string>();

    protected override void OnInitialize()
    {
        LoadWeapons();
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (var prefab in weaponPrefabs)
        {
            Debug.Log($"Prefab in list: {prefab.name}");
        }
    }

    private void LoadWeapons()
    {
        weaponPrefabs.Clear();
        weaponDataList.Clear();

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Weapons");

        foreach (var prefab in loadedPrefabs)
        {
            WeaponBase weapon = prefab.GetComponent<WeaponBase>();

            if (weapon != null && weapon.weaponData != null) 
            {
                weaponPrefabs.Add(weapon);
                weaponDataList.Add(weapon.weaponData);
                Debug.Log($"weapon: {weapon.weaponData.weaponTitle}");
            }
            else
            {
                Debug.LogWarning($"Skipping {prefab.name}.No WeaponData found!");
            }
        }
    }
    
    public WeaponInstance GetRandomWeapon(StageBracket bracket)
    {
        Rarity rolledRarity = RollRarity(bracket);
        Rarity weaponDefaultRarity;
        List<WeaponData> eligibleWeapons = new List<WeaponData>(weaponDataList);
        
        string randomWeaponName;
        WeaponData selectedWeapon = null;
        int attempts = 0;
        const int maxAttempts = 50;
        
        do
        {
            if (attempts >= maxAttempts)
            {
                Debug.LogError("Too many attempts, no viable weapons found.");
                return null;
            }
            
            attempts++;
            int randomIndex = Random.Range(0, eligibleWeapons.Count);
            selectedWeapon = eligibleWeapons[randomIndex];
            randomWeaponName = selectedWeapon.weaponTitle;
            weaponDefaultRarity = selectedWeapon.defaultRarity;
            Debug.Log($"Attempt {attempts}: Weapon {selectedWeapon.weaponTitle} is already owned! Trying a new one...");
            if (!activeWeapons.Contains(selectedWeapon.weaponTitle) && IsWeaponAllowed(randomWeaponName, weaponDefaultRarity, rolledRarity))
            {
                break; // Found a valid weapon, exit loop
            }
        }
        while (true);
        
        if (WeaponManager.Instance.HasWeapon(randomWeaponName))
        {
            Rarity? playerRarity = WeaponManager.Instance.GetOwnedWeaponRarity(randomWeaponName);
            rolledRarity = playerRarity.Value;
        }
        else
        {
            Debug.LogWarning($"{selectedWeapon.weaponTitle}, rarity: ({selectedWeapon.defaultRarity}) is not owned by player");
        }
        
        WeaponInstance rolledWeapon = new WeaponInstance(selectedWeapon, rolledRarity);
        activeWeapons.Add(rolledWeapon.weaponTitle);

        Debug.Log($"Weapon: {rolledWeapon.weaponTitle} ({rolledRarity})");
        return rolledWeapon;
    }
    
    private bool IsWeaponAllowed(string weaponName, Rarity weaponDefaultRarity, Rarity rolledRarity)
    {
        if (rolledRarity < weaponDefaultRarity)
        {
            Debug.LogWarning($"{weaponName}: Default Rarity ({weaponDefaultRarity}) is higher than rolled rarity ({rolledRarity}).");
            return false;
        }
        Debug.LogWarning($"{weaponName} found, accessing..");
        return true;
    }
    
    private Rarity RollRarity(StageBracket bracket)
    {
        Debug.LogWarning("Rolling weapon rarity...");
        Dictionary<Rarity, float> chances = RarityChances.GetRarityBracketChances(bracket);
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var rarityChance in chances)
        {
            cumulativeProbability += rarityChance.Value;
            if (randomValue <= cumulativeProbability)
            {
                return rarityChance.Key;
            }
        }
        Debug.LogWarning("Rarity roll failed, defaulting to Bronze.");
        return Rarity.Common;
    }

    public WeaponInstance CreateWeaponInstance(string weaponName, Rarity rarity)
    {
        Debug.Log($"🔎 Searching for weapon: {weaponName}");
        WeaponData weaponData = GetWeaponDataByName(weaponName);
        if (weaponData == null)
        {
            Debug.LogError($"Failed to get weapon instance: {weaponName} not found!");
            return null;
        }
        Debug.Log($" weapon: {weaponData.weaponTitle}. Creating instance...");
        WeaponInstance weaponInstance = new WeaponInstance(weaponData, rarity);
        if (weaponInstance.weaponPrefab == null)
        {
            Debug.LogError($"ERROR: WeaponInstance prefab is NULL after creation for {weaponInstance.weaponTitle}!");
        }
        return weaponInstance;
    }
    
    public WeaponData GetWeaponDataByName(string weaponName)
    {
        foreach (var weapon in weaponDataList)
        {
            if (weapon.weaponTitle == weaponName)
            {
                return weapon;
            }
        }
        Debug.LogWarning($"Weapon '{weaponName}' not found in the database.");
        return null;
    }
    
    public void RegisterActiveWeapon(string weaponName)
    {
        if (!activeWeapons.Contains(weaponName))
        {
            activeWeapons.Add(weaponName);
        }
    }
    
    public void UnregisterActiveWeapon(string weaponName)
    {
        if (activeWeapons.Contains(weaponName))
        {
            activeWeapons.Remove(weaponName);
        }
    }

    public List<string> GetActiveWeaponNames()
    {
        return activeWeapons.ToList();
    }
   
}
