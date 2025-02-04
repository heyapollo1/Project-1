using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    //public static UpgradeManager Instance { get; private set; }

    private List<UpgradeData> playerUpgrades = new List<UpgradeData>();

    private PlayerStatManager playerStats;

    void Awake()
    {
        EventManager.Instance.StartListening("UpgradeSelected", ApplyUpgrade);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("UpgradeSelected", ApplyUpgrade);
    }

    public void ApplyUpgrade(UpgradeData selectedUpgrade)
    {
        playerStats = PlayerStatManager.Instance;

        var existingUpgrade = playerUpgrades.Find(u => u.upgradeName == selectedUpgrade.upgradeName);

        if (existingUpgrade != null)
        {
            existingUpgrade.ScaleUpgrade(playerStats);
        }
        else
        {
            playerUpgrades.Add(selectedUpgrade);
            selectedUpgrade.Apply(playerStats);
        }
    }
}
