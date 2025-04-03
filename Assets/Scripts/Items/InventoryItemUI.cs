using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IInteractable
{
    public Image itemIcon;
    public UIRarityVisual uiRarityVisual;
    public BaseItem assignedItem;
    
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    public static InventoryItemUI draggedItem;

    private bool tooltipActive = false;
    public static bool successfulSwap = false;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void InitializeItem(BaseItem item)
    {
        assignedItem = item;
        itemIcon.gameObject.SetActive(true);
        uiRarityVisual.ApplyUIRarityVisual(item.currentRarity);
        
        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            Debug.Log($"Item placed: {assignedItem}");
        }
    }
    
    public void UpgradeItem()
    {
        if (assignedItem != null)
        {
            Debug.Log("Upgraded Item In Slot");
            assignedItem.Upgrade();
            InitializeItem(assignedItem);
        }
    }

    public BaseItem GetAssignedItem() => assignedItem;
    
    public void ClearItem()
    {
        assignedItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipActive) return;
        tooltipActive = true;
        Debug.Log("Enter pointer");
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!tooltipActive) return;
        tooltipActive = false;
        Debug.Log("Exit pointer");
        HideTooltip();
    }
    
    public void ShowTooltip()
    {
        Debug.Log("Show tooltip");
        TooltipManager.Instance.SetHoverTooltip(this, "ItemSlot"); 
        Debug.Log("Show tooltip");
    }

    public void HideTooltip()
    {
        TooltipManager.Instance.ClearHoverTooltip();
    }

    public void Interact()
    {
        if (TooltipManager.Instance.GetActiveTooltip() != this) return;
        Debug.Log($"Inspecting inventory item: {assignedItem.itemName}");
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedItem = this;
        originalParent = transform.parent;
        originalPosition = transform.localPosition;

        transform.SetParent(transform.root); // Move to root so it's above everything
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Follow cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"is successful: {successfulSwap}");
        if (successfulSwap) 
        {
            Debug.Log($"Successfully swapped Item {assignedItem}");
            draggedItem = null;
            canvasGroup.blocksRaycasts = true;
            successfulSwap = false;
            return;
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Still over UI but not a valid drop
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
            Debug.Log($"Returning {assignedItem?.itemName} to original slot (Invalid Drop).");
        }
        else
        {
            // Dropped into game world or nothing
            Debug.Log($"Dropped {assignedItem?.itemName} into the world");
            InventoryManager.Instance.DiscardItem(assignedItem);
            Destroy(gameObject);
        }
        
        //transform.SetParent(originalParent);
        //transform.localPosition = originalPosition;
        Debug.Log($"Returning {assignedItem?.itemName} to original slot (Failed Drop).");
        canvasGroup.blocksRaycasts = true;
        draggedItem = null;
    }
}
