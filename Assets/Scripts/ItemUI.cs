using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IInteractable
{
    [Header("UI References")]
    public WeaponInstance assignedWeapon;
    public BaseItem assignedItem;
    public GameObject highlightFX;
    public Slider cooldownSlider;
    public Image icon;
    
    private int loadoutIndex;
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private WeaponSlotUI handSlotUI;
    private InventoryUISlot inventorySlotUI;
    private IInteractable hoveredItemTooltip = null;
    private PlayerController player;
    
    public UIRarityVisual itemRarityVisual;
    public Rarity rarity;
    public string name;
    public string uniqueID;
    public int value;
    
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public InventoryUISlot originalInventorySlot;
    [HideInInspector] public WeaponSlotUI originalHandSlot;
    [HideInInspector]public string weaponKey;
    [HideInInspector] public bool isWeapon;
    [HideInInspector] public bool isInHand;
    [HideInInspector] public bool isLeftHand = false;
    
    public static ItemUI draggedItem;
    public static bool successfulSwap = false;
    private bool tooltipActive = false;
    private bool isDragging = false;
    
    private void Awake()
    {
        player = PlayerController.Instance;
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void InitializeItemUI(BaseItem item = null, WeaponInstance weapon = null, bool inHand = false, bool isDropped = false)
    {
        isWeapon = item == null;
        name = isWeapon ? weapon.weaponTitle : item.itemName;
        Debug.Log($"ItemUI Initialized: {name}");
        rarity = isWeapon ? weapon.rarity : item.rarity;
        uniqueID = isWeapon ? weapon.uniqueID : item.uniqueID;
        icon.sprite = isWeapon ? weapon.weaponIcon : item.icon;
        assignedItem = isWeapon ? null : item;
        assignedWeapon = isWeapon ? weapon : null;
        isInHand = inHand;
        
        if (isWeapon) weapon.AssignUI(this);
        else item.AssignUI(this);
        
        weaponKey = $"Loadout{loadoutIndex}_{(isLeftHand ? "L" : "R")}_{name}";
        itemRarityVisual.ApplyUIRarityVisual(rarity);
        
        icon.gameObject.SetActive(true);
        icon.enabled = true;
        tooltipActive = false;

        if (isDropped)
        {
            transform.SetParent(null);
        }
    }

    public void StartCooldownUI(float cooldownDuration)
    {
        cooldownSlider.value = 1;
        OnCooldownUpdated(weaponKey, cooldownDuration, cooldownDuration);
    }
    
    private void OnCooldownUpdated(string id, float currentCooldown, float totalCooldown)
    {
        if (id == uniqueID)
        {
            //Debug.Log($"Weapon Cooldown Updated: {currentCooldown} / {totalCooldown}");
            UpdateActiveCooldownUI(currentCooldown, totalCooldown);
        }
    }
    
    public void UpdateActiveCooldownUI(float currentCooldown, float totalCooldown)
    {
        if (currentCooldown > 0)
        {
            cooldownSlider.value = currentCooldown / totalCooldown;
            //Debug.Log($"Updating Cooldown UI{ currentCooldown / totalCooldown }");
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

    private bool CanDrag()
    {
        if (player.IsPlayerDisabled() || player.IsPlayerDead()) return false;
        if (isWeapon)
        {
            return !assignedWeapon.weaponBase.IsWeaponDisabled() && !assignedWeapon.weaponBase.isWeaponOnCooldown();
        }
        return true;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag()) return;
        if (isDragging) return;
        isDragging = true;
        successfulSwap = false; 
        draggedItem = this;
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        UIManager.Instance.SetDragging(true);
        if (!isInHand) originalInventorySlot = GetComponentInParent<InventoryUISlot>();
        else originalHandSlot = GetComponentInParent<WeaponSlotUI>();
        transform.SetParent(transform.root); 
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag()) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        if (successfulSwap) 
        {
            Debug.Log($"Successfully swapped Item {assignedItem}");
            draggedItem = null;
            originalParent = null;
            canvasGroup.blocksRaycasts = true;
            successfulSwap = false;
            isDragging = false;
            return;
        }
        StartCoroutine(FailedSwap()); //if swap fails, run fail logic
    }
    
    private IEnumerator FailedSwap()
    {
        yield return null;
        if (EventSystem.current.IsPointerOverGameObject()) //on other UI, negate
        {
            UIManager.Instance.SetDragging(false);
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
            canvasGroup.blocksRaycasts = true;
            draggedItem = null;
            successfulSwap = false;
            isDragging = false;
            
            Debug.Log($"Returning {assignedWeapon?.weaponTitle} to original slot (Invalid Drop).");
        }
        else // Drop logic
        {
            UIManager.Instance.SetDragging(false);
            if (isInHand) // if in hand, clear from weapon manager.
            {
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
            }
            else
            {
                if (isWeapon)
                {
                    Debug.Log($"Dropped {assignedWeapon?.weaponTitle} into the world");
                    InventoryManager.Instance.DropInventoryObject(assignedWeapon);
                }
                else
                {
                    Debug.Log($"Dropped {assignedItem?.itemName} into the world");
                    InventoryManager.Instance.DropInventoryObject(null, assignedItem);
                }
            }
            Debug.Log($"Returning {assignedWeapon?.weaponTitle} to original slot (Failed Drop).");
            isDragging = false;
            Destroy(gameObject);
        }
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
        TooltipManager.Instance.SetHoverTooltip(this, "ItemUI");
    }

    public void HideTooltip()
    {
        TooltipManager.Instance.ClearHoverTooltip();
    }

    public void Interact()
    {
        if (TooltipManager.Instance.GetActiveTooltip() != this) return;
    }
}