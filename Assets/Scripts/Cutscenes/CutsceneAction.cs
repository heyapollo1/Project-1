using System.Collections;
using UnityEngine;
using System;

public abstract class CutsceneAction : ScriptableObject
{
    public abstract IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete);
}