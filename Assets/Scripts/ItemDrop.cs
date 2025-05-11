using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDrop : MonoBehaviour, IInteractable
{
    [Header("Item Details")]
    public ItemPayload dropPayload;
    public RarityVisual itemRarityVisual;
    public Rarity rarity;
    public string name;
    public string uniqueID;
    public int value;
    
    [Header("UI Elements")]
    public GameObject priceTag;
    public ParticleSystem upgradeableFX;
    public ParticleSystem landingFX;
    public SpriteRenderer spriteRenderer;
    public TextMeshPro priceText;
    private Pedestal parentPedestal;

    public float detectionRadius = 1f;
    private bool isInitialized = false;
    private bool isAcquired = false;
    private bool tooltipActive = false;
    private bool isShopItem = false;
    
    public bool isWeapon = false;
    private LayerMask playerLayer;
    private ItemTracker itemTracker;
    
    public void InitializeDroppedItem(ItemPayload payload, bool isInShop = false)
    {
        if (isInitialized) return;
        isWeapon = payload.isWeapon;
        name = isWeapon ? payload.weaponScript.weaponTitle : payload.itemScript.itemName;
        uniqueID = isWeapon ? payload.weaponScript.uniqueID : payload.itemScript.uniqueID;
        rarity = isWeapon ? payload.weaponScript.rarity : payload.itemScript.rarity;
        priceText.text = isWeapon ? $"${payload.weaponScript.value}" : $"${payload.itemScript.value}";
        spriteRenderer.sprite = isWeapon ? payload.weaponScript.weaponIcon : payload.itemScript.icon;
        dropPayload = payload;
        
        itemRarityVisual.ApplyRarityMaterial(rarity);
        isShopItem = isInShop;

        priceTag.SetActive(false);
        isInitialized = true;
        tooltipActive = false;
        isAcquired = false;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
        playerLayer = LayerMask.GetMask("Player");
        itemTracker = ItemTracker.Instance;
        StartCoroutine(CheckIfPlayerIsNearby());
    }
    
    private bool IsPlayerNearby()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player != null)
        {
            return true;
        }
        return false;
    }
    
    public void SetPedestal(Pedestal pedestal)  => parentPedestal = pedestal;
    
    public void DropItemReward(Transform spawnPoint, ItemPayload payload)
    {
        if (payload != null) StartCoroutine(HandleDrop(spawnPoint, payload));
    }

    private IEnumerator CheckIfPlayerIsNearby()
    {
        yield return null;
        if (IsPlayerNearby())
        {
            if (!tooltipActive)
            {
                if (isShopItem) ShowPriceTag();
                tooltipActive = true;
                PlayerItemDetector.Instance.AddInteractableItem(this);
            }
        }
    }

    private IEnumerator HandleDrop(Transform spawnPoint, ItemPayload payload)
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
        
        if (landingFX != null) landingFX.Play();
        if (payload != null) InitializeDroppedItem(payload);
    }
    
    private void Update()
    {
        if (!isAcquired)
        {
            if (!IsPlayerNearby() && PlayerItemDetector.Instance.isInteractable(this))
            {
                PlayerItemDetector.Instance.RemoveInteractableItem(this);
                tooltipActive = false;
                
                if (isShopItem) HidePriceTag();
            }
        }
    }
    
    public void Interact()
    {
        if (!isAcquired && isInitialized)
        {
            GetItem();
        }
    }
    
    public void ShowTooltip()
    {
        if (!isInitialized) return;
        TooltipManager.Instance.SetWorldTooltip(this, "ItemDrop");
    }
    
    public void HideTooltip()
    {
        TooltipManager.Instance.ClearWorldTooltip();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!tooltipActive && collision.CompareTag("Player"))
        {
            if (isShopItem) ShowPriceTag();
            tooltipActive = true;
            PlayerItemDetector.Instance.AddInteractableItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tooltipActive && collision.CompareTag("Player"))
        {
            if (isShopItem) HidePriceTag();
            tooltipActive = false;
            PlayerItemDetector.Instance.RemoveInteractableItem(this);
        }
    }

    private void GetItem()
    {
        if (isAcquired) return;
        if (isShopItem) if (!ResourceManager.Instance.HasEnoughFunds(value)) return;
        if (!InventoryManager.Instance.isThereSpaceInInventory() &&
            !WeaponManager.Instance.isThereSpaceInLoadouts()) return;
        
        itemTracker.UnassignItemFromTracker(uniqueID);
        
        if (isWeapon)
        {
            if (WeaponManager.Instance.isThereSpaceInLoadouts())
            {
                WeaponManager.Instance.AddItemToLoadout(dropPayload);
            }
            else if (InventoryManager.Instance.isThereSpaceInInventory())
            {
                InventoryManager.Instance.AddItemToInventory(dropPayload);
            }
        }
        else
        {
            if (InventoryManager.Instance.isThereSpaceInInventory())
            {
                InventoryManager.Instance.AddItemToInventory(dropPayload);
            }
            else if (WeaponManager.Instance.isThereSpaceInLoadouts())
            {
                WeaponManager.Instance.AddItemToLoadout(dropPayload);
            }
        }
        
        if (isWeapon && TutorialManager.Instance.IsTutorialActive() && !TutorialManager.Instance.hasPickedUpTutorialWeapon)
        {
            TutorialManager.Instance.TutorialWeaponAcquired();
        }
        
        if (isShopItem)
        {
            ResourceManager.Instance.SpendCurrency(value);
            AudioManager.Instance.PlayUISound("Shop_Purchase");
        }
        
        AudioManager.Instance.PlayUISound("Item_PickUp");
        ItemCollected();
    }
    
    private void ShowPriceTag()
    {
        priceTag.SetActive(true);
        StartCoroutine(FadeAndSlide(priceText, true));
    }

    private void HidePriceTag()
    {
        StartCoroutine(FadeAndSlide(priceText, false));
        priceTag.SetActive(false);
    }

    private void ItemCollected()
    {
        PlayerItemDetector.Instance.RemoveInteractableItem(this);
        isAcquired = true;
        StartCoroutine(BounceAndDisappearCoroutine());
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
        Debug.Log($"Item '{name}' collected and destroying");
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
        Destroy(gameObject);
    }
}
    /*public void Initialize(BaseItem item = null, WeaponInstance weapon = null, bool isInShop = false)
    {
        if (isInitialized) return;
        isWeapon = item == null;
        name = isWeapon ? weapon.weaponTitle : item.itemName;
        rarity = isWeapon ? weapon.rarity : item.currentRarity;
        uniqueID = isWeapon ? weapon.uniqueID : item.uniqueID;
        priceText.text = isWeapon ? $"${weapon.value}" : $"${item.value}";
        spriteRenderer.sprite = isWeapon ? weapon.weaponIcon : item.icon;
        itemDrop = isWeapon ? null : item;
        weaponDrop = isWeapon ? weapon : null;
        
        
        itemRarityVisual.ApplyRarityMaterial(rarity);
        CheckForUpgrade(name);
        isShopItem = isInShop;

        priceTag.SetActive(false);
        isInitialized = true;
        tooltipActive = false;
        isAcquired = false;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
        playerLayer = LayerMask.GetMask("Player");
        EventManager.Instance.StartListening("ItemInteraction", CheckForUpgrade);
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.StopListening("ItemInteraction", CheckForUpgrade);
    }
    
    private bool IsPlayerNearby()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player != null)
        {
            return true;
        }
        return false;
    }
    
    public void SetPedestal(Pedestal pedestal)
    {
        parentPedestal = pedestal;
    }

    private bool WillUpgrade()
    {
        return eligibleForInventoryUpgrade || eligibleForLoadoutUpgrade;
    }
    
    public void CheckForUpgrade(string checkedItem)
    {
        if (isAcquired) return;
        if (checkedItem != name) return;
        
        bool hasMatchInHand = isWeapon
            ? WeaponManager.Instance.HasWeaponMatch(name, rarity)
            : WeaponManager.Instance.HasItemMatch(name, rarity);
        
        bool hasMatchInInventory = isWeapon
            ? InventoryManager.Instance.HasWeaponMatch(name, rarity)
            : InventoryManager.Instance.HasItemMatch(name, rarity);

        if (hasMatchInHand)
        {
            eligibleForLoadoutUpgrade = true;
            upgradeableFX.gameObject.SetActive(true);
        }
        else if (hasMatchInInventory)
        {
            eligibleForInventoryUpgrade = true;
            upgradeableFX.gameObject.SetActive(true);
        }
        else
        {
            eligibleForInventoryUpgrade = false;
            eligibleForLoadoutUpgrade = false;
            upgradeableFX.gameObject.SetActive(false);
        }
    }
    
    public void DropItemReward(Transform spawnPoint, BaseItem newItem = null, WeaponInstance newWeapon = null)
    {
        if (newItem != null)
        {
            StartCoroutine(HandleItemDrop(spawnPoint, newItem));
        }
        else
        {
            StartCoroutine(HandleItemDrop(spawnPoint, null, newWeapon));
        }
    }

    private IEnumerator HandleItemDrop(Transform spawnPoint, BaseItem newItem = null, WeaponInstance newWeapon = null)
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
        
        if (landingFX != null) landingFX.Play();
        Debug.Log($"Launch complete, initializing..{newItem}");
        if (newItem != null) Initialize(newItem);
        else Initialize(null, newWeapon);
    }
    
    private void Update()
    {
        if (!isAcquired)
        {
            if (!IsPlayerNearby() && PlayerItemDetector.Instance.isInteractable(this))
            {
                PlayerItemDetector.Instance.RemoveInteractableItem(this);
                tooltipActive = false;
                
                if (isShopItem) HidePriceTag();
                if (WillUpgrade()) HightlightUpgrade(false);
            }
        }
    }
    
    private void HightlightUpgrade(bool isActive)
    {
        if (eligibleForInventoryUpgrade)
        {
            EventManager.Instance.TriggerEvent("HighlightInventoryUpgrade", uniqueID, isActive, isWeapon);
        }
        else if (eligibleForLoadoutUpgrade)
        {
            EventManager.Instance.TriggerEvent("HighlightLoadoutUpgrade", uniqueID, isActive, isWeapon);
        }
    }
    
    public void Interact()
    {
        if (!isAcquired && isInitialized)
        {
            GetItem();
        }
    }
    
    public void ShowTooltip()
    {
        if (!isInitialized) return;
        TooltipManager.Instance.SetWorldTooltip(this, "ItemDrop", WillUpgrade());
    }
    
    public void HideTooltip()
    {
        if (!isAcquired && !tooltipActive) HightlightUpgrade(false);
        TooltipManager.Instance.ClearWorldTooltip();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!tooltipActive && collision.CompareTag("Player"))
        {
            if (isShopItem) ShowPriceTag();
            if (WillUpgrade()) HightlightUpgrade(true);

            tooltipActive = true;
            PlayerItemDetector.Instance.AddInteractableItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tooltipActive && collision.CompareTag("Player"))
        {
            if (isShopItem) HidePriceTag();
            if (WillUpgrade()) HightlightUpgrade(false);
            tooltipActive = false;
            PlayerItemDetector.Instance.RemoveInteractableItem(this);
        }
    }

    private void GetItem()
    {
        if (isAcquired) return;
        if (!InventoryManager.Instance.isThereSpaceInInventory() &&
            !WeaponManager.Instance.isThereSpaceInLoadouts()) return;
        if (isShopItem) if (!ResourceManager.Instance.HasEnoughFunds(value)) return;
        
        if (WillUpgrade())
        {
            string upgradeTargetID;
            if (eligibleForInventoryUpgrade)
            {
                upgradeTargetID = isWeapon ? InventoryManager.Instance.GetWeaponMatchID(name, rarity) : InventoryManager.Instance.GetItemMatchID(name, rarity);
                InventoryManager.Instance.UpgradeItemInInventory(upgradeTargetID, isWeapon);
            }
            else
            {
                upgradeTargetID = isWeapon ? WeaponManager.Instance.GetWeaponMatchID(name, rarity) : WeaponManager.Instance.GetItemMatchID(name, rarity);
                WeaponManager.Instance.UpgradeItemInLoadouts(upgradeTargetID, isWeapon);
            }
            
            if (isShopItem)
            {
                ResourceManager.Instance.SpendCurrency(value);
                AudioManager.Instance.PlayUISound("Shop_Purchase");
            }
            AudioManager.Instance.PlayUISound("Item_PickUp");
            ItemCollected();
            return;
        }
        
        var payload = new ItemPayload()
        {
            weaponScript = isWeapon ? weaponDrop : null,
            itemScript = isWeapon ? null : itemDrop,
            rarity = rarity,
            isWeapon = isWeapon
        };
        
        if (isWeapon)
        {
            if (WeaponManager.Instance.isThereSpaceInLoadouts())
            {
                WeaponManager.Instance.AddItemToLoadout(payload);
            }
            else if (InventoryManager.Instance.isThereSpaceInInventory())
            {
                InventoryManager.Instance.AddItemToInventory(payload);
            }
            WeaponDatabase.Instance.UnregisterActiveWeapon(name);
        }
        else
        {
            if (InventoryManager.Instance.isThereSpaceInInventory())
            {
                InventoryManager.Instance.AddItemToInventory(payload);
            }
            else if (WeaponManager.Instance.isThereSpaceInLoadouts())
            {
                WeaponManager.Instance.AddItemToLoadout(payload);
            }
            ItemDatabase.Instance.UnregisterActiveItem(name);
        }
        
        if (isWeapon && TutorialManager.Instance.IsTutorialActive() && !TutorialManager.Instance.hasPickedUpTutorialWeapon)
        {
            TutorialManager.Instance.TutorialWeaponAcquired();
        }
        
        if (isShopItem)
        {
            ResourceManager.Instance.SpendCurrency(value);
            AudioManager.Instance.PlayUISound("Shop_Purchase");
        }
        AudioManager.Instance.PlayUISound("Item_PickUp");
        ItemCollected();
    }
    
    private void ShowPriceTag()
    {
        priceTag.SetActive(true);
        StartCoroutine(FadeAndSlide(priceText, true));
    }

    private void HidePriceTag()
    {
        StartCoroutine(FadeAndSlide(priceText, false));
        priceTag.SetActive(false);
    }

    private void ItemCollected()
    {
        PlayerItemDetector.Instance.RemoveInteractableItem(this);
        isAcquired = true;
        StartCoroutine(BounceAndDisappearCoroutine());
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
        Debug.Log($"Item '{name}' collected and destroying");
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
        Destroy(gameObject);
    }
}*/