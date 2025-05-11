using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StoredLoadoutUI : MonoBehaviour
{
    [Header("UI Elements")]
    public StoredSlotUI leftStoredUI;
    public StoredSlotUI rightStoredUI;
    
    private PlayerLoadout cachedLoadout;
    private int loadoutID;
    
    public void UpdateStoredLoadout(PlayerLoadout loadout)
    {
        if (loadout == null) return;
        cachedLoadout = loadout;
        loadoutID = loadout.loadoutIndex;
        
        if (!cachedLoadout.IsHandFree(true))
        {
            if (cachedLoadout.leftHandItem.isWeapon) leftStoredUI.InitializeWeaponUI(cachedLoadout.leftHandItem.weaponScript, loadoutID, true);
            else leftStoredUI.InitializeItemUI(cachedLoadout.leftHandItem.itemScript, loadoutID, true);
        }
        else
        {
            leftStoredUI.SetEmpty();
        }

        if (!cachedLoadout.IsHandFree(false))
        {
            if (cachedLoadout.rightHandItem.isWeapon) rightStoredUI.InitializeWeaponUI(cachedLoadout.rightHandItem.weaponScript, loadoutID, false);
            else rightStoredUI.InitializeItemUI(cachedLoadout.rightHandItem.itemScript, loadoutID, false);
        }
        else
        {
            rightStoredUI.SetEmpty();
        }
    }
}