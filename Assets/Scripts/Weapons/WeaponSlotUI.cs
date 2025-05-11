using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlotUI : MonoBehaviour, IDropHandler
{
    [HideInInspector]public ItemUI slottedItem;
    public bool isWeapon;
    public bool isLeftHand;
    public int loadoutID;
    
    public bool IsFilled()
    {
        if (slottedItem == null)
        {
            Debug.Log($"Fail");
            return false;
        }
        return true;
    }
    
    public void AssignItemToLoadoutSlot(ItemUI item, bool itemIsWeapon)
    {
        Debug.Log($"Slotted {item.name}, {(itemIsWeapon ? "is weapon":"is item")}");
        slottedItem = item;
        isWeapon = itemIsWeapon;
    }
    
    public void SetEmpty()
    {
        slottedItem = null;
    }
    
    
     public void OnDrop(PointerEventData eventData)
    {
        if (ItemUI.draggedItem == null) return;
        WeaponSlotUI targetSlot = this;
       bool sourceIsWeapon = ItemUI.draggedItem.isWeapon;
       bool sourceIsFromInventory = !ItemUI.draggedItem.isInHand;
           
       WeaponInstance sourceWeaponInstance = sourceIsWeapon ? ItemUI.draggedItem?.assignedWeapon : null;
       BaseItem sourceBaseItem = sourceIsWeapon ?  null : ItemUI.draggedItem?.assignedItem;

       WeaponInstance targetWeaponInstance = isWeapon ? targetSlot.slottedItem?.assignedWeapon : null;
       BaseItem targetBaseItem = isWeapon ? null : targetSlot.slottedItem?.assignedItem;
       
       //Debug.Log($"[OnDrop] DraggedItem assignedWeapon: {ItemUI.draggedItem?.assignedWeapon.weaponTitle}");
       //Debug.Log($"[OnDrop] DraggedItem assignedItem: {ItemUI.draggedItem?.assignedItem.itemName}");
       //Debug.Log($"[OnDrop] slottedItem.assignedWeapon: {targetSlot.slottedItem?.assignedWeapon.weaponTitle}");
       //Debug.Log($"[OnDrop] slottedItem.assignedItem: {targetSlot.slottedItem?.assignedItem.itemName}");
           
       var payload = new UISwapPayload
       {
           sourceItem = new ItemPayload(){
                weaponScript = sourceWeaponInstance,
                itemScript = sourceBaseItem,
                isWeapon = sourceIsWeapon},
           
           targetItem = new ItemPayload(){
               weaponScript = targetWeaponInstance,
               itemScript = targetBaseItem,
               isWeapon = isWeapon},
       };
       
        if (!sourceIsFromInventory)
        {
            ItemUI.successfulSwap = false;
            if (targetSlot == ItemUI.draggedItem.originalHandSlot) return;
        }

        if (targetSlot.IsFilled())
        {
            if (isWeapon)
            {
                WeaponInstance targetWeapon = slottedItem.assignedWeapon;
                if (targetWeapon.weaponBase != null && (targetWeapon.weaponBase.IsWeaponDisabled() || targetWeapon.weaponBase.isWeaponOnCooldown()))
                {
                    Debug.Log("Drop denied - Target weapon is disabled or on cooldown.");
                    ItemUI.successfulSwap = false;
                    return;
                }
            }
            
            Debug.LogWarning($"source is going to {(isLeftHand ? "LEFT" : "RIGHT")}");
            ItemUI.successfulSwap = true;
            UIManager.Instance.SetDragging(false);
            WeaponManager.Instance.HandleLoadoutPayload(payload, isLeftHand, sourceIsFromInventory);
        }
        else
        {
            Debug.Log($"Trying to move to hand empty space from {(!sourceIsFromInventory ? "HAND":"INVENTORY")}. ");
            ItemUI.successfulSwap = true;
            UIManager.Instance.SetDragging(false);
            WeaponManager.Instance.HandleLoadoutPayload(payload, isLeftHand, sourceIsFromInventory, true);
        }
    }
}