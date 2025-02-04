using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : BaseManager
{
    public static ItemDatabase Instance { get; private set; }
    public override int Priority => 30;

    public List<ItemData> availableItems = new List<ItemData>();
    private HashSet<ItemData> activeItems = new HashSet<ItemData>();

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("PlayerReady", LoadItems);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("PlayerReady", LoadItems);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void LoadItems()
    {
        Sprite appleIcon = Resources.Load<Sprite>("Items/AppleIcon");
        Sprite rubyIcon = Resources.Load<Sprite>("Items/RubyIcon");
        Sprite witchPointIcon = Resources.Load<Sprite>("Items/WitchPointIcon");
        Sprite brassLinkIcon = Resources.Load<Sprite>("Items/brassLinkIcon");
        Sprite ghostWindIcon = Resources.Load<Sprite>("Items/GhostWindIcon");
        Sprite potionIcon = Resources.Load<Sprite>("Items/PotionIcon");
        Sprite cloverIcon = Resources.Load<Sprite>("Items/CloverIcon");
        Sprite madDogIcon = Resources.Load<Sprite>("Items/MadDogIcon");
        Sprite toothIcon = Resources.Load<Sprite>("Items/ToothIcon");
        Sprite swordIcon = Resources.Load<Sprite>("Items/SwordIcon");
        Sprite fullMetalJacketIcon = Resources.Load<Sprite>("Items/FullMetalJacketIcon");
        Sprite brawlersIcon = Resources.Load<Sprite>("Items/BrawlersIcon");
        Sprite topazIcon = Resources.Load<Sprite>("Items/TopazIcon");
        Sprite candleIcon = Resources.Load<Sprite>("Items/CandleIcon");

        availableItems.Add(new AppleItem(appleIcon));
        availableItems.Add(new RubyItem(rubyIcon));
        availableItems.Add(new WitchPointItem(witchPointIcon));
        availableItems.Add(new BrassLinkItem(brassLinkIcon));
        availableItems.Add(new GhostWindItem(ghostWindIcon));
        availableItems.Add(new PotionItem(potionIcon));
        availableItems.Add(new CloverItem(cloverIcon));
        availableItems.Add(new MadDogItem(madDogIcon));
        availableItems.Add(new ToothItem(toothIcon));
        availableItems.Add(new SwordItem(swordIcon));
        availableItems.Add(new FullMetalJacketItem(fullMetalJacketIcon));
        availableItems.Add(new BrawlersItem(brawlersIcon));
        availableItems.Add(new TopazItem(topazIcon));
        availableItems.Add(new CandleItem(candleIcon));
    }

    public List<ItemData> ShowRandomItems(int count)
    {
        List<ItemData> randomItems = new List<ItemData>();
        List<ItemData> availableItemsCopy = new List<ItemData>(availableItems);

        availableItemsCopy.RemoveAll(item => activeItems.Contains(item) || InventoryManager.Instance.HasItem(item));

        for (int i = 0; i < count; i++)
        {
            if (availableItemsCopy.Count == 0) break;
            int randomIndex = Random.Range(0, availableItemsCopy.Count);
            ItemData selectedItem = availableItemsCopy[randomIndex];
            randomItems.Add(selectedItem);
            availableItemsCopy.RemoveAt(randomIndex);

            activeItems.Add(selectedItem);
        }

        return randomItems;
    }

    public ItemData GetRandomItem()
    {
        // Create list of eligible items
        List<ItemData> eligibleItems = new List<ItemData>(availableItems);

        eligibleItems.RemoveAll(item => activeItems.Contains(item) || InventoryManager.Instance.HasItem(item));

        if (eligibleItems.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, eligibleItems.Count);
        ItemData selectedItem = eligibleItems[randomIndex];

        // Mark the item as active
        activeItems.Add(selectedItem);

        return selectedItem;
    }

    public void RemoveActiveItem(ItemData item)
    {
        activeItems.Remove(item);
    }

    public void AddNewItem(ItemData newItem)
    {
        availableItems.Add(newItem);
        Debug.Log($"Added item: {newItem.itemName}");
    }

    public ItemData GetItem(string itemName)
    {
        List<ItemData> eligibleItems = new List<ItemData>(availableItems);

        eligibleItems.RemoveAll(item => activeItems.Contains(item) || InventoryManager.Instance.HasItem(item));

        if (eligibleItems.Count == 0)
        {
            return null;
        }

        foreach (ItemData item in eligibleItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }

    public void ResetItems()
    {
        availableItems.Clear();
        LoadItems();
    }

    public bool AreThereItems()
    {
        return activeItems != null && activeItems.Count > 0;
    }
}
