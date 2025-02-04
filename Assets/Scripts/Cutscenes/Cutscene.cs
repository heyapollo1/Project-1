using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Cutscene")]
public class Cutscene : ScriptableObject
{
    public CutsceneAction[] actions;
}