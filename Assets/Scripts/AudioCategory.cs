using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCategory", menuName = "Audio/Category")]
public class AudioCategory : ScriptableObject
{
    public string categoryPrefix;  // E.g., "Footsteps_Grass" or "FireWeapon"
    public List<AudioClip> clips;  // The associated clips
}
