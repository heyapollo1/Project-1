using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponDatabase : BaseManager
{
    public static WeaponDatabase Instance { get; private set; }
    public override int Priority => 30;

    public List<WeaponData> weaponDataList = new List<WeaponData>();

    protected override void OnInitialize()
    {
        LoadWeaponsIntoDatabase();
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (var weapon in weaponDataList)
        {
            Debug.Log($"Added Weapon to Databse: {weapon.weaponTitle}");
        }
    }

    private void LoadWeaponsIntoDatabase()
    {
        weaponDataList.Clear();

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Weapons");
        foreach (var prefab in loadedPrefabs)
        {
            WeaponBase weapon = prefab.GetComponent<WeaponBase>();

            if (weapon != null && weapon.weaponData != null) 
            {
                weaponDataList.Add(weapon.weaponData);
                Debug.Log($"weapon: {weapon.weaponData.weaponTitle}");
            }
            else
            {
                Debug.LogWarning($"Skipping {prefab.name}.No WeaponData found!");
            }
        }
    }
    
    public WeaponInstance CreateRandomWeapon(StageBracket bracket)
    {
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
            Debug.Log($"Attempt {attempts}: Weapon {selectedWeapon.weaponTitle} already exists! Trying a new one...");
            if (!ItemTracker.Instance.DoesItemExist(randomWeaponName))
            {
                break; // Found a valid weapon, exit loop
            }
        }
        while (true);
        
        WeaponInstance rolledWeapon = new WeaponInstance(selectedWeapon);
        Debug.Log($"Weapon: {rolledWeapon.weaponTitle}");
        return rolledWeapon;
    }

    public WeaponInstance CreateWeaponInstance(string weaponName)
    {
        WeaponData weaponData = GetWeaponDataByName(weaponName);
        if (weaponData == null)
        {
            Debug.LogError($"Failed to get weapon instance: {weaponName} not found!");
            return null;
        }
        Debug.Log($" weapon: {weaponData.weaponTitle}. Creating instance...");
        WeaponInstance weaponInstance = new WeaponInstance(weaponData);
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
            if (weapon.weaponTitle == weaponName) return weapon;
        }
        Debug.LogWarning($"Weapon '{weaponName}' not found in the database.");
        return null;
    }
}