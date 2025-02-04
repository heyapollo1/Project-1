using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : BaseManager
{
    public static WeaponDatabase Instance { get; private set; }
    public override int Priority => 30;

    public List<WeaponBase> weaponPrefabs = new List<WeaponBase>();
    public List<WeaponData> weaponDataList = new List<WeaponData>();
    private HashSet<WeaponData> activeWeapons= new HashSet<WeaponData>();

    protected override void OnInitialize()
    {
        //EventManager.Instance.StartListening("ShowAbilityChoices", ShowAbilityChoices);
        //EventManager.Instance.StartListening("RequestAbilityReroll", ShowAbilityChoices);
        LoadWeapons();
    }

    private void OnDestroy()
    {
        //EventManager.Instance.StopListening("ShowUpgradeChoices", ShowAbilityChoices);
        //EventManager.Instance.StopListening("RequestUpgradeReroll", ShowAbilityChoices);
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

            if (weapon != null)
            {
                weaponPrefabs.Add(weapon);

                if (weapon.weaponData != null)
                {
                    weaponDataList.Add(weapon.weaponData);
                    Debug.Log($"Loaded weapon: {weapon.weaponData.weaponTitle}");
                }
                else
                {
                    Debug.LogWarning($"{prefab.name} is missing weaponData!");
                }
            }
        }
    }

    public WeaponData GetRandomWeapon()
    {
        // Create list of eligible items
        List<WeaponData> eligibleWeapons = new List<WeaponData>(weaponDataList);

        eligibleWeapons.RemoveAll(weapon => activeWeapons.Contains(weapon) || WeaponManager.Instance.HasWeapon(weapon));
        Debug.LogWarning($"Getting random weapon");
        if (eligibleWeapons.Count == 0)
        {
            Debug.LogError($"no available weapons");
            return null;
        }

        int randomIndex = Random.Range(0, eligibleWeapons.Count);
        WeaponData selectedWeapon = eligibleWeapons[randomIndex];

        // Mark the item as active
        activeWeapons.Add(selectedWeapon);
        Debug.Log($"returning weapon: {selectedWeapon}");
        return selectedWeapon;
    }

    public bool AreThereWeapons()
    {
        return activeWeapons != null && activeWeapons.Count > 0;
    }

    public WeaponData GetWeapon(string weaponName)
    {
        List<WeaponData> eligibleWeapons = new List<WeaponData>(weaponDataList);

        eligibleWeapons.RemoveAll(weapon => activeWeapons.Contains(weapon) || WeaponManager.Instance.HasWeapon(weapon));

        if (eligibleWeapons.Count == 0)
        {
            return null;
        }

        foreach (WeaponData weapon in eligibleWeapons)
        {
            if (weapon.weaponTitle == weaponName)
            {
                return weapon;
            }
        }
        return null;
    }
    /*public List<PlayerAbilityData> GenerateAbilityChoices(float count)
    {
        List<PlayerAbilityData> randomAbilities = new List<PlayerAbilityData>();
        List<PlayerAbilityData> availableAbilities = new List<PlayerAbilityData>(weaponDataList);

        List<PlayerAbilityData> acquiredAbilities = QueryAcquiredAbilities();
        availableAbilities.RemoveAll(a => acquiredAbilities.Contains(a));
        Debug.LogWarning("showing ability choices");
        for (int i = 0; i < count; i++)
        {
            if (availableAbilities.Count == 0) break;

            int randomIndex = Random.Range(0, availableAbilities.Count);
            randomAbilities.Add(availableAbilities[randomIndex]);
            availableAbilities.RemoveAt(randomIndex);  // Prevent duplicates
        }
        return randomAbilities;
    }

    private void ShowAbilityChoices(float choiceAmount)
    {
        Debug.LogWarning("showing ability choices");
        var abilityChoices = GenerateAbilityChoices(choiceAmount);
        EventManager.Instance.TriggerEvent("AbilityChoicesReady", abilityChoices);
    }

    private List<PlayerAbilityData> QueryAcquiredAbilities()
    {
        List<PlayerAbilityData> acquiredAbilities = new List<PlayerAbilityData>();

        EventManager.Instance.TriggerEvent("QueryAcquiredAbilities", acquiredAbilities);

        return acquiredAbilities;
    }*/
}
