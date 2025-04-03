using System.Collections;
using TMPro;
using UnityEngine;

public class ItemDrop : MonoBehaviour, IInteractable
{
    [Header("Item Details")]
    public BaseItem item;
    public RarityVisual itemRarityVisual;
    
    [Header("UI Elements")]
    public GameObject itemPriceTag;
    public ParticleSystem upgradeableFX;
    public ParticleSystem landingFX;
    public SpriteRenderer itemSprite;
    public TextMeshPro priceText;
    
    private Pedestal parentPedestal;
    public float detectionRadius = 1f; 
    private bool isInitialized = false;
    private bool isAcquired = false;
    private bool tooltipActive = false;
    private bool isShopItem = false;
    public bool eligibleForUpgrade = false;
    
    public void Initialize(BaseItem item, bool isInShop = false)
    {
        if (isInitialized) return;
        this.item = item;
        itemSprite.sprite = item.icon;
        priceText.text = $"${item.value}";
        itemPriceTag.SetActive(false);
        isInitialized = true;
        tooltipActive = false;
        isAcquired = false;
        isShopItem = isInShop;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
        itemRarityVisual.ApplyRarityMaterial(item.currentRarity);
        EventManager.Instance.StartListening("ItemInteraction", CheckForUpgrade);
        
        CheckForUpgrade(item.itemName);
        //StartCoroutine(DelayedCheck());
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.StopListening("ItemInteraction", CheckForUpgrade);
    }
    
