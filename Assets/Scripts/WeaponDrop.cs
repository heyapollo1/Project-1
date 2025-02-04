using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponDrop : MonoBehaviour
{
    [Header("Weapon")]
    public WeaponData weaponData;

    [Header("UI Elements")]
    public GameObject weaponPriceTag;
    public ParticleSystem particleEffect;
    public SpriteRenderer weaponSprite;
    public TextMeshPro priceText;

    private bool isPlayerNearby = false;
    private bool isInitialized = false;
    private bool isScaledUp = false;
    private bool isAcquired = false;
    private bool isHoveringPaused = false;
    //[HideInInspector] public bool isTutorialWeapon = false;

    /*public void Start()
    {
        if (isInitialized) return;
        Debug.LogWarning($"Initializing Weapon Drop");
        WeaponData newWeapon = WeaponDatabase.Instance.GetRandomWeapon();
        if (newWeapon != null)
        {
            weaponData = newWeapon;
            //weaponData.weaponIcon = Resources.Load<Sprite>("Weapons/TheGunIcon");
            weaponSprite.sprite = weaponData.weaponIcon;
            priceText.text = $"${weaponData.price}";
            StartCoroutine(HoverCoroutine());
        }
        else
        {
            Debug.LogError($"weapon null");
        }

        weaponPriceTag.SetActive(false);

        isInitialized = true;
    }*/

    public void Initialize(WeaponData newWeapon)
    {
        if (isInitialized) return;
        Debug.Log($"Weapon initialized: {newWeapon}");
        weaponData = newWeapon;
        weaponSprite.sprite = weaponData.weaponIcon;
        priceText.text = $"${weaponData.price}";
        StartCoroutine(HoverCoroutine());

        weaponPriceTag.SetActive(false);

        isInitialized = true;
    }

    public void SpawnWeaponReward(WeaponData weapon)
    {
        StartCoroutine(LaunchOutOfChest(weapon));
    }

    private IEnumerator LaunchOutOfChest(WeaponData newWeapon)
    {
        //weaponSprite.transform.localScale = Vector3.one * 0.5f;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -2f, 0); // Landing position in front of chest
        float peakHeight = 1.5f;
        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            currentPosition.y += Mathf.Sin(t * Mathf.PI) * peakHeight; // parabolic motion
            transform.position = currentPosition;

            // scale the object for dynamic effect
            //weaponSprite.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, t);

            yield return null;
        }

        if (particleEffect != null)
        {
            particleEffect.Play();
        }
        Debug.Log($"Launch complete, initializing..{newWeapon}");
        Initialize(newWeapon);
        //AudioManager.Instance.PlayUISound("Item_Transform");
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.C) && isInitialized)
        {
            if (IsInShop())
            {
                TryBuyWeapon();
            }
            else
            {
                TryPickUpWeapon();
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

            Tooltip.Instance.ShowTooltip(weaponData.weaponTitle, weaponData.weaponDescription);
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

    public bool TryPickUpWeapon()
    {
        if (isAcquired) return false;

        if (!InventoryManager.Instance.DoesInventoryHaveSpace())
        {

            Debug.Log("Not enough inventory space.");
            return false;
        }

        isAcquired = true;
        AudioManager.Instance.PlayUISound("Item_PickUp");
        Debug.Log($"Pickeds up {weaponData.weaponTitle}!");
        WeaponManager.Instance.ApplyWeapon(weaponData);

        if (TutorialManager.Instance.IsTutorialActive() && !TutorialManager.Instance.hasPickedUpTutorialWeapon)
        {
            Debug.Log($"tutorial weapon picked up");
            TutorialManager.Instance.TutorialWeaponAcquired();
            //CutsceneManager.Instance.StartCutscene(CutsceneDatabase.Instance.weaponPickupCutscene);
        }

        StartCoroutine(BounceAndDisappearCoroutine());
        return true;
    }

    public bool TryBuyWeapon()
    {
        if (isAcquired) return false;

        if (!CurrencyManager.Instance.HasEnoughFunds(weaponData.price))
        {
            Debug.Log("Not enough funds to buy item.");
            return false;
        }

        if (!InventoryManager.Instance.DoesInventoryHaveSpace())
        {
            Debug.Log("Not enough inventory space.");
            return false;
        }

        Debug.Log($"Item '{weaponData.weaponTitle}' successfully bought.");
        isAcquired = true;

        AudioManager.Instance.PlayUISound("Shop_Purchase");
        CurrencyManager.Instance.SpendCurrency(weaponData.price);
        WeaponManager.Instance.ApplyWeapon(weaponData);

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
        weaponPriceTag.SetActive(true);
        StartCoroutine(FadeAndSlide(priceText, true));
    }

    private void HidePriceTag()
    {
        StartCoroutine(FadeAndSlide(priceText, false));
        weaponPriceTag.SetActive(false);
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
        isHoveringPaused = true;

        Vector3 originalScale = weaponSprite.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        //Vector3 originalPosition = transform.position;
        //Vector3 targetPosition = originalPosition + new Vector3(0, 2f, 0);

        float duration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            weaponSprite.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            //transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        //transform.position = targetPosition;
        isHoveringPaused = false;
    }

    private IEnumerator ShrinkCoroutine()
    {
        if (!isScaledUp) yield break;

        isScaledUp = false;
        isHoveringPaused = true;

        Vector3 originalScale = weaponSprite.transform.localScale;
        Vector3 targetScale = originalScale / 1.1f;
        //Vector3 originalPosition = transform.position;
        //Vector3 targetPosition = originalPosition - new Vector3(0, 0.2f, 0); // Move downward slightly

        float duration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            weaponSprite.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            //transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponSprite.transform.localScale = targetScale;
        //transform.position = targetPosition;
        isHoveringPaused = false;
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
