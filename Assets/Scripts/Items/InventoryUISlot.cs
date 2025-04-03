using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUISlot : MonoBehaviour, IDropHandler
{
    public InventoryItemUI inventoryItem; // Reference to the item inside the slot
    public GameObject highlightFX;
    public int slotIndex;
    
    public void InitializeItemSlot(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }
    
    public bool IsFilled()
    {
        return inventoryItem != null;
    }

    public void CreateItem(InventoryItemUI newItem)
    {
        inventoryItem = newItem;
        newItem.transform.SetParent(transform); // Attach to slot
        newItem.transform.localPosition = Vector3.zero; // Reset position inside slot
    }
    
    public void AssignItem(InventoryItemUI newItem)
    {
        inventoryItem = newItem;
        newItem.transform.SetParent(transform); // Attach to slot
        newItem.transform.localPosition = Vector3.zero; // Reset position inside slot
    }
    
    public BaseItem GetItemInSlot()
    {
        BaseItem assignedItem = inventoryItem.GetAssignedItem();
        if (assignedItem == null) return null;
        return assignedItem;
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
    
    public void DestroySlottedItem()
    {
        if (inventoryItem != null)
        {
            inventoryItem.ClearItem();
            inventoryItem = null;
            Destroy(inventoryItem.gameObject);
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryItemUI.draggedItem == null) return;
        InventoryUISlot targetSlot = GetComponent<InventoryUISlot>();
        //InventoryUISlot targetSlot = transform.parent.GetComponent<InventoryUISlot>();
        InventoryUISlot sourceSlot = InventoryItemUI.draggedItem.originalParent.GetComponent<InventoryUISlot>();

        if (targetSlot == null || sourceSlot == null) return;
        if (targetSlot == sourceSlot) return;
        
        if (targetSlot.IsFilled())
        {
            InventoryItemUI.successfulSwap = true;
            //argetSlot.AssignItem(InventoryItemUI.draggedItem);
            sourceSlot.AssignItem(targetSlot.inventoryItem);
            targetSlot.AssignItem(InventoryItemUI.draggedItem);
            Debug.Log($"Successfully swapped Item {InventoryItemUI.draggedItem}");
        }
        else
        {
            //ActiveWeaponUI.successfulSwap = true;
            InventoryUI.Instance.SetItemInSlot(slotIndex, sourceSlot.inventoryItem.assignedItem);
            //AssignItem(targetSlot.inventoryItem);
            Destroy(sourceSlot.inventoryItem.gameObject);
            sourceSlot.DestroySlottedItem();
            Debug.Log($"MOVE: {sourceSlot.inventoryItem.assignedItem.itemName}");
            return;
        }
        
        Debug.Log($"Returning {InventoryItemUI.draggedItem?.assignedItem.itemName} to original slot.");
    }
}