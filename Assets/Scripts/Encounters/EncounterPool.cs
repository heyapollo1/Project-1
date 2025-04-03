using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterPool", menuName = "Encounters/EncounterPool")]
public class EncounterPool : ScriptableObject
{
    public List<EncounterData> availableEncounters;
}
