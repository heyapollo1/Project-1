using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventoryUISlot : MonoBehaviour, IDropHandler
{
    public ItemUI slottedItem; // Reference to the item inside the slot
    public GameObject highlightFX;
    public bool isWeapon;
    public int slotIndex;
    
    public void InitializeItemSlot(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }
    
    public bool IsFilled()
    {
        return slottedItem != null;
    }
    
    public void Clear()
    {
        Destroy(slottedItem.gameObject);
        slottedItem = null;
    }
    
    public void AssignItemToInventorySlot(ItemUI newItem, bool isWeapon)
    {
        slottedItem = newItem;
        this.isWeapon = isWeapon;
        newItem.transform.SetParent(transform);
        newItem.transform.localPosition = Vector3.zero;
    }
    
    public string GetSlottedItemID()
    {
        return slottedItem.uniqueID;
    }
    
    public BaseItem GetItemInSlot()
    {
        return slottedItem.assignedItem;
    }
    
    public WeaponInstance GetWeaponInSlot()
    {
        return slottedItem.assignedWeapon;
    }
    
    public void HighlightSlot(bool highlight)
    {
        if (highlight)
        {
            highlightFX.SetActive(true);
        }
        else
        {
            highlightFX.SetActive(false);
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    { 
        Debug.Log("source is the same as target");
       if (ItemUI.draggedItem == null) return;
       InventoryUISlot targetSlot = this;
       bool sourceIsWeapon = ItemUI.draggedItem.isWeapon;
       bool sourceIsFromHand = ItemUI.draggedItem.isInHand;
       int sourceSlotIndex = sourceIsFromHand ? -1 : ItemUI.draggedItem.originalInventorySlot.slotIndex;
       
       WeaponInstance sourceWeaponInstance = sourceIsWeapon ? ItemUI.draggedItem?.assignedWeapon : null;
       BaseItem sourceBaseItem = sourceIsWeapon ?  null : ItemUI.draggedItem?.assignedItem;
       
       WeaponInstance targetWeaponInstance = isWeapon ? targetSlot.slottedItem?.assignedWeapon : null;
       BaseItem targetBaseItem = isWeapon ? null : targetSlot.slottedItem?.assignedItem;
       
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
       
       if (!sourceIsFromHand)
       {
           Debug.Log("inventory source slot is the same as target slot");
           ItemUI.successfulSwap = false;
           if (targetSlot == ItemUI.draggedItem.originalInventorySlot) return;
       }
       
       if (targetSlot.IsFilled())
       {
           if (isWeapon)
           {
               if (slottedItem.assignedWeapon.weaponBase != null && 
                   (slottedItem.assignedWeapon.weaponBase.IsWeaponDisabled() || slottedItem.assignedWeapon.weaponBase.isWeaponOnCooldown()))
               {
                   Debug.Log("Drop denied - Target weapon is disabled or on cooldown.");
                   ItemUI.successfulSwap = false;
                   return;
               }
           }
           
           Debug.Log($"Swapping item from {(sourceIsFromHand ? "HAND":"INVENTORY")} with item in inventory slot: {slotIndex}");
           ItemUI.successfulSwap = true;
           UIManager.Instance.SetDragging(false);
           InventoryManager.Instance.HandleInventoryPayload(payload, slotIndex, sourceSlotIndex, sourceIsFromHand);
       }
       else // Moving item to empty inventory slot
       {
           Debug.Log($"Moving item from {(sourceIsFromHand ? "HAND":"INVENTORY")} to empty inventory slot: {slotIndex}");
           ItemUI.successfulSwap = true;
           UIManager.Instance.SetDragging(false);
           InventoryManager.Instance.HandleInventoryPayload(payload, slotIndex, sourceSlotIndex, sourceIsFromHand, true);
       }
    }
}