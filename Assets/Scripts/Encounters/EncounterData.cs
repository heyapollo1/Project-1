using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterData", menuName = "Encounters/Encounter Data")]
public class EncounterData : ScriptableObject
{
    public string encounterName;
    public string encounterDescription;
    public Sprite encounterIcon;
    public EncounterType encounterType;

    public List<StageBracket> availableBrackets;
    
    public bool IsAvailableInBracket(StageBracket currentBracket)
    {
        return availableBrackets.Contains(currentBracket);
    }
}
