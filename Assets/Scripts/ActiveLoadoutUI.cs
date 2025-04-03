using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveLoadoutUI : MonoBehaviour
{
    [Header("UI Elements")]
    public WeaponSlotUI leftWeaponSlot;
    public WeaponSlotUI rightWeaponSlot;
    public GameObject activeWeaponUIPrefab;
    
    private ActiveWeaponUI leftWeaponUI;
    private ActiveWeaponUI rightWeaponUI;
    
    public ActiveWeaponUI GetLeftWeaponUI()
    {
        return leftWeaponUI;
    }
    
    public ActiveWeaponUI GetRightWeaponUI()
    {
        return rightWeaponUI;
    }
        
    public void UpdateActiveLoadout(WeaponLoadout loadout)
    {
        ClearActiveLoadout();
        if (loadout.leftWeapon != null)
        {
            //WeaponSlotUI weaponSlot = leftWeaponSlot.GetComponent<WeaponSlotUI>();
            GameObject leftObj = Instantiate(activeWeaponUIPrefab, leftWeaponSlot.transform);
            leftWeaponUI = leftObj.GetComponent<ActiveWeaponUI>();
            leftWeaponUI.InitializeWeaponUI(loadout.leftWeapon, loadout.loadoutID, true, leftWeaponSlot);
            //leftWeaponSlot.AssignWeaponUI(leftWeaponUI);
        }

        if (loadout.rightWeapon != null)
        {
            //WeaponSlotUI weaponSlot = rightWeaponSlot.GetComponent<WeaponSlotUI>();
            GameObject rightObj = Instantiate(activeWeaponUIPrefab, rightWeaponSlot.transform);
            rightWeaponUI = rightObj.GetComponent<ActiveWeaponUI>();
            rightWeaponUI.InitializeWeaponUI(loadout.rightWeapon, loadout.loadoutID, false, rightWeaponSlot);
            //rightWeaponSlot.AssignWeaponUI(rightWeaponUI);
        }
    }
    
    public void UpgradeActiveWeapons(bool isLeftHand)
    {
        if (isLeftHand && leftWeaponUI != null)
        {
            leftWeaponUI.UpgradeWeaponInSlot();
        }
        else if (!isLeftHand && rightWeaponUI != null)
        {
            rightWeaponUI.UpgradeWeaponInSlot();
        }
    }
    
    public void HighlightSlot(WeaponInstance weapon, bool isLeftHand, bool highlight)
    {
        if (isLeftHand && leftWeaponUI != null)
        {
            leftWeaponUI.HighlightSlot(highlight);
        }
        else if (!isLeftHand && rightWeaponUI != null)
        {
            rightWeaponUI.HighlightSlot(highlight);
        }
    }
    
    public void ClearActiveLoadout()
    {
        if (leftWeaponUI != null)
        {
            Destroy(leftWeaponUI.gameObject);
            leftWeaponSlot.ClearWeaponUI();
            leftWeaponUI = null;
        }

        if (rightWeaponUI != null)
        {
            Destroy(rightWeaponUI.gameObject);
            rightWeaponSlot.ClearWeaponUI();
            rightWeaponUI = null;
        }
    }
}