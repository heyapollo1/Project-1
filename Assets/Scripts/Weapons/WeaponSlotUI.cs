using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotUI : MonoBehaviour, IDropHandler
{
    [HideInInspector]public ActiveWeaponUI equippedWeapon;
    public bool isLeftSlot;
    
    public bool IsFilled()
    {
        if (equippedWeapon == null)
        {
            Debug.Log($"Fail");
            return false;
        }
        return true;
    }
    
    public void AssignWeaponUI(ActiveWeaponUI weaponUI)
    {
        equippedWeapon = weaponUI;
    }
    
    public void ClearWeaponUI()
    {
        equippedWeapon = null;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("WeaponSlotUI received drop");

        if (ActiveWeaponUI.draggedWeapon == null) return;
        
        WeaponSlotUI sourceSlot = ActiveWeaponUI.draggedWeapon.originalParent.GetComponent<WeaponSlotUI>();
        
        WeaponInstance sourceWeapon = sourceSlot.equippedWeapon?.assignedWeapon;
        WeaponInstance targetWeapon = equippedWeapon?.assignedWeapon;
        
        bool isTargetLeftHand = isLeftSlot;
        bool isSourceLeftHand = sourceSlot.isLeftSlot;
        if (sourceSlot == null) return;
        if (targetWeapon == sourceWeapon) return;
        
        if (IsFilled())
        {
            if (targetWeapon.weaponBase != null && (targetWeapon.weaponBase.isWeaponDisabled() || targetWeapon.weaponBase.isWeaponOnCooldown()))
            {
                Debug.Log("Drop denied - Target weapon is disabled or on cooldown.");
                ActiveWeaponUI.successfulSwap = false;
                return;
            }
            AssignWeaponUI(sourceSlot.equippedWeapon);
            sourceSlot.AssignWeaponUI(equippedWeapon);
            ActiveWeaponUI.successfulSwap = true;
            WeaponManager.Instance.SwapWeaponHands(sourceWeapon, targetWeapon, isSourceLeftHand, isTargetLeftHand);
        }
        else
        {
            AssignWeaponUI(sourceSlot.equippedWeapon);
            sourceSlot.ClearWeaponUI();
            ActiveWeaponUI.successfulSwap = true;
            Debug.Log($"MOVE: {sourceWeapon.weaponTitle} → {(isTargetLeftHand ? "LEFT" : "RIGHT")} hand");
            WeaponManager.Instance.PlaceWeaponInEmptyHand(sourceWeapon, isTargetLeftHand);
            return;
        }

        Debug.Log("Not even close");
        ActiveWeaponUI.successfulSwap = false;
    }
}