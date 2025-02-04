using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UpgradeData
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public int upgradeLevel = 1;         

    public abstract void Apply(PlayerStatManager playerStats);

    public abstract void ScaleUpgrade(PlayerStatManager playerStats);

    internal void ScaleUpgrade(object playerStats)
    {
        throw new NotImplementedException();
    }
}
