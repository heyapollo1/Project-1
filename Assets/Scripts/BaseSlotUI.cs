/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType { Inventory, Hand }

public abstract class BaseSlotUI : MonoBehaviour, IDropHandler
{
    public SlotType slotType;
    public bool isWeaponSlot;
    public ItemUI slottedItem;

    public abstract bool IsFilled();
    public abstract void AssignItem(ItemUI item);
    public abstract void ClearSlot();

    public virtual void OnDrop(PointerEventData eventData)
    {
        var draggedItem = ItemUI.CurrentDraggedItem;
        if (draggedItem == null || draggedItem == slottedItem) return;

        BaseSlotUI sourceSlot = draggedItem.originalParent.GetComponent<BaseSlotUI>();
        if (sourceSlot == null || !CanAcceptItem(draggedItem)) return;

        // If this slot is filled, swap
        if (slottedItem != null)
        {
            if (!sourceSlot.CanAcceptItem(slottedItem)) return;

            var temp = slottedItem;
            sourceSlot.AssignItem(temp);
        }
        else
        {
            sourceSlot.ClearSlot();
        }

        AssignItem(draggedItem);
    }
}*/