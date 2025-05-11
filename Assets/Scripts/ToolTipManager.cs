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
    
    public void SetWorldTooltip(IInteractable obj, string tooltipType)
    {
        if (activeHoverTooltip != null) return;
        activeWorldTooltip = obj;
        CheckTooltipType(obj, tooltipType);
    }
    
    public void ClearWorldTooltip()
    {
        activeWorldTooltip = null;
        HideTooltip();
    }
    
    public void SetHoverTooltip(IInteractable obj, string tooltipType)
    {
        if (isWorldTooltipActive())
        {
            HideTooltip();
        }
        
        activeHoverTooltip = obj;
        CheckTooltipType(obj, tooltipType);
    }
    
    public void ClearHoverTooltip()
    {
        activeHoverTooltip = null;
        HideTooltip();
    }

    public IInteractable GetActiveTooltip()
    {
        if (activeHoverTooltip != null) return null;
        
        if (activeWorldTooltip is MonoBehaviour worldObj && worldObj.gameObject != null)
        {
            return activeWorldTooltip;
        }
        return null;
    }
    
    public bool isWorldTooltipActive()
    { 
        return activeWorldTooltip != null;
    }
    
    public bool isHoverTooltipActive()
    { 
       return activeHoverTooltip != null;
    }
    
    public void CheckTooltipType(IInteractable tooltipObject, string tooltipType)
    {
        if (tooltipObject == null)  return;

        //tooltipBackgroundImage.material = isUpgrade ? upgradeMaterial : defaultMaterial;

        switch (tooltipType)
        {
            case "ItemDrop":
                ItemDrop itemDrop = tooltipObject as ItemDrop;
                if (itemDrop != null) ShowItemDropTooltip(itemDrop, itemDrop.isWeapon);
                break;
            case "ItemUI":
                ItemUI itemUI = tooltipObject as ItemUI;
                if (itemUI != null) ShowItemUITooltip(itemUI, itemUI.isWeapon);
                break;
            case "Default":
                //Debug.Log($"Showing default tooltip");
                if (tooltipObject is IDefaultTooltipData defaultData)
                {
                    ShowDefaultTooltip(defaultData);
                }
                break;
        }
    }
    
    public void ShowItemDropTooltip(ItemDrop item, bool isWeapon, bool isUpgrade = false)
    {
        tooltipPanel.SetActive(true);
        mainPanelIcon.SetActive(true);
        if (updateCoroutine != null) 
            StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateLayoutOverTime());
        
        titleText.text = item.name;
        iconImage.sprite = item.spriteRenderer.sprite;
        valueText.text = item.value.ToString();
        uiRarityVisual.ApplyUIRarityVisual(item.rarity);
        descriptionText.text = isWeapon ?  item.dropPayload.weaponScript.weaponDescription : item.dropPayload.itemScript.description;
        ClearClassTags();
        ClearStatTags();
        
        if (EncounterManager.Instance.IsEncounterActive())
        {
            valueTag.SetActive(true);
        }
        
        mainTag.GetComponentInChildren<TextMeshProUGUI>().text = isWeapon ? "Weapon" : "Item";
        mainTag.SetActive(true);
        
        UpdateClassTags(isWeapon ? item.dropPayload.weaponScript.classTags : item.dropPayload.itemScript.classTags);

        if (isWeapon)
        {
            ApplyStatTagUI(damageTag, damgeText, item.dropPayload.weaponScript.statTags["DMG"]);
            ApplyStatTagUI(cooldownTag, cooldownText, item.dropPayload.weaponScript.statTags["SPD"], true);
            ApplyStatTagUI(rangeTag, rangeText, item.dropPayload.weaponScript.statTags["RNG"]);
        }
    }

    public void ShowItemUITooltip(ItemUI item, bool isWeapon, bool isUpgrade = false)
    {
        tooltipPanel.SetActive(true);
        mainPanelIcon.SetActive(true);
        if (updateCoroutine != null) 
            StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateLayoutOverTime());
        
        titleText.text = item.name;
        iconImage.sprite = item.icon.sprite;
        valueText.text = item.value.ToString();
        uiRarityVisual.ApplyUIRarityVisual(item.rarity);
        descriptionText.text = isWeapon ?  item.assignedWeapon.weaponDescription : item.assignedItem.UpdateDescription();
        ClearClassTags();
        ClearStatTags();
        
        if (EncounterManager.Instance.IsEncounterActive())
        {
            valueTag.SetActive(true);
        }
        
        mainTag.GetComponentInChildren<TextMeshProUGUI>().text = isWeapon ? "Weapon" : "Item";
        mainTag.SetActive(true);
        
        UpdateClassTags(isWeapon ? item.assignedWeapon.classTags : item.assignedItem.classTags);

        if (isWeapon)
        {
            ApplyStatTagUI(damageTag, damgeText, item.assignedWeapon.statTags["DMG"]);
            ApplyStatTagUI(cooldownTag, cooldownText, item.assignedWeapon.statTags["SPD"], true);
            ApplyStatTagUI(rangeTag, rangeText, item.assignedWeapon.statTags["RNG"]);
        }
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
        
        ClearClassTags();
        ClearStatTags();
        mainTag.SetActive(false);
    }
    
    public void HideTooltip()
    {
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