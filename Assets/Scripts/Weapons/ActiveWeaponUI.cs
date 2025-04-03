using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ActiveWeaponUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IInteractable
{
    [Header("Equipped Weapon References")]
    public WeaponInstance assignedWeapon;
    public Image weaponIcon;
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
    
    private bool isLeftHand;
    public static bool successfulSwap = false;
    private bool tooltipActive = false;
    
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
        weaponSlot.AssignWeaponUI(this);
        originalParent = weaponSlot.transform.parent;
        originalPosition = transform.localPosition;
        
        weapon.weaponBase.weaponUI = this;
        assignedWeapon = weapon;
        weaponIcon.gameObject.SetActive(true);

        if (weapon != null)
        {
            weaponIcon.sprite = weapon.weaponIcon;
            weaponIcon.enabled = true;
            Debug.Log($"Item placed: {assignedWeapon.weaponTitle}");
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
    
    public void ClearWeapon()
    {
        assignedWeapon = null;
        weaponIcon.sprite = null;
        weaponIcon.enabled = false;
        Debug.Log($"Weapon slot {(isLeftHand ? "Left" : "Right")} is now empty.");
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
        if (successfulSwap || assignedWeapon.weaponBase.isWeaponDisabled() || assignedWeapon.weaponBase.isWeaponOnCooldown()) return;
        draggedWeapon = this;
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        Debug.Log("Start Drag");
        transform.SetParent(transform.root); // Move to root so it's above everything
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (successfulSwap || assignedWeapon.weaponBase.isWeaponDisabled() || assignedWeapon.weaponBase.isWeaponOnCooldown()) return;
        Debug.Log("Dragging");
        transform.position = eventData.position; // Follow cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (successfulSwap) 
        {
            Debug.Log($"Successfully swapped Weapon {assignedWeapon}");
            draggedWeapon = null;
            canvasGroup.blocksRaycasts = true;
            successfulSwap = false;
            return;
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Still over UI but not a valid drop
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
            Debug.Log($"Returning {assignedWeapon?.weaponTitle} to original slot (Invalid Drop).");
        }
        else
        {
            // Dropped into game world or nothing
            Debug.Log($"Dropped {assignedWeapon?.weaponTitle} into the world");
            weaponSlot.ClearWeaponUI();
            WeaponManager.Instance.DiscardWeapon(assignedWeapon);
            Destroy(gameObject);
        }
        
        Debug.Log($"Returning {assignedWeapon?.weaponTitle} to original slot (Failed Drop).");
        canvasGroup.blocksRaycasts = true;
        draggedWeapon = null;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipActive) return;
        tooltipActive = true;
        //Debug.Log("Enter pointer");
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!tooltipActive) return;
        tooltipActive = false;
        //Debug.Log("Exit pointer");
        HideTooltip();
    }
    
    public void ShowTooltip()
    {
        //Debug.Log("Show tooltip");
        TooltipManager.Instance.SetHoverTooltip(this, "WeaponSlot"); 
        //Debug.Log("Show tooltip");
    }

    public void HideTooltip()
    {
        TooltipManager.Instance.ClearHoverTooltip();
    }

    public void Interact()
    {
        if (TooltipManager.Instance.GetActiveTooltip() != this) return;
        //Debug.Log($"Inspecting inventory item: {assignedWeapon.weaponTitle}");
    }
}