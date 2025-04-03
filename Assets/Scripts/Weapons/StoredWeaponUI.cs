using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoredWeaponUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider cooldownSlider;  // stored item state
    public Image sliderScale;  // Changes color/opacity when a weapon is stored
    public Image cooldownSliderBG;
    public UIRarityVisual uiRarityVisual;
    public Material defaultMaterial;
    public Material upgradeMaterial;
    
    private RectTransform uiTransform;
    private string weaponKey;
    [HideInInspector] public WeaponInstance assignedWeapon;
    
    private float emptySize = 1f; 
    private float normalSize = 2f; 

    public void Initialize(WeaponInstance weaponInstance, int loadoutID, bool isLeftHand)
    {
        GetWeaponCooldownEvent();
        weaponKey = $"Loadout{loadoutID}_{(isLeftHand ? "L" : "R")}_{weaponInstance.weaponTitle}";
        assignedWeapon = weaponInstance;
        
        if (uiTransform == null)
        {
            uiTransform = sliderScale.GetComponent<RectTransform>();
        }
        
        float currentCooldown = WeaponCooldownManager.Instance.GetCooldownTime(weaponKey);
        float totalCooldown = WeaponCooldownManager.Instance.GetTotalCooldownTime(weaponKey);
        Debug.Log("Initialized Stored Weapon UI");
        uiRarityVisual.ApplyUIRarityVisual(assignedWeapon.rarity);
        //ChangeSize(normalSize);
        UpdateStoredCooldownUI(currentCooldown, totalCooldown);
    }
    
    public void SetEmpty()
    {
        Debug.Log("SetEmpty");
        assignedWeapon = null;
        //uiRarityVisual.ApplyDefaultVisual();
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
            //Debug.Log($"cooldown Value: {cooldownSlider.value}");
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

    /*private IEnumerator AnimateSize(float targetSize)
    {
        float duration = 0.2f;
        float startSize = uiTransform.sizeDelta.y;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newY = Mathf.Lerp(startSize, targetSize, time / duration);
            uiTransform.sizeDelta = new Vector2(uiTransform.sizeDelta.x, newY);
            yield return null;
        }

        uiTransform.sizeDelta = new Vector2(uiTransform.sizeDelta.x, targetSize);
    }*/