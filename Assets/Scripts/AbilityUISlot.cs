using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerAbilityBase associatedAbility;
    public Slider cooldownSlider;
    private Image abilityIcon;

    private void Awake()
    {
        cooldownSlider = GetComponentInChildren<Slider>();
        abilityIcon = transform.Find("AbilityUIIcon")?.GetComponent<Image>();

        if (cooldownSlider == null)
        {
            Debug.LogError("Cooldown Slider not found in AbilityUISlot.");
        }

        if (abilityIcon == null)
        {
            Debug.LogError("Ability Icon (AbilityUIIcon) not found in AbilityUISlot.");
        }
    }


    public void Initialize(PlayerAbilityBase ability)
    {
        if (ability == null)
        {
            Debug.LogError("Ability or Slider is null during AbilityUISlot initialization.");
            return;
        }
        EventManager.Instance.StartListening("AbilityUsed", OnAbilityUsed);

        associatedAbility = ability;

        // Initialize ability icon
        if (abilityIcon != null)
        {
            if (associatedAbility.abilityData.abilityIcon != null)
            {
                abilityIcon.sprite = associatedAbility.abilityData.abilityIcon;
                Debug.LogWarning("icon:" + associatedAbility.abilityData.abilityIcon);
                abilityIcon.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Ability icon is missing in ability data.");
            }
        }

        // Initialize cooldown slider
        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0;
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("AbilityUsed", OnAbilityUsed);
    }

    private void OnAbilityUsed(PlayerAbilityData abilityData, float cooldownDuration)
    {
        if (associatedAbility != null && associatedAbility.abilityData == abilityData)
        {
            StartCoroutine(StartCooldown(cooldownDuration));
        }
    }

    private IEnumerator StartCooldown(float cooldownDuration)
    {
        float timer = cooldownDuration;
        cooldownSlider.value = 1;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float sliderValue = timer / cooldownDuration;
            cooldownSlider.value = sliderValue;
            yield return null;
        }

        cooldownSlider.value = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (associatedAbility != null)
        {
            Tooltip.Instance.ShowTooltip(associatedAbility.abilityData.abilityTitle, associatedAbility.abilityData.abilityDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }
}