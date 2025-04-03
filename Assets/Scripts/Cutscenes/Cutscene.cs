using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/New Cutscene")]
public class Cutscene : ScriptableObject
{
    public string cutsceneName;
    
    public CutsceneAction[] actions;
}