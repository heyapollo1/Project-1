/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ActiveWeaponUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IInteractable
{
    [Header("Equipped Loadout References")]
    public WeaponInstance assignedWeapon;
    public BaseItem assignedItem;
    public Image icon;
    public Slider cooldownSlider;
    public GameObject highlightFX;
    public UIRarityVisual uiRarityVisual;
    
    [HideInInspector]public string weaponKey;
    [HideInInspector] public Transform originalParent;
    
    private int loadoutIndex;
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private WeaponSlotUI weaponSlot;
    public static ActiveWeaponUI draggedWeapon;
    private IInteractable hoveredItemTooltip = null;

    public bool isWeapon;
    private bool isLeftHand;
    public static bool successfulSwap = false;
    private bool tooltipActive = false;
    private bool isInitialized = false;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void InitializeWeaponUI(WeaponInstance weapon, int loadoutIndex, bool isLeftHand, WeaponSlotUI weaponSlot)
    {
        uiRarityVisual.ApplyUIRarityVisual(weapon.rarity);
        weaponKey = $"Loadout{loadoutIndex}_{(isLeftHand ? "L" : "R")}_{weapon.weaponTitle}";
        this.weaponSlot = weaponSlot;
        this.loadoutIndex = loadoutIndex;
        this.isLeftHand = isLeftHand;
        originalParent = weaponSlot.transform.parent;
        originalPosition = transform.localPosition;

        weaponSlot.AssignWeaponUI(this, true);
        weapon.weaponBase.weaponUI = this;
        assignedWeapon = weapon;
        isWeapon = true;
        isInitialized = true;
        icon.gameObject.SetActive(true);
        if (weapon != null)
        {
            icon.sprite = weapon.weaponIcon;
            icon.enabled = true;
            //Debug.Log($"Weapon placed: {assignedWeapon.weaponTitle}");
        }
    }
    
    public void InitializeItemUI(BaseItem item, int loadoutIndex, bool isLeftHand, WeaponSlotUI weaponSlot)
    {
        uiRarityVisual.ApplyUIRarityVisual(item.currentRarity);
        weaponKey = $"Loadout{loadoutIndex}_{(isLeftHand ? "L" : "R")}_{item.itemName}";
        this.weaponSlot = weaponSlot;
        this.loadoutIndex = loadoutIndex;
        this.isLeftHand = isLeftHand;
        weaponSlot.AssignWeaponUI(this, false);
        originalParent = weaponSlot.transform.parent;
        originalPosition = transform.localPosition;
        
        item.loadoutUI = this;
        assignedItem = item;
        isWeapon = false;
        icon.gameObject.SetActive(true);
        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
           // Debug.Log($"Item placed: {assignedWeapon.weaponTitle}");
        }
    }
    
    public void UpgradeWeaponInSlot()
    {
        if (assignedWeapon != null)
        {
            Debug.Log("Upgraded Weapon In Slot");
            assignedWeapon.UpgradeWeaponInstance();
            uiRarityVisual.ApplyUIRarityVisual(assignedWeapon.rarity);
        }
    }

    public void UpgradeItemInSlot()
    {
        if (assignedItem != null)
        {
            Debug.Log("Upgraded Item In Slot");
            assignedItem.Upgrade();
            uiRarityVisual.ApplyUIRarityVisual(assignedItem.currentRarity);
        }
    }

    public void StartCooldownUI(float cooldownDuration)
    {
        cooldownSlider.value = 1;
        OnCooldownUpdated(weaponKey, cooldownDuration, cooldownDuration);
    }
    
    private void OnCooldownUpdated(string updatedWeaponKey, float currentCooldown, float totalCooldown)
    {
        if (updatedWeaponKey == weaponKey)
        {
            //Debug.Log($"Stored Weapon Cooldown Updated: {currentCooldown} / {totalCooldown}");
            UpdateActiveCooldownUI(currentCooldown, totalCooldown);
        }
    }
    
    public void UpdateActiveCooldownUI(float currentCooldown, float totalCooldown)
    {
        if (currentCooldown > 0)
        {
            cooldownSlider.value = currentCooldown / totalCooldown;
        }
        else
        {
            cooldownSlider.value = 0;
        }
    }
    
    public void HighlightSlot(bool highlight)
    {
        if (highlight)
        {
            highlightFX.SetActive(true);
        }
        else
        {
            highlightFX.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        WeaponCooldownManager.Instance.OnCooldownUpdated += OnCooldownUpdated;
    }

    private void OnDisable()
    {
        WeaponCooldownManager.Instance.OnCooldownUpdated -= OnCooldownUpdated;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isWeapon)
        {
            if (assignedWeapon.weaponBase.isWeaponDisabled() || assignedWeapon.weaponBase.isWeaponOnCooldown()) return;
        }

        successfulSwap = false;
        draggedWeapon = this;
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        transform.SetParent(transform.root); // Move to root so it's above everything
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isWeapon)
        {
            if (assignedWeapon.weaponBase.isWeaponDisabled() || assignedWeapon.weaponBase.isWeaponOnCooldown()) return;
        }
        
        transform.position = eventData.position; // Follow cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (successfulSwap) 
        {
            if (isWeapon)
            {
                var weapon = WeaponManager.Instance.GetWeaponByTitle(assignedWeapon.weaponTitle);
                WeaponManager.Instance.ShuffleActiveLoadout(!isLeftHand, weapon);
            }
            else
            {
                var item = WeaponManager.Instance.GetItemByName(assignedItem.itemName);
                WeaponManager.Instance.ShuffleActiveLoadout(!isLeftHand, null, item);
            }
            Debug.Log($"Successfully swapped Weapon {assignedWeapon}");
            draggedWeapon = null;
            canvasGroup.blocksRaycasts = true;
            isInitialized = false;
            successfulSwap = false;
            return;
        }
        StartCoroutine(DelayedEndDrag());
    }
    
    private IEnumerator DelayedEndDrag()
    {
        yield return null; // wait one frame to let OnDrop() run
        Debug.Log($"End drag, is successful Swap: {successfulSwap}");

        if (EventSystem.current.IsPointerOverGameObject())
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
            successfulSwap = false;
            Debug.Log($"Returning {assignedWeapon?.weaponTitle} to original slot (Invalid Drop).");
        }
        else
        {
            weaponSlot.ClearWeaponUI();
            if (isWeapon)
            {
                Debug.Log($"Dropped {assignedWeapon?.weaponTitle} into the world");
                WeaponManager.Instance.DropHandObject(assignedWeapon);
            }
            else
            {
                Debug.Log($"Dropped {assignedItem?.itemName} into the world");
                WeaponManager.Instance.DropHandObject(null, assignedItem);
            }
            Destroy(gameObject);
        }

        Debug.Log($"Returning {assignedWeapon?.weaponTitle} to original slot (Failed Drop).");
        canvasGroup.blocksRaycasts = true;
        draggedWeapon = null;
        successfulSwap = false;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipActive) return;
        tooltipActive = true;
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!tooltipActive) return;
        tooltipActive = false;
        HideTooltip();
    }
    
    public void ShowTooltip()
    {
        TooltipManager.Instance.SetHoverTooltip(this, "Loadout");
    }

    public void HideTooltip()
    {
        TooltipManager.Instance.ClearHoverTooltip();
    }

    public void Interact()
    {
        if (TooltipManager.Instance.GetActiveTooltip() != this) return;
    }
}*/