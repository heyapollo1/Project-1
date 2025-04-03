using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StoredLoadoutUI : MonoBehaviour
{
    [Header("UI Elements")]
    public StoredWeaponUI leftWeaponUI;
    public StoredWeaponUI rightWeaponUI;
    
    private int loadoutID;
    private WeaponLoadout cachedLoadout;
    
    public void Initialize(WeaponLoadout loadout)
    {
        cachedLoadout = loadout;
        loadoutID = loadout.loadoutID;
        UpdateStoredLoadout();
    }
    
    /*public StoredWeaponUI GetStoredWeaponUI(string weaponKey)
    {
        if (leftWeaponKey == weaponKey) return leftWeaponUI;
        if (rightWeaponKey == weaponKey) return rightWeaponUI;
        return null;
    }*/
    
    public void UpgradeWeaponInStoredLoadout(bool isLeftHand)
    {
        if (isLeftHand)
        {
            leftWeaponUI.UpgradeWeaponInSlot();
        }
        else
        {
            rightWeaponUI.UpgradeWeaponInSlot();
        }
    }
    
    public void UpdateStoredLoadout()
    {
        if (cachedLoadout == null)
        {
            Debug.LogError("StoredLoadoutUI: No cached loadout.");
            return;
        }
        bool isLeftValid = cachedLoadout.leftWeapon != null && cachedLoadout.leftWeapon.weaponPrefab != null;
        bool isRightValid = cachedLoadout.rightWeapon != null && cachedLoadout.rightWeapon.weaponPrefab != null;
        
        if (isLeftValid)
        {
            leftWeaponUI.Initialize(cachedLoadout.leftWeapon, loadoutID, true);
        }
        else
        {
            Debug.LogWarning($"Left weapon null or prefab missing for loadout {loadoutID}");
            leftWeaponUI.SetEmpty();
        }

        if (isRightValid)
        {
            rightWeaponUI.Initialize(cachedLoadout.rightWeapon, loadoutID, false);
        }
        else
        {
            Debug.LogWarning($"Right weapon null or prefab missing for loadout {loadoutID}");
            rightWeaponUI.SetEmpty();
        }
    }

    public void HighlightSlot(WeaponInstance weapon, bool isLeftHand, bool highlight)
    {
        if (isLeftHand)
        {
            leftWeaponUI.HighlightSlot(highlight);
        }
        else
        {
            rightWeaponUI.HighlightSlot(highlight);
        }
    }
}