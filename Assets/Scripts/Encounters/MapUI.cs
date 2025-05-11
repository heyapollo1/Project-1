using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
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

    private EncounterManager encounterManager;
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
        encounterManager = EncounterManager.Instance;
            
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
        if (!encounterManager.IsEncounterActive()) LoadEncounters();
        else LoadGameplay();
    }

    private void LoadEncounters()
    {
        mapPrefab.SetActive(true);
        PlayerController.Instance.DisableControls();
        PlayerAbilityManager.Instance.DisableAbilities();
        
        List<EncounterData> selectedEncounters = encounterManager.GetEventChoices();
        DisplayEncounterChoices(selectedEncounters);
    }

    private void LoadGameplay()
    {
        StartCoroutine(OnGameplaySelected());
    }

    public void DisplayEncounterChoices(List<EncounterData> encounters)
    {
        Debug.Log($"Displaying encounter choices: {encounters.Count}");
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
        BaseEncounter encounterScript = EncounterManager.Instance.GetEncounterFromType(encounterData.encounterType);
        if (encounterSelectButton != null)
        {
            encounterSelectButton.onClick.RemoveAllListeners();
            encounterSelectButton.onClick.AddListener(() => StartCoroutine(OnEncounterSelected(encounterScript)));
        }
    }
    
    private IEnumerator OnEncounterSelected(BaseEncounter selectedEncounter)
    {
        CloseMap();
        DisableMapUI();
        StageUI.Instance.DisableStageUI();
        selectedEncounter.PrepareEncounter();
        
        yield return null;
        
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
        Debug.Log($"Encounter selected: {selectedEncounter.entryPoint}");
        CutsceneManager.Instance.StartCutscene("TeleportEncounterCutscene", selectedEncounter.entryPoint);
    }
    
    private IEnumerator OnGameplaySelected()
    {
        CloseMap();
        DisableMapUI();
        encounterManager.GetCurrentEncounter().EndEncounter();

        yield return null;
        
        Transform spawnPoint = TransitionManager.Instance.GetGameplaySpawnPoint();
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
        CutsceneManager.Instance.StartCutscene("TeleportStageCutscene", spawnPoint);
    }
    
    private void CloseMap()
    {
        mapOpen = false;
        mapPrefab.SetActive(false);
    }

}