using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    public GameObject shopPanel;               // The shop panel containing GridLayoutGroup
    public Transform shopItemContainer;        // Holds the GridLayoutGroup
    public Button closeShopButton;             // Button to close the shop
    public Button rerollButton;

    private ShopEncounter shopEncounter;           // Reference to the ShopManager

    public void Initialize(ShopEncounter manager)
    {
        shopEncounter = manager;
        shopPanel.SetActive(false);
        closeShopButton.onClick.AddListener(CloseShop);
        rerollButton.onClick.AddListener(RerollItems);
    }

    private void OnDestroy()
    {
        closeShopButton.onClick.RemoveListener(CloseShop);
        rerollButton.onClick.RemoveListener(RerollItems);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenShop()
    {
        Debug.Log("Openshop");
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        AudioManager.Instance.PlayUISound("UI_Close");
        shopPanel.SetActive(false);
    }

    public void DisplayItems(List<BaseItem> items)
    {
        // Clear previous items
        foreach (Transform child in shopItemContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate new items
        foreach (var item in items)
        {
            CreateShopItemCard(item);
        }
    }

    private void CreateShopItemCard(BaseItem item)
    {
        GameObject itemCard = Instantiate(shopEncounter.shopItemCardPrefab, shopItemContainer);

        var itemNameText = itemCard.transform.Find("ItemTitle")?.GetComponent<TextMeshProUGUI>();
        var itemIconImage = itemCard.transform.Find("ItemIcon")?.GetComponent<Image>();
        var itemDescriptionText = itemCard.transform.Find("ItemDescription")?.GetComponent<TextMeshProUGUI>();
        var itemCurrencyText = itemCard.transform.Find("ItemPrice")?.GetComponent<TextMeshProUGUI>();
        var itemButton = itemCard.GetComponent<Button>();

        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemIconImage != null) itemIconImage.sprite = item.icon;
        if (itemDescriptionText != null) itemDescriptionText.text = item.description;
        if (itemCurrencyText != null) itemCurrencyText.text = $"${item.value}";

        //itemButton.onClick.AddListener(() => OnBuyButtonPressed(item, itemCard));
    }

   /* public void OnBuyButtonPressed(BaseItem selectedItem, GameObject itemCard)
    {
        bool successfulPurchase = shopManager.TryBuyItem(selectedItem);
        if (successfulPurchase)
        {
            MarkItemAsSold(itemCard);
        }
        else
        {
            Debug.Log("Not enough funds or inventory space.");
        }
    }*/

    private void MarkItemAsSold(GameObject itemCard)
    {
        Material desaturationMaterial = Resources.Load<Material>("Materials/DesaturationMaterial");

        if (desaturationMaterial == null)
        {
            Debug.LogError("Not found grayscale mat");
            return;
        }

        foreach (Image img in itemCard.GetComponentsInChildren<Image>())
        {
            img.material = desaturationMaterial;
        }

        TextMeshProUGUI soldText = itemCard.transform.Find("SoldText")?.GetComponent<TextMeshProUGUI>();
        if (soldText != null)
        {
            soldText.text = "SOLD";
            soldText.gameObject.SetActive(true);
        }

        Button itemButton = itemCard.GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.interactable = false;
        }
    }
    
    private void RerollItems()
    {
        //shopManager.RerollItems();
    }
}
