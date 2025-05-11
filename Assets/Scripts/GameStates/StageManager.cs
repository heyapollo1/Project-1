using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public StageSettings currentStage;
    public GameObject gameplayMap;
    private StageConfig currentStageConfig;
    
    private float stageDuration; // Total time for the stage
    private float stageTimer = 0;
    private float waveInterval = 2f; 
    private float waveDuration = 2f; // Time between waves
    private int currentStageIndex = 0;
    private int currentWaveIndex = 0;
    private int wavesInStage = 0;
    
    public Transform entryPoint;
    public bool stageIsActive = false;

    public StageBracket GetStageBracket() => currentStageConfig.GetStageBracket(currentStageIndex);
    public void Initialize()
    {
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("EnterStage", SaveCombatState);
        EventManager.Instance.StartListening("StageCompleted", ExitCombatState);
        EventManager.Instance.StartListening("BossStageStart", PrepareBossWave);
        EventManager.Instance.StartListening("WaveCompleted", CompleteWave);
        EventManager.Instance.StartListening("StartStageTimer", TriggerStageTimer);
        EventManager.Instance.StartListening("PrepareStage", PrepareStage);
        
        GameData gameData = SaveManager.LoadGame();
        if (gameData.isNewGame) 
        {
            Debug.Log("Starting a new game. Resetting Stage");
            entryPoint = TransitionManager.Instance.GetGameplaySpawnPoint();
            stageIsActive = false;
            gameplayMap.SetActive(false);
            currentWaveIndex = 0;
            stageTimer = 0f;
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("EnterStage", SaveCombatState);
        EventManager.Instance.StopListening("StageCompleted", ExitCombatState);
        EventManager.Instance.StopListening("BossStageStart", PrepareBossWave);
        EventManager.Instance.StopListening("WaveCompleted", CompleteWave);
        EventManager.Instance.StopListening("StartStageTimer", TriggerStageTimer);
        EventManager.Instance.StartListening("PrepareStage", PrepareStage);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
    }
    
    private void OnSceneUnloaded(string sceneName)
    {
        if (sceneName == "GameScene") CleanupStages();
    }
    
    private void SaveCombatState()
    { 
        if (!stageIsActive) return;
        stageIsActive = true;
        currentWaveIndex = 0;
        stageTimer = 0f;
        wavesInStage = GetWavesInStage(currentStage);
        stageDuration = GetStageDuration(wavesInStage);
        StageUI.Instance.ActivateStageUI(currentStageIndex, currentWaveIndex);
        EventManager.Instance.TriggerEvent("ShowUI");
        GameData currentData = SaveManager.LoadGame();
        SaveManager.SaveGame(currentData);
        
        StartStage();
        Debug.LogWarning($"Saving game: {currentStageIndex} and {currentWaveIndex}");
    }
    
    public void ExitCombatState()
    {
        if (!stageIsActive) return;
        stageIsActive = false;
        currentStageIndex++;
        StageUI.Instance.HandleStageCompleteUI();
        SpawnManager.Instance.ResetSpawnManager();
        MapUI.Instance.EnableMapUI();
        
        GameData currentData = SaveManager.LoadGame();
        SaveManager.SaveGame(currentData);
        Debug.LogWarning($"Stage: {currentStageIndex} completed, upcoming stage: {currentStageIndex + 1}");
    }
    
    public void LoadStageConfig(StageConfig selectedStage)
    {
        if (selectedStage != null)
        {
            currentStageIndex = 0;
            currentStageConfig = selectedStage;
            currentStage = selectedStage.Stages[currentStageIndex];
            
            foreach (StageSettings stage in selectedStage.Stages)
            {
                foreach (WaveSettings wave in stage.wavesInStage)
                {
                    foreach (EnemyPayload enemyReq in wave.enemyPayload)
                    {
                        enemyReq.AutoAssignEnemyType();
                    }
                }
                Debug.Log($"Initialized '{currentStageConfig.stageName}' StageConfig.");
            }
        }
        else Debug.LogError("No stage configuration found or given.");
    }
    
    public void PrepareStage()
    {
        if (stageIsActive) return;
        stageIsActive = true;
        gameplayMap.SetActive(true);
        
        Debug.LogWarning($"Preparing next stage '{currentStageIndex}'.");
        if (currentStageIndex >= currentStageConfig.Stages.Count)
        {
            EndGame();
            Debug.LogWarning($"Game Completed.{currentStageIndex} and {currentStageConfig.Stages.Count}");
            return;
        }
        
        currentStage = currentStageConfig.Stages[currentStageIndex];
        EnemyPoolManager.Instance.PrepareEnemyPool(currentStage);
    }
    
    public void StartStage()
    {
        Debug.LogWarning($"Starting next stage '{currentStageIndex}'.");
        if (currentStage.isBossStage)
        {
            Debug.Log($"boss stage{currentStageIndex} started.");
            AudioManager.Instance.PlayUISound("Stage_BossStart");
            HandleBossStageUI();
            return;
        }
        
        AudioManager.Instance.PlayUISound("Stage_Start");
        StageUI.Instance.UpdateWaveUI(currentStageIndex, currentWaveIndex);
        StageUI.Instance.DisplayStageSplash(currentStageIndex);
    }
    
    private int GetWavesInStage(StageSettings currentStage)
    {
        int waves = currentStage.wavesInStage.Count;
        return waves;
    }

    private float GetStageDuration(int wavesInStage)
    {
        stageDuration = ((wavesInStage) * waveInterval) + (wavesInStage * waveDuration);
        return stageDuration;
    }

    public void TriggerStageTimer()
    {
        StartCoroutine(StageTimerCoroutine(stageTimer, stageDuration));
    }
    
    private IEnumerator StageTimerCoroutine(float currentProgress, float maxProgress)
    {
        Debug.Log("Started stage progression");
        while (currentProgress < maxProgress)
        {
            StageUI.Instance.UpdateStageUI(currentProgress, maxProgress);
            yield return new WaitForSeconds(0.15f);
            currentProgress += 0.15f;
        }
        
        currentProgress = maxProgress;
        StageUI.Instance.UpdateStageUI(currentProgress, maxProgress);
    }

    public void PrepareNextWave()
    {
        if (!stageIsActive) return;
        Debug.Log($"wave:{currentWaveIndex}");
        if (currentWaveIndex >= wavesInStage)
        {
            EnemyManager.Instance.SetAllWavesComplete();
            Debug.Log("No more waves in this stage.");
            return;
        }
        EventManager.Instance.TriggerEvent("WaveUIUpdate", currentStageIndex, currentWaveIndex);
        WaveSettings currentWave = currentStage.wavesInStage[currentWaveIndex];
        EnemyPoolManager.Instance.InitializeEnemyWeights(currentWave);
        
        Debug.LogWarning($"Starting Wave {currentWaveIndex + 1} of Stage {currentStage.stageNumber}");
    }
    
    private void PrepareBossWave()
    {
        Debug.Log($"boss stage");
        WaveSettings currentWave = currentStage.wavesInStage[currentWaveIndex];
        EnemyPoolManager.Instance.InitializeBossWeight(currentWave);
        Debug.LogWarning($"Starting Wave {currentWaveIndex + 1} of Stage {currentStage.stageNumber}");
    }
    
    public void CompleteWave()
    {
        currentWaveIndex++;
        PrepareNextWave();
    }
    
    private void HandleBossStageUI()
    {
        Debug.Log("boss stage triggered");
        StageUI.Instance.DisableStageUI();
        StageUI.Instance.DisplayBossSplash();
    }

    public void EndGame()
    {
        EventManager.Instance.TriggerEvent("GameVictory");
        Debug.Log("All stages COMPLETE!");
    }
    
    public StageConfig GetCurrentStageConfig()
    {
        if (currentStageConfig == null)
        {
            return null;
        }
        return currentStageConfig;
    }
    
    public string GetCurrentStageConfigName()
    {
        if (currentStageConfig == null)
        {
            Debug.Log("No current stage config");
            return "EMPTY";
        }
        return currentStageConfig.stageName;
    }
    
    public int GetCurrentStageIndex()
    {
        if (currentStageConfig == null)
        {
            return 0;
        }
        return currentStageIndex;
    }

    public bool IsStageActive()
    {
        return stageIsActive;
    }
    
    public void SetStageProgress(WorldState worldState)
    {
        Debug.Log($"Stage Level: {worldState.currentStageIndex}. Config: {worldState.currentStageConfigName}");
        if (worldState.currentStageConfigName == "EMPTY")
        {
            Debug.LogWarning("No stage config name provided.");
            return;
        }
        
        stageIsActive = worldState.isStageActive;
        currentWaveIndex = 0;
        stageTimer = 0f;
        currentStageConfig = StageDatabase.Instance.GetStageByName(worldState.currentStageConfigName);
        currentStageIndex = worldState.currentStageIndex;
        currentStage = currentStageConfig.Stages[currentStageIndex];
        wavesInStage = GetWavesInStage(currentStage);
        stageDuration = GetStageDuration(wavesInStage);

        if (stageIsActive)
        {
            EnemyPoolManager.Instance.PrepareEnemyPool(currentStage);
        }
    }
    
    private void CleanupStages()
    {
        currentStage = null;
        stageIsActive = false;
        currentStageIndex = 0;
        currentWaveIndex = 0;
        StopAllCoroutines();
    }
}
