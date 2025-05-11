using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    
    private List<UpgradeData> playerUpgrades = new List<UpgradeData>();
    private AttributeManager playerAttributes;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        EventManager.Instance.StartListening("UpgradeSelected", ApplyUpgrade);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("UpgradeSelected", ApplyUpgrade);
    }

    public void ApplyUpgrade(UpgradeData selectedUpgrade)
    {
        playerAttributes = AttributeManager.Instance;

        var existingUpgrade = playerUpgrades.Find(u => u.upgradeName == selectedUpgrade.upgradeName);

        if (existingUpgrade != null)
        {
            existingUpgrade.ScaleUpgrade(playerAttributes);
        }
        else
        {
            playerUpgrades.Add(selectedUpgrade);
            selectedUpgrade.Apply(playerAttributes);
        }
    }
    
    public void ApplyAllUpgrades()
    {
        Debug.Log("Applying player upgrades");
        foreach (var upgrade in playerUpgrades)
        {
            Debug.Log($"Applying: {upgrade}");
            ApplyUpgrade(upgrade);
        }
    }
    
    public void LoadUpgradesFromSave(PlayerState playerState) //loading save
    {
        playerUpgrades.Clear();
        foreach (var upgradeName in playerState.upgrades)
        {
            Debug.Log($" Upgrade loaded: {upgradeName}");
            var upgrade = UpgradeDatabase.Instance.GetUpgradeByName(upgradeName);
            if (upgrade != null)
            {
                playerUpgrades.Add(upgrade); //add that shit
            }
            else
            {
                Debug.LogWarning($"Item '{upgradeName}' not found in the database!");
            }
        }
    }
    
    public List<string> GetUpgradeNames() //getting save
    {
        Debug.Log("Getting upgrades from save.");
        List<string> upgradeNames = new List<string>();
        foreach (var upgrade in playerUpgrades)
        {
            Debug.Log($"Loading Upgrade: {upgrade}");
            upgradeNames.Add(upgrade.upgradeName);
        }
        return upgradeNames;
    }
}
