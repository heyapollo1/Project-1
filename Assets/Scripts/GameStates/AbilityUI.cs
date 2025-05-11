using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class AbilityUI : MonoBehaviour
{
    public Image abilityPanel;
    public GameObject abilityUISlotPrefab;
    public Transform abilityUIContainer;

    private List<GameObject> abilitySlots = new List<GameObject>(); // List to track slot GameObjects

    public void Initialize()
    {
        GameManager.Instance.RegisterDependency("AbilityUI", this);
        //EventManager.Instance.StartListening("AbilityUnlocked", AddAbilityToUI);
        SetupSlots();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("AbilityUnlocked", AddAbilityToUI);
        ClearAllSlots();
    }


    private void SetupSlots()
    {
        if (abilitySlots.Count == 0) // Create initial slots if none exist
        {
            for (int i = 0; i < 3; i++)
            {
                //Debug.LogWarning("creating ability slots");
                GameObject slot = Instantiate(abilityUISlotPrefab, abilityUIContainer);
                abilitySlots.Add(slot);
                slot.SetActive(false); // Initially hide all slots
            }
        }
        GameManager.Instance.MarkSystemReady("AbilityUI");
    }

    private void AddAbilityToUI(PlayerAbilityData abilityData)
    {
        if (abilityData == null || abilityData.abilityPrefab == null)
        {
            Debug.LogError("Invalid ability data or prefab.");
            return;
        }

        PlayerAbilityBase ability = abilityData.abilityPrefab.GetComponent<PlayerAbilityBase>();
        if (ability == null)
        {
            Debug.LogError("Ability prefab does not have a PlayerAbilityBase component.");
            return;
        }

        GameObject abilitySlot = abilitySlots.Find(slot => !slot.activeSelf);
        if (abilitySlot == null)
        {
            abilitySlot = Instantiate(abilityUISlotPrefab, abilityUIContainer);
            abilitySlots.Add(abilitySlot);
        }

        abilitySlot.SetActive(true); // Ensure the slot is visible
        AbilityUISlot slotUI = abilitySlot.GetComponent<AbilityUISlot>();
        if (slotUI != null)
        {
            slotUI.Initialize(ability);
        }
        else
        {
            Debug.LogError("AbilityUISlot component not found on ability slot prefab.");
        }
    }

    public void ClearAllSlots()
    {
        foreach (var slot in abilitySlots) Destroy(slot);
        abilitySlots.Clear();
    }
}
