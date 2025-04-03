using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    
    [Header("Inventory UI Setup")]
    public GameObject inventorySlotPrefab;  // Prefab for static slots
    public GameObject inventoryItemPrefab;  // Prefab for slotted items
    public Transform inventorySlotContainer; // Parent for slots

    private List<InventoryUISlot> inventorySlots = new List<InventoryUISlot>();

    private InventoryManager inventoryManager;
    private bool setupComplete = false;

    public void Initialize()
    {
        inventoryManager = InventoryManager.Instance;
        SetupInitialSlots(inventoryManager.inventorySlots);
    }

    private void OnDestroy()
    {
        ClearAllSlots();
    }
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetupInitialSlots(int totalSlots)
    {
        if (setupComplete) return;
        setupComplete = true;
        
        if (inventorySlotPrefab == null || inventorySlotContainer == null)
        {
            Debug.LogError("InventorySlotPrefab or InventorySlotContainer is not assigned!");
            return;
        }
        
        for (int i = 0; i < totalSlots; i++)
        {
            var slotObject = Instantiate(inventorySlotPrefab, inventorySlotContainer);
            if (slotObject == null)
            {
                Debug.LogError($"Failed to instantiate slot prefab for slot {i}");
                continue;
            }

            InventoryUISlot slotUI = slotObject.GetComponent<InventoryUISlot>();
            slotUI.InitializeItemSlot(i);
            inventorySlots.Add(slotUI);
        }
        Debug.Log($"Inventory UI initialized with {totalSlots} slots.");
    }
    
    public void PlaceItemIntoInventory (BaseItem selectedItem, bool loadSave = false)
    {
        foreach (InventoryUISlot slot in inventorySlots)
        {
            if (!slot.IsFilled()) // Check if the slot is empty
            {
                if (selectedItem == null)
                {
                    Debug.Log($"inventoryItemPrefab is null: {inventoryItemPrefab}");
                }
                GameObject itemObject = Instantiate(inventoryItemPrefab, slot.transform);
                InventoryItemUI newItemUI = itemObject.GetComponent<InventoryItemUI>();
                if (itemObject == null)
                {
                    Debug.Log($"itemObject is null: {itemObject}");
                }
                if (newItemUI == null)
                {
                    Debug.Log($"new item UI is null: {newItemUI}");
                }
                newItemUI.InitializeItem(selectedItem);
                slot.AssignItem(newItemUI);

                Debug.Log($"Placed {selectedItem.itemName} into inventory slot.");
                return; // Exit after placing the item
            }
        }
        Debug.LogWarning("No available inventory slots!");
    }
    
    public void SetItemInSlot(int slotIndex, BaseItem item) // Loading items from save
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
        {
            Debug.LogError($"Invalid inventory slot index: {slotIndex}");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning($"Attempted to set a null item in slot {slotIndex}.");
            return;
        }
        
        InventoryUISlot targetSlot = inventorySlots[slotIndex];
        GameObject itemObject = Instantiate(inventoryItemPrefab, targetSlot.transform);
        InventoryItemUI newItemUI = itemObject.GetComponent<InventoryItemUI>();
        
        newItemUI.InitializeItem(item);
        targetSlot.AssignItem(newItemUI);
        Debug.Log($"Placed {item.itemName} into slot {slotIndex}.");
    }
    
    public void HighlightMatchingItem(BaseItem hoveredItem, bool highlight)
    {
        foreach (InventoryUISlot slot in inventorySlots)
        {
            if (slot.IsFilled() && slot.inventoryItem.GetAssignedItem().itemName == hoveredItem.itemName)
            {
                slot.HighlightSlot(highlight);
            }
        }
    }
    
    public void UpgradeItemInInventorySlot(BaseItem selectedItem)
    {
        foreach (InventoryUISlot slot in inventorySlots)
        {
            if (slot.IsFilled() && slot.inventoryItem.GetAssignedItem().itemName == selectedItem.itemName)
            {
                slot.inventoryItem.UpgradeItem();
                slot.HighlightSlot(false);
            }
        }
    }
    
    public List<InventoryUISlot> GetInventorySlots()
    {
        return inventorySlots;
    }

    public void ClearAllSlots()
    {
        foreach (var slot in inventorySlots)
        {
            slot.DestroySlottedItem();
        }

        setupComplete = false;
    }
}