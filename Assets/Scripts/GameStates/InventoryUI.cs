using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image inventoryPanel;          // Panel holding inventory UI
    public GameObject inventorySlotPrefab;     // Prefab for individual slots
    public Transform inventorySlotContainer;   // Parent container for slots

    private List<InventoryUISlot> inventorySlots = new List<InventoryUISlot>();

    private InventoryManager inventoryManager;

    public void Initialize(InventoryManager manager)
    {
        inventoryManager = manager;
        EventManager.Instance.StartListening("InventoryUpdated", UpdateInventoryUI);
        SetupInitialSlots();
    }

    private void OnDestroy()
    {
        // Cleanup event listeners
        EventManager.Instance.StopListening("InventoryUpdated", UpdateInventoryUI);
        ClearAllSlots();
    }

    private void SetupInitialSlots()
    {
        if (inventorySlotPrefab == null)
        {
            Debug.LogError("InventorySlotPrefab is not assigned!");
            return;
        }

        if (inventorySlotContainer == null)
        {
            Debug.LogError("InventorySlotContainer is not assigned!");
            return;
        }

        for (int i = 0; i < inventoryManager.maxSlots; i++)
        {
            var slotObject = Instantiate(inventorySlotPrefab, inventorySlotContainer);

            if (slotObject == null)
            {
                Debug.LogError($"Failed to instantiate slot prefab for slot {i}");
                continue;
            }
            
            var slotUI = slotObject.GetComponent<InventoryUISlot>();
            inventorySlots.Add(slotUI);//adding references
        }
    }

    public void UpdateInventoryUI(List<ItemData> items)
    {
        foreach (var slot in inventorySlots)
        {
            slot.ClearSlot();
        }

        for (int i = 0; i < items.Count && i < inventorySlots.Count; i++)
        {
            inventorySlots[i].Initialize(items[i]);
        }
    }

    public void ClearAllSlots()
    {
        foreach (var slot in inventorySlots)
        {
            slot.ClearSlot();
        }
    }
}