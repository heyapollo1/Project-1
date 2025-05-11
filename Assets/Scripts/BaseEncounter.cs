using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseEncounter : MonoBehaviour
{
    public GameObject encounterMap;
    public EncounterData encounterData;
    public Transform entryPoint;
    public EncounterType encounterType;
    
    [HideInInspector] public StageBracket stageBracket;
    [HideInInspector] public bool encounterIsActive;

    public abstract void InitializeEncounter();
    public abstract void PrepareEncounter();
    public abstract void EnterEncounter();
    public abstract void EndEncounter();
    public abstract void LoadEncounterState(WorldState state);
    public StageBracket CurrentStageBracket() => StageManager.Instance.GetStageBracket();
    
    protected virtual void Awake()
    {
        EncounterManager.Instance.RegisterEncounter(this);
        EventManager.Instance.StartListening("TriggerEncounter", MatchEncounter);
    }
    
    protected virtual void OnDestroy()
    {
        EventManager.Instance.StartListening("TriggerEncounter", MatchEncounter);
    }
    
    private void MatchEncounter(string name)
    {
        Debug.LogError($"Encounter {encounterData?.name} has been triggered");
        if (encounterData?.name == name)
        {
            Debug.Log($"Encounter {encounterData?.name} has been triggered");
            EnterEncounter();
        }
    }
}
