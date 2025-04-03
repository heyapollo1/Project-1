using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDatabase : BaseManager
{
    public static StageDatabase Instance { get; private set; }
    public override int Priority => 30;

    public List<StageConfig> stageConfigs = new List<StageConfig>();

    protected override void OnInitialize()
    {
        Debug.Log($"Loading {stageConfigs.Count} stages in Stage Database");
        LoadStages();
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //EventManager.Instance.StopListening("ShowStageChoices", ShowStageChoices);
    }

    private void LoadStages()
    {
        stageConfigs = new List<StageConfig>();
        StageConfig[] loadedConfigs = Resources.LoadAll<StageConfig>("Stages");
        
        stageConfigs.AddRange(loadedConfigs);
        Debug.Log($"Loaded {stageConfigs.Count} StageConfigs from Resources.");
    }

    /*private void ShowStageChoices()
    {
        List<StageConfig> stageChoices = new List<StageConfig>();
        int totalStages = stageConfigs.Count;
        for (int i = 0; i < totalStages; i++)
        {
            int randomIndex = Random.Range(0, stageConfigs.Count);
            stageChoices.Add(availableStatUpgradesCopy[randomIndex]);
            availableStatUpgradesCopy.RemoveAt(randomIndex);
        }
        EventManager.Instance.TriggerEvent("Stages", upgradeChoices);
    }*/
    
    public List<StageConfig> GetStages()
    {
        return stageConfigs;
        //Debug.Log($"Added upgrade: {newStage.stageName}");
    }
    
    public void AddNewStage(StageConfig newStage)
    {
        stageConfigs.Add(newStage);
        Debug.Log($"Added upgrade: {newStage.stageName}");
    }
    
    public StageConfig GetStageByName(string stageName)
    {
        foreach (var stage in stageConfigs)
        {
            if (stage.stageName == stageName)
            {
                return stage;
            }
        }
        Debug.LogWarning($"Item '{stageName}' not found in the database.");
        return null;
    }

    public void ResetStages()
    {
        stageConfigs.Clear();
        LoadStages();
    }
}