using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterData : ScriptableObject
{
    public Sprite encounterIcon;
    public string encounterName;
    public string encounterDescription;

    public List<StageBracket> availableBrackets;
    
    public abstract EncounterType encounterType { get; }
    
    public bool IsAvailableInBracket(StageBracket currentBracket)
    {
        return availableBrackets.Contains(currentBracket);
    }
    
    public abstract void TriggerEncounter(int stageIndex);
    
    public abstract void EndEncounter();
}
