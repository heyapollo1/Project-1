using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class StageSelectUI : MonoBehaviour
{
    public static StageSelectUI Instance { get; private set; }

    [SerializeField] public GameObject stageSelectPanel;
    [SerializeField] private GameObject stageCardPrefab;
    [SerializeField] private Transform stageContainer;
    [SerializeField] private Button returnButton;
    
    private bool stageSelectPanelActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(CloseStageSelectPanel);
        stageSelectPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        returnButton.onClick.RemoveAllListeners();
    }

    public void DisplayStageChoices(List<StageConfig> stages)
    {
        stageSelectPanelActive = true;
        stageSelectPanel.SetActive(true);

        foreach (Transform child in stageContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var stage in stages)
        {
            GameObject stageOptionPrefab = Instantiate(stageCardPrefab, stageContainer);
            SetStageData(stageOptionPrefab, stage);
        }
    }
    
    private void SetStageData(GameObject stageOptionPrefab, StageConfig stageData)
    {
        if (stageData == null)
        {
            Debug.LogError("Upgrade data missing!");
            return;
        }

        TextMeshProUGUI titleText = stageOptionPrefab.transform.Find("StageName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = stageOptionPrefab.transform.Find("StageDescription")?.GetComponent<TextMeshProUGUI>();
        Image iconImage = stageOptionPrefab.transform.Find("UpgradeIcon")?.GetComponent<Image>();
        
        if (titleText != null) titleText.text = stageData.stageName;
        if (descriptionText != null) descriptionText.text = stageData.stageDescription; // red udnerltined.
        if (iconImage != null) iconImage.sprite = stageData.stageIcon; // red underlined.

        Button stageSelectButton = stageOptionPrefab.GetComponent<Button>();
        if (stageSelectButton != null)
        {
            stageSelectButton.onClick.RemoveAllListeners();
            stageSelectButton.onClick.AddListener(() => StartCoroutine(OnStageSelected(stageData)));
        }
    }

    private IEnumerator OnStageSelected(StageConfig selectedStage)
    {
        Debug.Log($"Stage selected: {selectedStage.stageName}");
        AudioManager.Instance.PlayUISound("Upgrade_Select", 0.7f);
        StageManager.Instance.LoadStageConfig(selectedStage);
        
        yield return null;
        
        EventManager.Instance.TriggerEvent("StageInitialized", selectedStage);
        stageSelectPanelActive = false;
        CloseStageSelectPanel();
    }

    public void CloseStageSelectPanel()
    {
        stageSelectPanel.SetActive(false);
    }
}