    private IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to let Unity register physics interactions
        if (IsPlayerNearby())
        {
            Debug.Log($"[Delayed] Player is nearby: {IsPlayerNearby()}");
            ShowTooltip();
        }
        else
        {
            Debug.Log($"[Delayed] Player is NOT nearby: {IsPlayerNearby()}");
        }
    }
    
    private bool IsPlayerNearby()
    {
        LayerMask playerLayer = LayerMask.GetMask("Player"); // Ensure this is correct

        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (player != null)
        {
            //Debug.Log($"Detected Player: {player.gameObject.name}");
            return true;
        }
        return false;
    }
    
    public void SetPedestal(Pedestal pedestal)
    {
        parentPedestal = pedestal;
    }

    public void CheckForUpgrade(string itemName)
    {
        if (isAcquired) return;
        if (itemName == item.itemName)
        {
            if (InventoryManager.Instance.HasItem(itemName))
            {
                Debug.Log($"Inventory has item: {itemName}");
                eligibleForUpgrade = true;
                upgradeableFX.gameObject.SetActive(true);
            }
            else
            {
                eligibleForUpgrade = false;
                upgradeableFX.gameObject.SetActive(false);
            }
        }
    }

    public void DropItemReward(BaseItem item, Transform spawnPoint)
    {
        StartCoroutine(HandleItemDrop(item, spawnPoint));
    }

    private IEnumerator HandleItemDrop(BaseItem newItem, Transform spawnPoint)
    {
        Vector3 startPosition = spawnPoint.position;
        Vector3 endPosition = startPosition + new Vector3(0, -2f, 0);
        float peakHeight = 1.5f;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            currentPosition.y += Mathf.Sin(t * Mathf.PI) * peakHeight; // parabolic motion
            transform.position = currentPosition;

            yield return null;
        }

        if (landingFX != null)
        {
            landingFX.Play();
        }

        Debug.Log($"Launch complete, initializing..{newItem}");
        Initialize(newItem);
    }
    
    private void Update()
    {
        if(isAcquired) return;
        if (tooltipActive || !isAcquired)
        {
            if (!IsPlayerNearby())
            {
                if (isShopItem)
                {
                    HidePriceTag();
                }
                if (eligibleForUpgrade)
                {
                    InventoryUI.Instance.HighlightMatchingItem(item, false);
                }
                if (PlayerItemDetector.Instance.isInteractable(this))
                {
                    PlayerItemDetector.Instance.RemoveInteractableItem(this);
                    tooltipActive = false;
                }
            }
        }
    }
    
    public void Interact()
    {
        if (!isAcquired && isInitialized)
        {
            if (isShopItem)
            {
                TryBuyItem();
            }
            else
            {
                PickUpItem();
            }
        }
    }
    
    public void ShowTooltip()
    {
        if (!isInitialized) return;
        TooltipManager.Instance.SetWorldTooltip(this, "Item", eligibleForUpgrade);
    }
    
    public void HideTooltip()
    {
        InventoryUI.Instance.HighlightMatchingItem(item, false);
        TooltipManager.Instance.ClearWorldTooltip();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!tooltipActive && collision.CompareTag("Player"))
        {
            if (isShopItem)
            {
                Debug.Log($"Showing price tag.");
                ShowPriceTag();
            }
            if (eligibleForUpgrade)
            {
                InventoryUI.Instance.HighlightMatchingItem(item, true);
            }
            tooltipActive = true;
            PlayerItemDetector.Instance.AddInteractableItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tooltipActive && collision.CompareTag("Player"))
        {
            tooltipActive = false;
            if (isShopItem)
            {
                HidePriceTag();
            }
            if (eligibleForUpgrade)
            {
                InventoryUI.Instance.HighlightMatchingItem(item, false);
            }

            PlayerItemDetector.Instance.RemoveInteractableItem(this);
        }
    }

    private void PickUpItem()
    {
        if (isAcquired) return;
        if (!InventoryManager.Instance.DoesInventoryHaveSpace())
        {
            Debug.Log("Not enough inventory space.");
            return;
        }

        Debug.Log($"Picked up {item.itemName}!");
        AudioManager.Instance.PlayUISound("Item_PickUp");
        InventoryManager.Instance.AddItem(item, eligibleForUpgrade);
        ItemDatabase.Instance.UnregisterActiveItem(item.itemName);
        InventoryUI.Instance.HighlightMatchingItem(item, false);
        PlayerItemDetector.Instance.RemoveInteractableItem(this);
        //TooltipManager.Instance.HideTooltip();
        isAcquired = true;
        StartCoroutine(BounceAndDisappearCoroutine());
    }

    private bool TryBuyItem()
    {
        if (isAcquired) return false;
        isAcquired = true;
        if (!ResourceManager.Instance.HasEnoughFunds(item.value))
        {
            Debug.Log("Not enough funds to buy item.");
            return false;
        }

        if (!InventoryManager.Instance.DoesInventoryHaveSpace())
        {
            Debug.Log("Not enough inventory space.");
            return false;
        }

        Debug.Log($"Item '{item.itemName}' successfully bought.");
        AudioManager.Instance.PlayUISound("Shop_Purchase");
        ResourceManager.Instance.SpendCurrency(item.value);
        InventoryManager.Instance.AddItem(item, eligibleForUpgrade);
        ItemDatabase.Instance.UnregisterActiveItem(item.itemName);
        InventoryUI.Instance.HighlightMatchingItem(item, false);
        PlayerItemDetector.Instance.RemoveInteractableItem(this);

        StartCoroutine(BounceAndDisappearCoroutine());
        return true;
    }
    
    private void ShowPriceTag()
    {
        itemPriceTag.SetActive(true);
        StartCoroutine(FadeAndSlide(priceText, true));
    }

    private void HidePriceTag()
    {
        StartCoroutine(FadeAndSlide(priceText, false));
        itemPriceTag.SetActive(false);
    }

    private IEnumerator FadeAndSlide(TextMeshPro text, bool fadeIn)
    {
        Vector3 originalPosition = text.transform.localPosition;
        Vector3 targetPosition =
            fadeIn ? originalPosition + new Vector3(0, 0.25f, 0) : originalPosition - new Vector3(0, 0.25f, 0);
        float duration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            text.alpha = Mathf.Lerp(text.alpha, fadeIn ? 1f : 0f, t);
            text.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.alpha = fadeIn ? 1f : 0f;
        text.transform.localPosition = targetPosition;
    }

    private IEnumerator BounceAndDisappearCoroutine()
    {
        Debug.Log($"Item '{item.itemName}' bought and destroying");
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.4f;
        float duration = 0.1f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (parentPedestal != null)
        {
            parentPedestal.ResetPedestal();
        }

        Debug.Log($"Item '{item.itemName}' bought and destroyed");
        Destroy(gameObject);
    }
}