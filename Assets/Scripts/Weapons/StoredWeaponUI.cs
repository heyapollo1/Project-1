using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoredSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider cooldownSlider;  // stored item state
    public Image sliderScale;  // Changes color/opacity when a weapon is stored
    public Image cooldownSliderBG;
    public UIRarityVisual uiRarityVisual;
    public Material defaultMaterial;
    public Material upgradeMaterial;
    public bool isWeapon;
    
    private RectTransform uiTransform;
    private string weaponKey;
    [HideInInspector] public WeaponInstance assignedWeapon;
    [HideInInspector] public BaseItem assignedItem;
    
    private float emptySize = 1f; 
    private float normalSize = 2f; 
    
    public void InitializeWeaponUI(WeaponInstance weapon, int loadoutIndex, bool isLeftHand)
    {
        GetWeaponCooldownEvent();
        weaponKey = $"Loadout{loadoutIndex}_{(isLeftHand ? "L" : "R")}_{weapon.weaponTitle}";
        assignedWeapon = weapon;
        
        if (uiTransform == null)
        {
            uiTransform = sliderScale.GetComponent<RectTransform>();
        }
        
        float currentCooldown = WeaponCooldownManager.Instance.GetCooldownTime(weaponKey);
        float totalCooldown = WeaponCooldownManager.Instance.GetTotalCooldownTime(weaponKey);
        //Debug.Log("Initialized Stored Weapon UI");
        uiRarityVisual.ApplyUIRarityVisual(assignedWeapon.rarity);
        UpdateStoredCooldownUI(currentCooldown, totalCooldown);

        isWeapon = true;
    }
    
    public void InitializeItemUI(BaseItem item, int loadoutIndex, bool isLeftHand)
    {
        uiRarityVisual.ApplyUIRarityVisual(item.rarity);
        weaponKey = $"Loadout{loadoutIndex}_{(isLeftHand ? "L" : "R")}_{item.itemName}";
        assignedItem = item;
        
        if (uiTransform == null)
        {
            uiTransform = sliderScale.GetComponent<RectTransform>();
        }
        
        float currentCooldown = WeaponCooldownManager.Instance.GetCooldownTime(weaponKey);
        float totalCooldown = WeaponCooldownManager.Instance.GetTotalCooldownTime(weaponKey);
        //Debug.Log("Initialized Stored Item UI");
        uiRarityVisual.ApplyUIRarityVisual(assignedItem.rarity);
        UpdateStoredCooldownUI(currentCooldown, totalCooldown);

        isWeapon = false;
    }
    
    public void SetEmpty()
    {
        assignedWeapon = null;
        cooldownSlider.value = 0;
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
    
    public void UpdateStoredCooldownUI(float currentCooldown, float totalCooldown)
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

    public void GetWeaponCooldownEvent()
    {
        if (WeaponCooldownManager.Instance == null)
        {
            Debug.LogError("Missing cooldown manager");
        }
        WeaponCooldownManager.Instance.OnCooldownUpdated += OnCooldownUpdated;
    }

    private void OnDisable()
    {
        WeaponCooldownManager.Instance.OnCooldownUpdated -= OnCooldownUpdated;
    }

    private void OnCooldownUpdated(string updatedWeaponKey, float currentCooldown, float totalCooldown)
    {
        if (updatedWeaponKey == weaponKey)
        {
            //Debug.Log($"Stored Weapon Cooldown Updated: {currentCooldown} / {totalCooldown}");
            UpdateStoredCooldownUI(currentCooldown, totalCooldown);
        }
    }
    
    public void HighlightSlot(bool highlight)
    {
        cooldownSliderBG.material = highlight ? upgradeMaterial : defaultMaterial;
    }
    
    private void ChangeSize(float targetSize)
    {
        uiTransform.sizeDelta = new Vector2(uiTransform.sizeDelta.x, targetSize);
    }
}
