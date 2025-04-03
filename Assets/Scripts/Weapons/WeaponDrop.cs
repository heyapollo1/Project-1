using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class WeaponDrop : MonoBehaviour, IInteractable
{
    [Header("Weapon")]
    public WeaponInstance weapon;
    public RarityVisual weaponRarityVisual;

    [Header("UI Elements")]
    public GameObject weaponPriceTag;
    public ParticleSystem upgradeableFX;
    public ParticleSystem landingFX;
    public SpriteRenderer weaponSprite;
    public TextMeshPro priceText;
    public float detectionRadius = 1f; 
    
    private Pedestal parentPedestal;
    private bool isInitialized = false;
    private bool isAcquired = false;
    private bool tooltipActive = false;
    private bool isShopItem = false;
    public bool eligibleForUpgrade = false;

    public LayerMask playerLayer;

    public void Initialize(WeaponInstance newWeapon, bool isInShop = false)
    {
        if (isInitialized) return;
        Debug.Log($"Weapon initialized: {newWeapon.weaponTitle}");
        playerLayer = LayerMask.GetMask("Player");
        weapon = newWeapon;
        weaponSprite.sprite = newWeapon.weaponIcon;
        priceText.text = $"${newWeapon.value}";
        weaponPriceTag.SetActive(false);
        isInitialized = true;
        tooltipActive = false;
        isAcquired = false;
        isShopItem = isInShop;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
        weaponRarityVisual.ApplyRarityMaterial(newWeapon.rarity);
        EventManager.Instance.StartListening("WeaponInteraction", CheckForUpgrade);

        CheckForUpgrade(weapon.weaponTitle);
       // StartCoroutine(DelayedCheck());
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("WeaponInteraction", CheckForUpgrade);
    }

    private IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(0.05f);
        if (IsPlayerNearby())
        {
            if (!tooltipActive)
            {
                if (isShopItem)
                {
                    ShowPriceTag();
                }
                if (eligibleForUpgrade)
                {
                    Debug.Log("Turning ON upgrade.");
                    LoadoutUIManager.Instance.HighlightMatchingWeapon(weapon, true);
                }
                tooltipActive = true;
                PlayerItemDetector.Instance.AddInteractableItem(this);
            }
        }
        else
        {
            Debug.Log($"[Delayed] Player is NOT nearby: {IsPlayerNearby()}");
        }
    }
    
    private bool IsPlayerNearby()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (player != null)
        {
            //Debug.Log($"Detected Player: {player.gameObject.name}");
            return true;
        }
        //Debug.Log("No player detected nearby.");
        return false;
    }
    
    public void CheckForUpgrade(string weaponName)
    {
        if (isAcquired) return;
        if (weaponName == weapon.weaponTitle)
        {
            if (WeaponManager.Instance.HasWeapon(weaponName))
            {
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
    
    private void Update()
    {
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
                    LoadoutUIManager.Instance.HighlightMatchingWeapon(weapon, false);
                }
                if (PlayerItemDetector.Instance.isInteractable(this))
                {
                    PlayerItemDetector.Instance.RemoveInteractableItem(this);
                    tooltipActive = false;
                }
            }
        }
    }
    
    public void SetPedestal(Pedestal pedestal)
    {
        parentPedestal = pedestal;
    }
    
    public void DropWeaponReward(WeaponInstance weapon, Transform spawnPoint)
    {
        StartCoroutine(HandleWeaponDrop(weapon, spawnPoint));
    }

    private IEnumerator HandleWeaponDrop(WeaponInstance newWeapon, Transform spawnPoint)
    {
        Vector3 startPosition = spawnPoint.position;
        Vector3 endPosition = startPosition + new Vector3(0, -2f, 0); // Landing position in front of chest
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
        Debug.Log($"Launch complete, initializing..{newWeapon}");
        Initialize(newWeapon);
    }
    
    public void Interact()
    {
        Debug.Log($"Interacting...{tooltipActive}, {isInitialized}");
        if (!isAcquired && isInitialized)
        {
            Debug.Log("Interacting...");
            if (isShopItem)
            {
                TryBuyWeapon();
            }
            else
            {
                PickUpWeapon();
            }
        }
    }
    
    public void ShowTooltip()
    {
        if (!isInitialized) return;
        Debug.Log("Entering space...");
        TooltipManager.Instance.SetWorldTooltip(this, "Weapon", eligibleForUpgrade);
    }
    
    public void HideTooltip()
    {
        LoadoutUIManager.Instance.HighlightMatchingWeapon(weapon, false);
        TooltipManager.Instance.ClearWorldTooltip();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!tooltipActive && collision.CompareTag("Player"))
        {
            Debug.Log("Entering space...");
            if (isShopItem)
            {
                ShowPriceTag();
            }
            if (eligibleForUpgrade)
            {
                //Debug.Log("Turning ON upgrade.");
                LoadoutUIManager.Instance.HighlightMatchingWeapon(weapon, true);
            }
            tooltipActive = true;
            Debug.Log("Entering space...");
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
                LoadoutUIManager.Instance.HighlightMatchingWeapon(weapon, false);
            }
            PlayerItemDetector.Instance.RemoveInteractableItem(this);
        }
    }
    
    public void PickUpWeapon()
    {
        if (isAcquired) return;
        if (!WeaponManager.Instance.IsThereSpaceForWeapons())
        {
            Debug.Log("Not enough weapon space.");
            return;
        }
        
        //Debug.Log($"Picked up {weapon.weaponTitle}!");
        AudioManager.Instance.PlayUISound("Item_PickUp");
        WeaponManager.Instance.AcquireWeapon(weapon, eligibleForUpgrade);
        WeaponDatabase.Instance.UnregisterActiveWeapon(weapon.weaponTitle);
        LoadoutUIManager.Instance.HighlightMatchingWeapon(weapon, false);
        PlayerItemDetector.Instance.RemoveInteractableItem(this);
        //TooltipManager.Instance.HideTooltip();
        isAcquired = true;
        Debug.Log($"Picked up {weapon.weaponTitle}!");
        if (TutorialManager.Instance.IsTutorialActive() && !TutorialManager.Instance.hasPickedUpTutorialWeapon)
        {
            TutorialManager.Instance.TutorialWeaponAcquired();
        }
        StartCoroutine(BounceAndDisappearCoroutine());
    }

    public bool TryBuyWeapon()
    {
        if (isAcquired) return false;
        isAcquired = true;
        if (!ResourceManager.Instance.HasEnoughFunds(weapon.value))
        {
            Debug.Log("Not enough funds to buy item.");
            return false;
        }

        if (!InventoryManager.Instance.DoesInventoryHaveSpace())
        {
            Debug.Log("Not enough inventory space.");
            return false;
        }

        Debug.Log($"Item '{weapon.weaponTitle}' successfully bought.");
        AudioManager.Instance.PlayUISound("Shop_Purchase");
        TooltipManager.Instance.HideTooltip();
        ResourceManager.Instance.SpendCurrency(weapon.value);
        WeaponManager.Instance.AcquireWeapon(weapon, eligibleForUpgrade);
        WeaponDatabase.Instance.UnregisterActiveWeapon(weapon.weaponTitle);
        LoadoutUIManager.Instance.HighlightMatchingWeapon(weapon, false);
        PlayerItemDetector.Instance.RemoveInteractableItem(this);

        StartCoroutine(BounceAndDisappearCoroutine());
        return true;
    }

    private void ShowPriceTag()
    {
        weaponPriceTag.SetActive(true);
        StartCoroutine(FadeAndSlide(priceText, true));
    }

    private void HidePriceTag()
    {
        StartCoroutine(FadeAndSlide(priceText, false));
        weaponPriceTag.SetActive(false);
    }

    private IEnumerator BounceAndDisappearCoroutine()
    {
        Debug.Log("Disappear");
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
