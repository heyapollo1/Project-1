using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public int maxSlots = 8;
    private List<ItemData> itemsInInventory = new List<ItemData>();

    private InventoryUI inventoryUI;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //EventManager.Instance.StartListening("ItemBought", AddItem);
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
    }

    private void OnSceneLoaded(string scene)
    {
        if (scene == "GameScene")
        {
            Debug.LogWarning("InventoryManagerLoaded!");
            inventoryUI = FindObjectOfType<InventoryUI>();
            inventoryUI.Initialize(this);
            inventoryUI.UpdateInventoryUI(itemsInInventory); // Sync current inventory state
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
    }

    public void RemoveItem(ItemData selectedItem)
    {
        itemsInInventory.Remove(selectedItem);
        EventManager.Instance.TriggerEvent("InventoryUpdated", itemsInInventory);
    }

    public bool DoesInventoryHaveSpace()
    {
        return itemsInInventory.Count < maxSlots;
    }

    public void AddItem(ItemData selectedItem)
    {
        if (DoesInventoryHaveSpace())
        {
            selectedItem.Apply(PlayerStatManager.Instance);
            itemsInInventory.Add(selectedItem);
            EventManager.Instance.TriggerEvent("InventoryUpdated", itemsInInventory);
        }
        else
        {
            Debug.LogWarning("No space in inventory!");
        }
    }

    public bool HasItem(ItemData item)
    {
        return itemsInInventory.Contains(item);
    }

    public void ResetInventory()
    {
        itemsInInventory.Clear();
        EventManager.Instance.TriggerEvent("InventoryUpdated", itemsInInventory);
    }
}