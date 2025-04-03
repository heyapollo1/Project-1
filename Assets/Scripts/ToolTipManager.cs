using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }
    
    [Header("General")]
    public GameObject tooltipPanel;
    public UIRarityVisual uiRarityVisual;
    public Image tooltipBackgroundImage;
    public Material defaultMaterial;
    public Material upgradeMaterial;
    
    [Header("Class Tag Panel")]
    public Transform classTagContainer;
    public GameObject classTagPrefab;
    public GameObject mainTag;
    
    [Header("Main Panel")]
    public GameObject mainPanelIcon;
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    
    [Header("Stat Tag Panel")]
    public Transform statTagContainer;
    public GameObject valueTag;
    public GameObject damageTag;
    public GameObject cooldownTag;
    public GameObject rangeTag;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI damgeText;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI rangeText;
    
    private Coroutine updateCoroutine;
    private IInteractable activeWorldTooltip = null;  // World-based tooltips
    private IInteractable activeHoverTooltip = null; // UI-based tooltips (inventory, etc.)

    
    public void Initialize()
    {
        HideTooltip();
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        HideTooltip();
    }
    
    public void SetWorldTooltip(IInteractable obj, string tooltipType, bool isUpgrade = false)
    {
        if (activeHoverTooltip != null) return;
        //Debug.Log($"Setting Weapon tooltip for {tooltipType}");
        activeWorldTooltip = obj;
        CheckTooltipType(obj, tooltipType, isUpgrade);
    }
    
    public void ClearWorldTooltip()
    {
        //Debug.Log($"Clearing World tooltip");
        activeWorldTooltip = null;
        HideTooltip();
    }
    
    public void SetHoverTooltip(IInteractable obj, string tooltipType, bool isUpgrade = false)
    {
        if (activeWorldTooltip != null)
        {
            //Debug.Log("Hiding, override world tooltip");
            HideTooltip(); // This hides any active world tooltip
        }
        //Debug.Log("Triggering hover tooltip");
        activeHoverTooltip = obj;
        CheckTooltipType(obj, tooltipType, isUpgrade);
    }
    
    public void ClearHoverTooltip()
    {
        if (activeHoverTooltip != null)
        {
            activeHoverTooltip = null;
            HideTooltip();
            PlayerItemDetector.Instance.UpdateTooltip();
        }
    }

    public IInteractable GetActiveTooltip()
    {
        if (activeHoverTooltip != null) return null;
        
        if (activeWorldTooltip is MonoBehaviour worldObj && worldObj.gameObject != null)
        {
            return activeWorldTooltip;
        }
        return null; // No valid tooltip available
    }
    
    public bool isHoverTooltipActive()
    { 
       return activeHoverTooltip != null;
    }
    
    public void CheckTooltipType(IInteractable tooltipObject, string tooltipType, bool isUpgrade)
    {
        if (tooltipObject == null)  return;

        tooltipBackgroundImage.material = isUpgrade ? upgradeMaterial : defaultMaterial;

        switch (tooltipType)
        {
            case "Weapon":
                WeaponDrop weaponDrop = tooltipObject as WeaponDrop;
                if (weaponDrop != null && weaponDrop.weapon != null)
                {
                    //Debug.Log($"Showing Weapon tooltip for {weaponDrop.weapon.weaponTitle}");
                    ShowWeaponTooltip(weaponDrop.weapon, isUpgrade);
                }
                break;
            case "Item":
                ItemDrop itemDrop = tooltipObject as ItemDrop;
                if (itemDrop != null)
                {
                    //Debug.Log($"Showing Item tooltip for {itemDrop.item}");
                    ShowItemTooltip(itemDrop.item, isUpgrade);
                }
                break;
            case "WeaponSlot":
                ActiveWeaponUI weaponUI = tooltipObject as ActiveWeaponUI;
                if (weaponUI != null && weaponUI.assignedWeapon != null)
                {
                    //Debug.Log($"Showing Weapon tooltip for {weaponUI.assignedWeapon.weaponTitle}");
                    ShowWeaponTooltip(weaponUI.assignedWeapon);
                }
                break;
            case "ItemSlot":
                InventoryItemUI itemUI = tooltipObject as InventoryItemUI;
                if (itemUI != null)
                {
                    //Debug.Log($"Showing Item tooltip for {itemUI.assignedItem}");
                    ShowItemTooltip(itemUI.assignedItem);
                }
                break;
            case "Default":
                //Debug.Log($"Showing default tooltip");
                IDefaultTooltipData defaultData = tooltipObject as IDefaultTooltipData;
                if (defaultData != null)
                {
                    ShowDefaultTooltip(defaultData);
                }
                break;
        }
    }
    
    public void ShowWeaponTooltip(WeaponInstance weapon, bool isUpgrade = false)
    {
        tooltipPanel.SetActive(true);
        mainPanelIcon.SetActive(true);
        if (updateCoroutine != null) 
            StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateLayoutOverTime());
        
        titleText.text = weapon.weaponTitle;
        descriptionText.text = weapon.weaponDescription;
        iconImage.sprite = weapon.weaponIcon;
        valueText.text = weapon.value.ToString();
        uiRarityVisual.ApplyUIRarityVisual(weapon.rarity);
            
        ClearClassTags();
        ClearStatTags();
        
        if (EncounterManager.Instance.CheckIfEncounterActive())
        {
            valueTag.SetActive(true);
        }
        
        mainTag.GetComponentInChildren<TextMeshProUGUI>().text = "Weapon";
        mainTag.SetActive(true);
        
        UpdateClassTags(weapon.classTags);
        ApplyStatTagUI(damageTag, damgeText, weapon.statTags["DMG"]);
        ApplyStatTagUI(cooldownTag, cooldownText, weapon.statTags["SPD"], true);
        ApplyStatTagUI(rangeTag, rangeText, weapon.statTags["RNG"]);
    }
    
    public void ShowItemTooltip(BaseItem item, bool isUpgrade = false)
    {
        tooltipPanel.SetActive(true);
        mainPanelIcon.SetActive(true);
        if (updateCoroutine != null) 
            StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateLayoutOverTime());
        
        titleText.text = item.itemName;
        iconImage.sprite = item.icon;
        valueText.text = item.value.ToString();
        descriptionText.text = item.UpdateDescription(isUpgrade);
        uiRarityVisual.ApplyUIRarityVisual(item.currentRarity);
        
        ClearClassTags();
        ClearStatTags();
        
        if (EncounterManager.Instance.CheckIfEncounterActive())
        {
            valueTag.SetActive(true);
        }
        
        mainTag.GetComponentInChildren<TextMeshProUGUI>().text = "Item";
        mainTag.SetActive(true);
        
        UpdateClassTags(item.classTags);
    }
    
    public void ShowDefaultTooltip(IDefaultTooltipData data)
    {
        tooltipPanel.SetActive(true);
        if (updateCoroutine != null) 
            StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateLayoutOverTime());
        
        titleText.text = data.GetTitle();
        descriptionText.text = data.GetDescription();
        iconImage.sprite = data.GetIcon();
        if (iconImage.sprite == null)
        {
            mainPanelIcon.SetActive(false);
        }
        else
        {
            uiRarityVisual.ApplyDefaultVisual();
        }
        
        //classTagContainer.gameObject.SetActive(false);
        //statTagContainer.gameObject.SetActive(false);
        ClearClassTags();
        ClearStatTags();
        mainTag.SetActive(false);
    }
    
    public void HideTooltip()
    {
        //Debug.Log($"Hiding tooltip");
        ClearClassTags();
        ClearStatTags();
    
        titleText.text = "";
        descriptionText.text = "";
        iconImage.sprite = null;
        
        tooltipPanel.SetActive(false);
    }
    
    private void ClearStatTags()
    {
        valueTag.SetActive(false);
        damageTag.SetActive(false);
        cooldownTag.SetActive(false);
        rangeTag.SetActive(false);
    }
    
    private void ClearClassTags()
    {
        List<Transform> tagsToRemove = new List<Transform>();
        foreach (Transform child in classTagContainer)
        {
            if (!child.CompareTag("Main Tag"))
            {
                tagsToRemove.Add(child);
            }
        }
        foreach (Transform tag in tagsToRemove)
        {
            //Debug.Log($"Destroying tag: {tag.gameObject.name}");
            Destroy(tag.gameObject);
        }
    }
    
    private IEnumerator UpdateLayoutOverTime()
    {
        for (int i = 0; i < 10; i++) // Run updates for a few frames
        {
            UpdateTooltipLayout();
            yield return null; // Wait one frame before running again
        }
    }

    private void UpdateTooltipLayout()
    {
        foreach (var rect in tooltipPanel.GetComponentsInChildren<RectTransform>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
        Canvas.ForceUpdateCanvases();
    }
    
    private void UpdateClassTags(List<TagType> classTags)
    {
        foreach (TagType tag in classTags)
        {
            GameObject newTag = Instantiate(classTagPrefab, classTagContainer);
            newTag.GetComponentInChildren<TextMeshProUGUI>().text = TagDatabase.ClassTags[tag];
        }
    }
    
    private void ApplyStatTagUI(GameObject statObject, TextMeshProUGUI statText, float value, bool isCooldown = false)
    {
        if (value > 0)
        {
            statObject.SetActive(true);
            statText.text = isCooldown ? value.ToString("F1") : Mathf.RoundToInt(value).ToString(); 
        }
        else
        {
            statObject.SetActive(false);
        }
    }
}