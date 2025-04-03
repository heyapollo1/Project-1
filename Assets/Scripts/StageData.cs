using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class StageData : ScriptableObject
{
    public string stageName;
    public string stageDescription;
    public Sprite stageIcon;
    //public Transform spawnPosition;
}
