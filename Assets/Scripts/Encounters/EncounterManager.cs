using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance;

    private EncounterType encounterType = EncounterType.None;
    private EncounterData encounterData;
    private bool isInEncounter = false;
    
    public void Initialize()
    {
        EventManager.Instance.StartListening("EnterEncounter", AutoSaveEncounter);
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.StopListening("EnterEncounter", AutoSaveEncounter);
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void AutoSaveEncounter(EncounterData encounter)
    {
        Debug.LogWarning("Saving Encounter Data");
        if (isInEncounter) return;
        isInEncounter = true;
        encounterData = encounter;
        encounterType = encounterData.encounterType;
        MapUI.Instance.EnableMapUI();
        EventManager.Instance.TriggerEvent("ShowUI");
        
        GameData currentData = SaveManager.LoadGame(); //Save instance, automatic
        SaveManager.SaveGame(currentData);
        Debug.Log($"Encounter Saved: {PlayerManager.Instance.playerInstance.transform.position}");
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
    
    public void LoadEncounterFromSave(WorldState worldState) // load state
    {
        if (StageManager.Instance.isStageActive)
        {
            Debug.Log("Stage active, is not in encounter");
            isInEncounter = false;
            return;
        }
        Debug.Log($"Loading Encounter {worldState.currentEncounter}");
        encounterType = worldState.currentEncounter;
        encounterData = worldState.encounterData;
        
        switch (worldState.currentEncounter)
        {
            case EncounterType.Shop:
                isInEncounter = true;
                ShopManager.Instance.LoadShopState(worldState);
                break;

            case EncounterType.Treasure:
                //TreasureManager.Instance.StartEncounter();
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
    
    public bool CheckIfEncounterActive()
    {
        return isInEncounter;
    }
    
    public EncounterType GetCurrentEncounterType() // save state
    {
        return encounterType;
    }
    
    public EncounterData GetCurrentEncounterData() // save state
    {
        return encounterData;
    }
}
