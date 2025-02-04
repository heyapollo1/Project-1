using UnityEngine;
using System.Collections.Generic;

public class PlayerAbilityManager : MonoBehaviour
{
    public static PlayerAbilityManager Instance { get; private set; }  // Singleton instance
    private List<PlayerAbilityBase> playerAbilities = new List<PlayerAbilityBase>(); // abilities player owns

    private bool isDisabled = false;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EventManager.Instance.StartListening("DisablePlayerAbilities", DisableAbilities);
        EventManager.Instance.StartListening("AbilitySelected", ApplyAbility);
        EventManager.Instance.StartListening("QueryAcquiredAbilities", PlayerAcquiredAbilities);
        //EventManager.Instance.StartListening("AbilityReady", OnSceneLoaded);
    }

    private void OnDestroy()
    {
        ResetAbilities();
        EventManager.Instance.StopListening("DisablePlayerAbilities", DisableAbilities);
        EventManager.Instance.StopListening("AbilitySelected", ApplyAbility);
        EventManager.Instance.StopListening("QueryAcquiredAbilities", PlayerAcquiredAbilities);
        //EventManager.Instance.StopListening("AbilityReady", OnSceneLoaded);
    }

    private void Update()
    {
        if (isDisabled) return;
        foreach (var ability in playerAbilities)
        {
            if (ability != null && ability.CanUseAbility())
            {
                ability.UseAbility();
            }
        }
    }

    public void ApplyAbility(PlayerAbilityData selectedAbility)
    {
        UnlockAbility(selectedAbility, gameObject.transform);
        UnlockAbilitySpecificUpgrades(selectedAbility);
    }

    public void UnlockAbility(PlayerAbilityData playerAbilityData, Transform player)
    {
        EventManager.Instance.TriggerEvent("AbilityUnlocked", playerAbilityData);
        GameObject abilityInstance = Instantiate(playerAbilityData.abilityPrefab, player);
        PlayerAbilityBase newAbility = abilityInstance.GetComponent<PlayerAbilityBase>();

        newAbility.abilityData = playerAbilityData;
        playerAbilities.Add(newAbility);
        newAbility.Initialize();

        Debug.Log($"Unlocked ability: {playerAbilityData.abilityTitle}");
    }

    public void UnlockAbilitySpecificUpgrades(PlayerAbilityData data)
    {
        foreach (var ability in playerAbilities)
        {
            if (ability == null || ability.abilityData == null)
            {
                Debug.LogWarning("Found a null entry in playerAbilities list!");
                continue;
            }

            if (ability.abilityData == data)
            {
                Debug.Log($"Adding specific upgrades to: {data.abilityTitle}");
                ability.AddAbilitySpecificUpgrades();
                return;
            }
        }
        Debug.LogWarning($"No matching ability found for: {data.abilityTitle}");
    }

    public T GetAbilityOfType<T>() where T : PlayerAbilityBase
    {
        foreach (var ability in playerAbilities)
        {
            if (ability is T matchedAbility)
            {
                return matchedAbility;
            }
        }
        Debug.LogWarning($"Ability of type {typeof(T).Name} not found.");
        return null;
    }

    private void PlayerAcquiredAbilities(List<PlayerAbilityData> queryResult)
    {
        foreach (var ability in playerAbilities)
        {
            if (ability != null && ability.abilityData != null)
            {
                queryResult.Add(ability.abilityData);
            }
        }
    }

    public void DisableAbilities()
    {
        foreach (var ability in playerAbilities)
        {
            if (ability != null)
            {
                isDisabled = true;
                ability.DisableAbility();
            }
        }
    }

    public void EnableAbilities()
    {
        foreach (var ability in playerAbilities)
        {
            if (ability != null)
            {
                ability.AbilityReset();
                ability.EnableAbility();
            }
        }
    }

    public void ResetAbilities()
    {
        foreach (var ability in playerAbilities)
        {
            if (ability != null)
            {
                Destroy(ability.gameObject);
            }
        }

        playerAbilities.Clear(); 
    }
}
