using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Tester")]
    public GameObject shopItemCardPrefab; // Prefab reference for shop items
    private ShopUI shopUI; // Reference to the ShopUI

    [Header("Shop Manager")]
    public GameObject itemPrefab;
    public Pedestal[] pedestals;

    public void Initialize()
    {
        EventManager.Instance.StartListening("TestShop", TriggerShopTester);
        EventManager.Instance.StartListening("TravelDeparture", HandleShopPreparation);
        //EventManager.Instance.StartListening("TravelArrival", HandleShopEntry);
        shopUI = FindObjectOfType<ShopUI>();
        shopUI.Initialize(this);
    }

    public void OnDestroy()
    {
        EventManager.Instance.StopListening("TestShop", TriggerShopTester);
        EventManager.Instance.StopListening("TravelDeparture", HandleShopPreparation);
        //EventManager.Instance.StopListening("TravelArrival", HandleShopEntry);
    }

    public void HandleShopEntry(string destination)
    {
        Debug.Log("Shop Entry Logic");
    }

    public void HandleShopPreparation(string destination)
    {
        if (destination != "Shop")
        {
            foreach (var pedestal in pedestals)
            {
                if (pedestal.transform.childCount > 0)
                {
                    pedestal.DestroyItem();
                }
            }
        }
        if (destination == "Shop")
        {
            foreach (var pedestal in pedestals)
            {
                ItemData randomItem = ItemDatabase.Instance.GetRandomItem();
                SpawnItemOnPedestal(pedestal, randomItem);
            }
        }
    }

    public void TriggerShopTester()
    {
        List<ItemData> randomItems = ItemDatabase.Instance.ShowRandomItems(3);
        shopUI.OpenShop();
        shopUI.DisplayItems(randomItems);
    }

    public bool TryBuyItem(ItemData selectedItem)
    {
        if (!CurrencyManager.Instance.HasEnoughFunds(selectedItem.price))
        {
            Debug.Log("Not enough funds to buy item.");
            return false;
        }

        if (!InventoryManager.Instance.DoesInventoryHaveSpace())
        {
            Debug.Log("Not enough inventory space.");
            return false;
        }

        // Deduct funds and add item to inventory
        AudioManager.Instance.PlayUISound("Shop_Purchase");
        CurrencyManager.Instance.SpendCurrency(selectedItem.price);
        InventoryManager.Instance.AddItem(selectedItem);

        // Trigger the ItemBought event
        EventManager.Instance.TriggerEvent("ItemBought", selectedItem);

        Debug.Log($"Item '{selectedItem.itemName}' successfully bought.");
        return true;
    }

    public void RerollItems()
    {
        AudioManager.Instance.PlayUISound("UI_Reroll");
        //OpenShop();
    }

    private void SpawnItemOnPedestal(Pedestal pedestal, ItemData itemData)
    {
        if (itemData == null || itemPrefab == null) return;

        // Instantiate a new item GameObject
        GameObject newItem = Instantiate(itemPrefab, pedestal.itemSpawnPoint);

        // Set its position to the pedestal's position
        newItem.transform.localPosition = Vector3.zero;

        // Initialize the item with the given ItemData
        Item itemComponent = newItem.GetComponent<Item>();

        if (itemComponent != null)
        {
            itemComponent.Initialize(itemData);
        }
        Debug.Log($"Item '{itemData}' set.");
        pedestal.SetItem(itemData);
    }

    public void RerollShop()
    {
        foreach (var pedestal in pedestals)
        {
            Pedestal pedestalScript = pedestal.GetComponent<Pedestal>();
            pedestalScript.RerollItem();
        }
    }
}