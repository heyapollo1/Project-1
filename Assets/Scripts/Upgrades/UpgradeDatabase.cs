using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDatabase : BaseManager
{
    public static UpgradeDatabase Instance { get; private set; }
    public override int Priority => 30;

    public List<UpgradeData> availableUpgrades = new List<UpgradeData>();  // Pool of all potential upgrades

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("ShowUpgradeChoices", ShowUpgradeChoices);
        LoadUpgrades();
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log($"Loading {availableUpgrades.Count} upgrades in UpgradeDatabase");
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("ShowUpgradeChoices", ShowUpgradeChoices);
    }

    private void LoadUpgrades()
    {
        Sprite healthIcon = Resources.Load<Sprite>("Icons/HealthUpgrade");
        Sprite cooldownRateIcon = Resources.Load<Sprite>("Icons/CooldownRateUpgrade");
        Sprite damageIcon = Resources.Load<Sprite>("Icons/DamageUpgrade");
        Sprite movementSpeedIcon = Resources.Load<Sprite>("Icons/MovementSpeedUpgrade");
        Sprite criticalHitChanceIcon = Resources.Load<Sprite>("Icons/CriticalHitChanceUpgrade");
        Sprite criticalHitDamageIcon = Resources.Load<Sprite>("Icons/CriticalHitDamageUpgrade");
        Sprite rangeIcon = Resources.Load<Sprite>("Icons/RangeUpgrade");
        Sprite armourIcon = Resources.Load<Sprite>("Icons/ArmourUpgrade");

        // Initialize available upgrades with their default values
        availableUpgrades.Add(new CooldownRateUpgrade(cooldownRateIcon));
        availableUpgrades.Add(new HealthUpgrade(healthIcon));
        availableUpgrades.Add(new DamageUpgrade(damageIcon));
        availableUpgrades.Add(new MovementSpeedUpgrade(movementSpeedIcon));
        availableUpgrades.Add(new CriticalHitChanceUpgrade(criticalHitChanceIcon));
        availableUpgrades.Add(new CriticalHitDamageUpgrade(criticalHitDamageIcon));
        availableUpgrades.Add(new RangeUpgrade(rangeIcon));
        availableUpgrades.Add(new ArmourUpgrade(armourIcon));
    }

    // Method to get list of random upgrades
    public List<UpgradeData> GenerateUpgradeChoices(float count)
    {
        List<UpgradeData> upgradeChoices = new List<UpgradeData>();
        List<UpgradeData> availableStatUpgradesCopy = new List<UpgradeData>(availableUpgrades);
        for (int i = 0; i < count; i++)
        {
            if (availableStatUpgradesCopy.Count == 0) break;
            int randomIndex = Random.Range(0, availableStatUpgradesCopy.Count);
            upgradeChoices.Add(availableStatUpgradesCopy[randomIndex]);
            availableStatUpgradesCopy.RemoveAt(randomIndex);  // Prevent duplicates
        }

        return upgradeChoices;
    }

    private void ShowUpgradeChoices(float choiceAmount)
    {
        var upgradeChoices = GenerateUpgradeChoices(choiceAmount);
        EventManager.Instance.TriggerEvent("UpgradeChoicesReady", upgradeChoices);
    }

    public void AddNewUpgrade(UpgradeData newUpgrade)
    {
        availableUpgrades.Add(newUpgrade);
        Debug.Log($"Added upgrade: {newUpgrade.upgradeName}");
    }

    public void ResetUpgrades()
    {
        availableUpgrades.Clear();
        LoadUpgrades();
    }
}
