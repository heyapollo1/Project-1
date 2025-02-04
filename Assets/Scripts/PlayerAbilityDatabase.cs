using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityDatabase : BaseManager
{
    public static PlayerAbilityDatabase Instance { get; private set; }
    public override int Priority => 10;
    public List<PlayerAbilityBase> abilityPrefabs = new List<PlayerAbilityBase>();
    public List<PlayerAbilityData> abilityDataList = new List<PlayerAbilityData>();

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("ShowAbilityChoices", ShowAbilityChoices);
        EventManager.Instance.StartListening("RequestAbilityReroll", ShowAbilityChoices);
        LoadAbilities();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("ShowUpgradeChoices", ShowAbilityChoices);
        EventManager.Instance.StopListening("RequestUpgradeReroll", ShowAbilityChoices);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (var prefab in abilityPrefabs)
        {
            Debug.Log($"Prefab in list: {prefab.name}");
        }
    }

    private void LoadAbilities()
    {
        abilityPrefabs.Clear();
        abilityDataList.Clear();

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("PlayerAbilities");

        foreach (var prefab in loadedPrefabs)
        {
            PlayerAbilityBase ability = prefab.GetComponent<PlayerAbilityBase>();

            if (ability != null)
            {
                abilityPrefabs.Add(ability);

                if (ability.abilityData != null)
                {
                    abilityDataList.Add(ability.abilityData);
                    Debug.Log($"Loaded ability: {ability.abilityData.abilityTitle}");
                }
                else
                {
                    Debug.LogWarning($"{prefab.name} is missing abilityData!");
                }
            }
        }
    }

    public List<PlayerAbilityData> GenerateAbilityChoices(float count)
    {
        List<PlayerAbilityData> randomAbilities = new List<PlayerAbilityData>();
        List<PlayerAbilityData> availableAbilities = new List<PlayerAbilityData>(abilityDataList);

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
    }
}