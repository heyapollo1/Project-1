using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponLoadout //For in-game purposes.
{
    public WeaponInstance leftWeapon;
    public Rarity leftWeaponRarity;
    
    public WeaponInstance rightWeapon;
    public Rarity rightWeaponRarity;
    
    public int loadoutID;
    
    public WeaponLoadout(WeaponInstance leftWeapon,  Rarity leftWeaponRarity, WeaponInstance rightWeapon, Rarity rightWeaponRarity)
    {
        this.leftWeapon = leftWeapon;
        this.rightWeapon = rightWeapon;
        this.leftWeaponRarity = leftWeaponRarity;
        this.rightWeaponRarity = rightWeaponRarity;
    }
    
    public void AssignWeapon(WeaponInstance weapon, bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftWeapon = weapon;
        }
        else
        {
            rightWeapon = weapon;
        }
    }
    
    public void UnassignWeapon(bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftWeapon = null;
        }
        else
        {
            rightWeapon = null;
        }
    }
}

[System.Serializable]
public class WeaponLoadoutData //For saving and loading purposes.
{
    public string leftWeapon;
    public Rarity leftWeaponRarity;
    
    public string rightWeapon;
    public Rarity rightWeaponRarity;

    public int loadoutID;
    
    public WeaponLoadoutData(WeaponLoadout loadout)
    {
        loadoutID = loadout.loadoutID;
        
        leftWeapon = loadout.leftWeapon != null ? loadout.leftWeapon.weaponTitle : null;
        leftWeaponRarity = loadout.leftWeaponRarity;
        
        rightWeapon = loadout.rightWeapon != null ? loadout.rightWeapon.weaponTitle : null;
        rightWeaponRarity = loadout.rightWeaponRarity;
    }
}

/*[System.Serializable]
public class WeaponLoadout //For in-game purposes.
{
    public WeaponInstance leftWeapon;
    public Rarity leftWeaponRarity;
    
    public WeaponInstance rightWeapon;
    public Rarity rightWeaponRarity;

    public int loadoutID;
    
    public WeaponLoadout(WeaponInstance leftWeapon,  Rarity leftWeaponRarity, WeaponInstance rightWeapon, Rarity rightWeaponRarity)
    {
        this.leftWeapon = leftWeapon;
        this.rightWeapon = rightWeapon;
        this.leftWeaponRarity = leftWeaponRarity;
        this.rightWeaponRarity = rightWeaponRarity;
    }
    
    public void AssignWeapon(WeaponInstance weapon, bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftWeapon = weapon;
        }
        else
        {
            rightWeapon = weapon;
        }
    }
    
    public void UnassignWeapon(bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftWeapon = null;
        }
        else
        {
            rightWeapon = null;
        }
    }
}

[System.Serializable]
public class WeaponLoadoutData //For saving and loading purposes.
{
    public string leftWeapon;
    public Rarity leftWeaponRarity;

    public string rightWeapon;
    public Rarity rightWeaponRarity;

    public int loadoutID;
    
    public WeaponLoadoutData(WeaponLoadout loadout)
    {
        loadoutID = loadout.loadoutID;
        
        leftWeapon = loadout.leftWeapon != null ? loadout.leftWeapon.weaponTitle : null;
        leftWeaponRarity = loadout.leftWeaponRarity;
        
        rightWeapon = loadout.rightWeapon != null ? loadout.rightWeapon.weaponTitle : null;
        rightWeaponRarity = loadout.rightWeaponRarity;
    }
}*/
