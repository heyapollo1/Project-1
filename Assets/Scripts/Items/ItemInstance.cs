using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    public WeaponData weaponData;  
    public WeaponBase weaponBase;  
    public Rarity rarity;
    
    public string weaponTitle;
    public string weaponDescription;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    public TagType mainTag;
    public int value;
    public int loadoutID = -1;
    public bool isLeftHand = false;
}
