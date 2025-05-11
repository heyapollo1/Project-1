using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance;
    
    public EncounterData encounterData;
    private EncounterType encounterType = EncounterType.None;
    private BaseEncounter currentEncounter;
    private EncounterPool encounterPool;
    private bool isInEncounter = false;
    
    public EncounterType GetCurrentEncounterType() => encounterType;
    public BaseEncounter GetCurrentEncounter() => currentEncounter;
    public bool IsEncounterActive() => isInEncounter;

    private Dictionary<EncounterType, BaseEncounter> encounterList = new Dictionary<EncounterType, BaseEncounter>();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        EventManager.Instance.StartListening("TriggerEncounter", StartCurrentEncounter);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("TriggerEncounter", StartCurrentEncounter);
    }

    private void StartCurrentEncounter()
    {
        currentEncounter.EnterEncounter();
    }

    public void RegisterEncounter(BaseEncounter encounter)
    {
        if (encounterList.ContainsKey(encounter.encounterType))
        {
            Debug.LogWarning($"Duplicate encounter type: {encounter.encounterType}");
            return;
        }
        encounterList[encounter.encounterType] = encounter;
        Debug.Log($"Registered encounter: {encounter.encounterType} ({encounter.name})");
    }
    
    public void SaveEncounterState(EncounterData encounter)
    {
        Debug.Log($"Saving Encounter State: {encounter.name}");
        if (isInEncounter) return;
        isInEncounter = true;
        encounterType = encounter.encounterType;
        currentEncounter = GetEncounterFromType(encounterType);
    }
    
    public void ExitEncounterState()
    {
        Debug.LogWarning("Saving Encounter Data");
        if (!isInEncounter) return;
        isInEncounter = false;
        encounterType = EncounterType.None;
        currentEncounter = null;
    }
    
    public List<EncounterData> GetEventChoices()
    {
        var encounterPool = Resources.Load<EncounterPool>("ScriptableObjects/Encounters/EncounterPool");
        if (encounterPool == null || encounterPool.availableEncounters.Count == 0)
        {
            Debug.LogError("No events found in the pool.");
            return null;
        }
        
        List<EncounterData> selectedEncounters = new List<EncounterData>();
        while (selectedEncounters.Count < 2 && selectedEncounters.Count < encounterPool.availableEncounters.Count)
        {
            EncounterData randomEncounter = encounterPool.availableEncounters[Random.Range(0, encounterPool.availableEncounters.Count)];
            if (!selectedEncounters.Contains(randomEncounter))
                selectedEncounters.Add(randomEncounter);
        }
        
        return selectedEncounters;
    }

    public BaseEncounter GetEncounterFromType(EncounterType encounterType)
    {
        if (encounterList.ContainsKey(encounterType)) return encounterList[encounterType];
        return null;
    }
    
    public void LoadEncounterFromSave(WorldState worldState) // load state
    {
        if (StageManager.Instance.stageIsActive)
        {
            isInEncounter = false;
            return;
        }
        Debug.Log($"Loading Encounter {worldState.currentEncounter}");
        encounterType = worldState.currentEncounter;
        
        switch (worldState.currentEncounter)
        {
            case EncounterType.Shop:
                ShopEncounter.Instance.LoadEncounterState(worldState);
                break; 

            case EncounterType.Treasure:
                TreasureEncounter.Instance.LoadEncounterState(worldState);
                break;

            case EncounterType.Victory:
                //VictoryManager.Instance.StartEncounter();
                break;

            case EncounterType.None:
                Debug.Log("No active encounter.");
                isInEncounter = false;
                break;
        }
    }
    
    public void IsInEncounter(bool isInEncounter)
    {
        this.isInEncounter = isInEncounter;
    }
}
