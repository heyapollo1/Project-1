using System.Collections;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Details")]
    public ItemData itemData;

    [Header("UI Elements")]
    public GameObject itemPriceTag;
    public ParticleSystem particleEffect;
    public SpriteRenderer itemSprite;
    public TextMeshPro priceText;

    private bool isPlayerNearby = false;
    private bool isInitialized = false;
    private bool isScaledUp = false;
    private bool isAcquired = false;
    private bool isHoveringPaused = false;
    //public bool isShopItem { get; private set; }

    /*public void Start()
    {
        if (isInitialized) return;

        ItemData newItem = ItemDatabase.Instance.GetRandomItem();
        itemData = newItem;
        itemSprite.sprite = itemData.icon;
        priceText.text = $"${itemData.price}";
        StartCoroutine(HoverCoroutine());

        itemPriceTag.SetActive(false);

        isInitialized = true;
    }*/

    public void Initialize(ItemData item)
    {
        if (isInitialized) return;

        itemData = item;
        itemSprite.sprite = itemData.icon;
        priceText.text = $"${itemData.price}";
        StartCoroutine(HoverCoroutine());

        itemPriceTag.SetActive(false);

        isInitialized = true;
    }

    public void SpawnItemReward(ItemData item)
    {
        StartCoroutine(LaunchOutOfChest(item));
    }

    private IEnumerator LaunchOutOfChest(ItemData newItem)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -2f, 0);
        float peakHeight = 2f;
        float duration = 0.8f;
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

        if (particleEffect != null)
        {
            particleEffect.Play();
        }
        Debug.Log($"Launch complete, initializing..{newItem}");
        Initialize(newItem);
        //AudioManager.Instance.PlayUISound("Item_Transform");
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            if (IsInShop())
            {
                TryBuyItem();
            }
            else
            {
                PickUpItem();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (IsInShop())
            {
                ShowPriceTag();
            }

            Tooltip.Instance.ShowTooltip(itemData.itemName, itemData.description);
            StartCoroutine(EnlargeCoroutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (IsInShop())
            {
                HidePriceTag();
            }

            Tooltip.Instance.HideTooltip();
            StartCoroutine(ShrinkCoroutine());
        }
    }

    private void PickUpItem()
    {
        if (isAcquired) return;

        isAcquired = true;
        AudioManager.Instance.PlayUISound("Item_PickUp");
        Debug.Log($"Pickeds up {itemData.itemName}!");
        InventoryManager.Instance.AddItem(itemData);

        StartCoroutine(BounceAndDisappearCoroutine());
    }

    public bool TryBuyItem()
    {
        if (isAcquired) return false;

        if (!CurrencyManager.Instance.HasEnoughFunds(itemData.price))
        {
            Debug.Log("Not enough funds to buy item.");
            return false;
        }

        // Check inventory space
        if (!InventoryManager.Instance.DoesInventoryHaveSpace())
        {
            Debug.Log("Not enough inventory space.");
            return false;
        }

        Debug.Log($"Item '{itemData.itemName}' successfully bought.");
        isAcquired = true;

        // Deduct funds and add item to inventory
        AudioManager.Instance.PlayUISound("Shop_Purchase");
        CurrencyManager.Instance.SpendCurrency(itemData.price);
        InventoryManager.Instance.AddItem(itemData);

        StartCoroutine(BounceAndDisappearCoroutine());
        return true;
    }

    private bool IsInShop()
    {
        while (transform.parent != null)
        {
            if (transform.parent.CompareTag("Pedestal"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
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

    private IEnumerator HoverCoroutine()
    {
        Vector3 startPos = transform.position;
        float hoverSpeed = 2f; // Speed
        float hoverHeight = 0.1f; // Height 

        while (true)
        {
            if (!isHoveringPaused)
            {
                float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
                transform.position = new Vector3(startPos.x, newY, startPos.z);
            }

            yield return null;
        }
    }

    private IEnumerator EnlargeCoroutine()
    {
        if (isScaledUp) yield break;

        isScaledUp = true;
        //isHoveringPaused = true;

        Vector3 originalScale = itemSprite.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        //Vector3 originalPosition = transform.position;
        //Vector3 targetPosition = originalPosition + new Vector3(0, 2f, 0);

        float duration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            itemSprite.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            //transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        //transform.position = targetPosition;
        //isHoveringPaused = false;
    }

    private IEnumerator ShrinkCoroutine()
    {
        if (!isScaledUp) yield break;

        isScaledUp = false;
        //isHoveringPaused = true;

        Vector3 originalScale = itemSprite.transform.localScale;
        Vector3 targetScale = originalScale / 1.1f;
        //Vector3 originalPosition = transform.position;
        //Vector3 targetPosition = originalPosition - new Vector3(0, 0.2f, 0); // Move downward slightly

        float duration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            itemSprite.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            //transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        itemSprite.transform.localScale = targetScale;
        //transform.position = targetPosition;
        //isHoveringPaused = false;
    }

    private IEnumerator BounceAndDisappearCoroutine()
    {
        isHoveringPaused = true;

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

        // Destroy the item
        Destroy(gameObject);
    }

    private IEnumerator FadeAndSlide(TextMeshPro text, bool fadeIn)
    {
        Vector3 originalPosition = text.transform.localPosition;
        Vector3 targetPosition = fadeIn ? originalPosition + new Vector3(0, 0.25f, 0) : originalPosition - new Vector3(0, 0.25f, 0);
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
}