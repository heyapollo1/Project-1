using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemPayload
{
    public WeaponInstance weaponScript;
    public BaseItem itemScript;
    public bool isWeapon;
}

[Serializable]
public class UISwapPayload
{
    public ItemPayload sourceItem;
    public ItemPayload targetItem;
}