using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class ActiveLoadoutUI : MonoBehaviour
{
    public static ActiveLoadoutUI Instance { get; private set; }
    
    [Header("UI Elements")]
    public WeaponSlotUI leftWeaponSlot;
    public WeaponSlotUI rightWeaponSlot;
    public GameObject itemUIPrefab;
    
    private ItemUI leftHandUI;
    private ItemUI rightHandUI;

    public bool IsLeftHandOccupied() => leftHandUI != null && leftHandUI.isWeapon;
    public bool IsRightHandOccupied() => rightHandUI != null && leftHandUI.isWeapon;
    public ItemUI GetActiveLeftItem() => leftHandUI;
    public ItemUI GetActiveRightItem() => rightHandUI;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateActiveLoadout(PlayerLoadout loadout)
    {
        if (loadout == null) return;
        ClearActiveLoadout(); //remove visual objects
        
        if (!loadout.IsHandFree(true))
        {
            if (loadout.leftHandItem == null) return;
            Debug.Log("Not hand free");
            //Debug.Log($"creating left: {(loadout.leftHandItem.isWeapon ? loadout.leftHandItem.weaponScript.weaponTitle : loadout.leftHandItem.itemScript.itemName)}");
            GameObject leftObj = Instantiate(itemUIPrefab, leftWeaponSlot.transform);
            leftHandUI = leftObj.GetComponent<ItemUI>();
            leftHandUI.InitializeItemUI(
                loadout.leftHandItem.isWeapon ? null : loadout.leftHandItem.itemScript,
                loadout.leftHandItem.isWeapon ? loadout.leftHandItem.weaponScript : null, true);
        }
        
        if (!loadout.IsHandFree(false))
        {
            Debug.Log($"creating right: {(loadout.leftHandItem.isWeapon ? loadout.leftHandItem.weaponScript.weaponTitle : loadout.leftHandItem.itemScript.itemName)}");
            GameObject rightObj = Instantiate(itemUIPrefab, leftWeaponSlot.transform);
            rightHandUI = rightObj.GetComponent<ItemUI>();
            rightHandUI.InitializeItemUI(
                loadout.rightHandItem.isWeapon ? null : loadout.rightHandItem.itemScript,
                loadout.rightHandItem.isWeapon ? loadout.rightHandItem.weaponScript : null, true);
        }
    }
    
    public void UpgradeActiveItem(bool isLeftHand)
    {
        if (isLeftHand && leftHandUI != null)
        {
            if (leftHandUI.isWeapon) leftHandUI.UpgradeWeapon();
            else leftHandUI.UpgradeItem();
        }
        else if (!isLeftHand && rightHandUI != null)
        {
            if (rightHandUI.isWeapon) rightHandUI.UpgradeWeapon();
            else rightHandUI.UpgradeItem();
        }
    }
    
    public void HighlightSlot(bool isLeftHand, bool active)
    {
        if (isLeftHand && leftHandUI != null)
        {
            leftHandUI.HighlightSlot(active);
        }
        else if (!isLeftHand && rightHandUI != null)
        {
            rightHandUI.HighlightSlot(active);
        }
    }
    
    public void ClearActiveLoadout()
    {
        if (leftHandUI != null)
        {
            Destroy(leftHandUI.gameObject);
            leftWeaponSlot.SetEmpty();
            leftHandUI = null;
        }

        if (rightHandUI != null)
        {
            Destroy(rightHandUI.gameObject);
            rightWeaponSlot.SetEmpty();
            rightHandUI = null;
        }
    }
}*/