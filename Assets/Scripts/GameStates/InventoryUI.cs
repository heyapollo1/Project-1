using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour

{
    public static InventoryUI Instance { get; private set; }
    
    [Header("Inventory UI Setup")]
    public GameObject inventorySlotPrefab;  // Prefab for static slots
    public GameObject itemUIPrefab;  // Prefab for slotted items
    public Transform inventorySlotContainer; // Parent for slots
    
    public Dictionary<int, InventoryUISlot> inventorySlots = new Dictionary<int, InventoryUISlot>();

    private InventoryManager inventoryManager;
    private bool setupComplete = false;

    public void Initialize()
    {
        //EventManager.Instance.StartListening("HighlightInventoryUpgrade", HighlightInventoryMatch);
        //EventManager.Instance.StartListening("UpgradeInventoryItem", UpgradeItemInInventory);
        
        inventoryManager = InventoryManager.Instance;
        SetupInitialSlots(inventoryManager.inventorySlots);
    }

    private void OnDestroy()
    {
        //EventManager.Instance.StopListening("HighlightInventoryUpgrade", HighlightInventoryMatch);
        //EventManager.Instance.StopListening("UpgradeInventoryItem", UpgradeItemInInventory);
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
            Debug.LogWarning("InventorySlotPrefab or InventorySlotContainer is not assigned!");
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
            inventorySlots.Add(i, slotUI);
        }
        Debug.Log($"Inventory UI initialized with {totalSlots} slots.");
    }
    
    public int GetFreeInventorySlot()
    {
        foreach (var slot in inventorySlots.Values)
        {
            if (!slot.IsFilled())  return slot.slotIndex;
        }
        return -1;
    }
    
    public void SetItemInSlot(int slotIndex, BaseItem item = null, WeaponInstance weapon = null)
    {
        bool isWeapon = item == null;
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
        {
            Debug.LogError($"Invalid inventory slot index: {slotIndex}");
            return;
        }
        InventoryUISlot targetSlot = inventorySlots[slotIndex];
        GameObject itemObject = Instantiate(itemUIPrefab, targetSlot.transform);
        ItemUI newItemUI = itemObject.GetComponent<ItemUI>();
        
        newItemUI.InitializeItemUI(
            isWeapon ? null : item,
            isWeapon ? weapon : null, false);
        
        targetSlot.AssignItemToInventorySlot(newItemUI, isWeapon);
    }

    public void DestroySlottedItem(string uniqueID)
    {
        GetInventorySlotByItemID(uniqueID).Clear();
    }
    
    public Dictionary<int, InventoryUISlot> GetInventorySlots()
    {
        return inventorySlots;
    }
    
    public InventoryUISlot GetInventorySlotByIndex(int id)
    {
        return inventorySlots.TryGetValue(id, out var ui) ? ui : null;
    }
    
    public InventoryUISlot GetInventorySlotByItemID(string id)
    {
        foreach (var slot in inventorySlots.Values)
        {
            if (slot.IsFilled() && slot.GetSlottedItemID() == id)
            {
                return slot;
            }
        }
        return null;
    }
    
    public ItemUI GetSlottedItemByItemID (string id)
    {
        foreach (var slot in inventorySlots.Values)
        {
            if (slot.GetItemInSlot().uniqueID == id) return slot.slottedItem;
        }
        return null;
    }

    private void ClearSlot(int index)
    {
        if (inventorySlots.TryGetValue(index, out var slot))
        {
            slot.Clear();
        }
    }
    
    private void ClearAllSlots()
    {
        foreach (var (_, slot) in inventorySlots)
        {
            if (slot.IsFilled()) slot.Clear();
        }
    }
}