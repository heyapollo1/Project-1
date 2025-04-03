using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CutsceneDatabase", menuName = "Cutscene/CutsceneDatabase")]
public class CutsceneDatabase : ScriptableObject
{
    public List<Cutscene> cutscenes = new List<Cutscene>();

    public Cutscene GetCutscene(string name)
    {
        foreach (var cutscene in cutscenes)
        {
            if (cutscene.cutsceneName == name)
                return cutscene;
        }
        Debug.LogError($"Cutscene '{name}' not found in database.");
        return null;
    }
}