using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Simple, 
    Combat      
}


[System.Serializable]
public abstract class ItemData
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int price;
    public ItemType itemType;
    public GameObject player;
    public EnemyDetector enemyDetector;

    public abstract void Apply(PlayerStatManager playerStats);

    public abstract void Remove(PlayerStatManager playerStats);
}