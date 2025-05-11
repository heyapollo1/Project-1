using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerLoadout //For in-game purposes.
{
    [NonSerialized] public ItemPayload leftHandItem;
    [NonSerialized] public ItemPayload rightHandItem;
    
    public int loadoutIndex;
    
    public PlayerLoadout(ItemPayload leftItem = null, ItemPayload rightItem = null)
    {
        leftHandItem = leftItem;
        rightHandItem = rightItem;
    }
    
    public void AssignItemToLoadout(ItemPayload assignedItem, ItemLocation handLocation)
    {
        //Debug.Log($"assigned to loadoutID: {(assignedItem.isWeapon ? assignedItem.weaponScript.weaponTitle : assignedItem.itemScript.itemName)}");
        if (handLocation == ItemLocation.LeftHand)
        {
            leftHandItem = assignedItem;
        }
        else
        {
            rightHandItem = assignedItem;
        }
    }
    
    public void UnassignItemFromLoadout(ItemLocation handLocation)
    {
        if (handLocation == ItemLocation.LeftHand)
        {
            leftHandItem = null;
        }
        else
        {
            rightHandItem = null;
        }
    }
    
    public bool IsHandFree(bool isLeftHand)
    {
        if (isLeftHand)
        {
            return leftHandItem == null;
        }
        return rightHandItem == null;
    }
    
    public bool IsEmpty()
    {
        return leftHandItem == null && rightHandItem == null;
    }
}

[System.Serializable]
public class PlayerLoadoutData //For saving and loading purposes.
{
    public string leftItemName;
    public bool isLeftAWeapon;
    public bool isLeftInActiveHand;
    
    public string rightItemName;
    public bool isRightAWeapon;
    public bool isRightInActiveHand;

    public int loadoutID;

    public PlayerLoadoutData(PlayerLoadout loadout)
    {
        loadoutID = loadout.loadoutIndex;
        
        if (loadout.leftHandItem != null)
        {
            Debug.Log($"LOADOUT ({loadoutID}) - Logging LEFTHAND: {loadout.leftHandItem}, isWeapon: {loadout.leftHandItem.isWeapon}");
            leftItemName = loadout.leftHandItem.isWeapon ? loadout.leftHandItem.weaponScript.weaponTitle : loadout.leftHandItem.itemScript.itemName;
            isLeftAWeapon = loadout.leftHandItem.isWeapon;
            isLeftInActiveHand = WeaponManager.Instance.GetCurrentLoadoutID() == loadoutID;
        }
        else
        {
            Debug.Log($"LOADOUT ({loadoutID}) - Logging LEFTHAND: EMPTY");
            leftItemName = "EMPTY";
        }

        if (loadout.rightHandItem != null)
        {
            Debug.Log($"LOADOUT ({loadoutID}) - Logging RIGHTHAND: {loadout.rightHandItem}, isWeapon: {loadout.rightHandItem.isWeapon}");
            rightItemName = loadout.rightHandItem.isWeapon ? loadout.rightHandItem.weaponScript.weaponTitle : loadout.rightHandItem.itemScript.itemName;
            isRightAWeapon = loadout.rightHandItem.isWeapon;
            isRightInActiveHand = WeaponManager.Instance.GetCurrentLoadoutID() == loadoutID;
        }
        else
        {
            Debug.Log($"LOADOUT ({loadoutID}) - Logging RIGHTHAND: EMPTY");
            rightItemName = "EMPTY";
        }
    }
}

/*[System.Serializable]
public class PlayerLoadout //For in-game purposes.
{
    public WeaponInstance leftWeapon;
    public BaseItem leftItem;
    public ItemUI leftHandItem;
    public bool isLeftAWeapon;
    
    public WeaponInstance rightWeapon;
    public BaseItem rightItem;
    public bool isRightAWeapon;
    
    public int loadoutID;
    
    public PlayerLoadout(bool isLeftAWeapon, bool isRightAWeapon, WeaponInstance leftWeapon = null, BaseItem leftItem = null, WeaponInstance rightWeapon = null, BaseItem rightItem = null)
    {
        this.isLeftAWeapon = isLeftAWeapon;
        this.isRightAWeapon = isRightAWeapon;

        if (isLeftAWeapon)
        {
            this.leftWeapon = leftWeapon;
            this.leftItem = null;
        }
        else
        {
            this.leftItem = leftItem;
            this.leftWeapon = null;
        }
        
        if (isRightAWeapon)
        {
            this.rightWeapon = leftWeapon;
            this.rightItem = null;
        }
        else
        {
            this.rightItem = leftItem;
            this.rightWeapon = null;
        }
    }
    
    public void AssignWeapon(WeaponInstance assignedWeapon, bool isLeftHand)
    {
        if (isLeftHand)
        {
            if (assignedWeapon == null) leftWeapon = null;
            else
            {
                leftWeapon = assignedWeapon;
                isLeftAWeapon = true;
            }
        }
        else
        {
            if (assignedWeapon == null) rightWeapon = null;
            else
            {
                rightWeapon = assignedWeapon;
                isRightAWeapon = true;
            }
        }
    }
    
    public void AssignItem(BaseItem assignedItem, bool isLeftHand)
    {
        if (isLeftHand)
        {
            if (assignedItem == null) leftItem = null;
            else
            {
                leftItem = assignedItem;
                isLeftAWeapon = false;
            }
        }
        else
        {
            if (assignedItem == null) rightItem = null;
            else
            {
                Debug.Log($"assigned right item: {assignedItem.itemName}");
                rightItem = assignedItem;
                isRightAWeapon = false;
            }
        }
    }
    
    public void UnassignWeapon(bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftWeapon = null;
            leftItem = null;
        }
        else
        {
            rightWeapon = null;
            rightItem = null;
        }
    }
    
    public bool IsHandFree(bool isLeftHand)
    {
        if (isLeftHand)
        {
            return leftWeapon == null && leftItem == null;
        }
        return rightWeapon == null && rightItem == null;
    }
}*/

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
