using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MapUI : MonoBehaviour
{
     public static MapUI Instance { get; private set; }

    [SerializeField] public GameObject mapPanel;
    [SerializeField] private GameObject mapPrefab;
    [SerializeField] private GameObject encounterCardPrefab;
    [SerializeField] private Transform encounterContainer;
    [SerializeField] private Button openMapButton;
    [SerializeField] private Button closeMapButton;

    private bool travelPermitted = false;
    private bool mapOpen = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        openMapButton.onClick.AddListener(OpenMap);
        closeMapButton.onClick.AddListener(CloseMap);
        
        GameData gameData = SaveManager.LoadGame();
        if (gameData.isNewGame) 
        {
            Debug.Log("Starting a new game.");
            mapPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        openMapButton.onClick.RemoveAllListeners();
        closeMapButton.onClick.RemoveAllListeners();
    }
    
    public void EnableMapUI()
    {
        Debug.Log("enabled map");
        travelPermitted = true;
        mapPrefab.SetActive(false);
        mapPanel.SetActive(true);
        openMapButton.gameObject.SetActive(true);
    }
    
    public void DisableMapUI()
    {
        Debug.Log("disabled map");
        travelPermitted = false;
        mapPrefab.SetActive(false);
        mapPanel.SetActive(false);
        openMapButton.gameObject.SetActive(false);
    }
    
    public bool IsMapActive() => travelPermitted;
    
    public void ApplyMapUIState(WorldState state)
    {
        travelPermitted = state.isMapActive;
        if (travelPermitted)
        {
            EnableMapUI();
        }
        else
        {
            DisableMapUI();
        }
    }
    
    private void OpenMap()
    {
        if (mapOpen)
        { 
            CloseMap();
            return;
        }
        Debug.Log("map button clicked");
        mapOpen = true;
        if (!EncounterManager.Instance.CheckIfEncounterActive() && !StageManager.Instance.isStageActive)
        {
            Debug.Log("opening map");
            mapPrefab.SetActive(true);
            PlayerController.Instance.DisableControls();
            PlayerAbilityManager.Instance.DisableAbilities();
            WeaponManager.Instance.DisableWeapons();
            
            List<EncounterData> selectedEncounters = EncounterManager.Instance.GetEventChoices();
            DisplayEncounterChoices(selectedEncounters);
        }
        else
        {
            Debug.Log("Transition to stage");
            var encounter = EncounterManager.Instance.GetCurrentEncounterData();
            encounter.EndEncounter();
            DisableMapUI();
        }
    }
    
    public void DisplayEncounterChoices(List<EncounterData> encounters)
    {
        mapPanel.SetActive(true);
        foreach (Transform child in encounterContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var encounter in encounters)
        {
            GameObject encounterOptionPrefab = Instantiate(encounterCardPrefab, encounterContainer);
            SetEncounterData(encounterOptionPrefab, encounter);
        }
    }
    
    private void SetEncounterData(GameObject encounterOptionPrefab, EncounterData encounterData)
    {
        if (encounterData == null)
        {
            Debug.LogError("Upgrade data missing!");
            return;
        }
        TextMeshProUGUI titleText = encounterOptionPrefab.transform.Find("EncounterName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = encounterOptionPrefab.transform.Find("EncounterDescription")?.GetComponent<TextMeshProUGUI>();
        Image iconImage = encounterOptionPrefab.transform.Find("EncounterIcon")?.GetComponent<Image>();
        
        if (titleText != null) titleText.text = encounterData.encounterName;
        if (descriptionText != null) descriptionText.text = encounterData.encounterDescription;
        if (iconImage != null) iconImage.sprite = encounterData.encounterIcon; 

        Button encounterSelectButton = encounterOptionPrefab.GetComponent<Button>();
        if (encounterSelectButton != null)
        {
            encounterSelectButton.onClick.RemoveAllListeners();
            encounterSelectButton.onClick.AddListener(() => OnEncounterSelected(encounterData));
        }
    }

    private void OnEncounterSelected(EncounterData selectedEncounter)
    {
        CloseMap();
        DisableMapUI();
        Debug.LogWarning("Encounter selected");
        int stageIndex = StageManager.Instance.GetCurrentStageIndex();
        selectedEncounter.TriggerEncounter(stageIndex);
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
    }
    
    private void CloseMap()
    {
        mapOpen = false;
        mapPrefab.SetActive(false);
    }

}