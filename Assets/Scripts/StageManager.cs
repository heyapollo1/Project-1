using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseManager
{
    public static StageManager Instance;

    public override int Priority => 10;
    public List<GameConfig> stageConfigs;

    private StageUI stageUI;
    public GameObject portalPrefab;

    private float stageDuration; // Total time for the stage
    private float stageTimer;
    private float waveInterval = 2f; 
    private float waveDuration = 2f; // Time between waves
    private int currentStageIndex = 0;
    private int currentWaveIndex = 0;
    private int wavesInStage = 0;
    private bool isStageActive = false;

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("StartStage1", StartGame);
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("StageStart", StartStage);
        EventManager.Instance.StartListening("BossStageStart", StartBossStage);
        EventManager.Instance.StartListening("WaveCompleted", HandleNextWave);
        EventManager.Instance.StartListening("TravelArrival", PrepareNextStage);
        EventManager.Instance.StartListening("StartStageTimer", StartStageTimer);
        //EventManager.Instance.StartListening("TravelDeparture", PrepareNextStage);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("StartStage1", StartGame);
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("StageStart", StartStage);
        EventManager.Instance.StopListening("WaveCompleted", HandleNextWave);
        EventManager.Instance.StopListening("TravelArrival", PrepareNextStage);
        EventManager.Instance.StopListening("StartStageTimer", StartStageTimer);
        //EventManager.Instance.StartListening("TravelDeparture", PrepareNextStage);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnSceneLoaded(string sceneName)
    {
        if (sceneName == "GameScene")
        {
            LoadStageConfigs();
        }
    }

    private void OnSceneUnloaded(string sceneName)
    {
        if (sceneName == "GameScene")
        {
            Debug.LogWarning("Stage Cleanup");
            CleanupStages();
        }
    }

    private void LoadStageConfigs()
    {
        stageConfigs = new List<GameConfig>();
        GameConfig[] loadedConfigs = Resources.LoadAll<GameConfig>("Stages");

        stageConfigs.AddRange(loadedConfigs);
        Debug.Log($"Loaded {stageConfigs.Count} StageConfigs from Resources.");
    }

    public void StartGame()
    {
        Debug.LogWarning("Start Game!");
        stageUI = GameManager.Instance.GetDependency<StageUI>("StageUI");
        currentStageIndex = 0;
        HandlStagePreparation();
    }

    public void PrepareNextStage()
    {
        stageUI.ActivateStageUI(currentStageIndex, currentWaveIndex);
        HandlStagePreparation();
    }

    private void HandlStagePreparation()
    {
        if (isStageActive) return;

        isStageActive = true;

        currentWaveIndex = 0;
        stageTimer = 0f; 

        GameConfig currentStageConfig = stageConfigs[0];
        StageSettings currentStage = currentStageConfig.Stages[currentStageIndex];
        EnemyPoolManager.Instance.PrepareEnemyPool(currentStage);

        wavesInStage = currentStage.wavesInStage.Count;
        stageDuration = ((wavesInStage - 1) * waveInterval) + (wavesInStage * waveDuration);

        if (currentStage.isBossStage)
        {
            Debug.Log($"boss stage triggered");
            AudioManager.Instance.PlayUISound("Stage_BossStart");
            HandleBossStage();
            return;
        }
        else
        {
            AudioManager.Instance.PlayUISound("Stage_Start");
        }

        stageUI.UpdateWaveUI(currentStageIndex, currentWaveIndex);
        stageUI.DisplayStageSplash(currentStageIndex);
    }

    public void StartStage()
    {
        if (!isStageActive) return;

        PrepareWave();
    }

    public void StartStageTimer()
    {
        StartCoroutine(StageTimerCoroutine(stageTimer, stageDuration));
    }

    public void PrepareWave()
    {
        if (!isStageActive) return;
        Debug.Log($"stage:{currentStageIndex + 1}");
        StageSettings currentStage = stageConfigs[0].Stages[currentStageIndex];
        if (currentWaveIndex >= currentStage.wavesInStage.Count)
        {
            Debug.Log("No more waves in this stage.");
            return;
        }

        EventManager.Instance.TriggerEvent("WaveUIUpdate", currentStageIndex, currentWaveIndex);

        WaveSettings currentWave = currentStage.wavesInStage[currentWaveIndex];
        EnemyPoolManager.Instance.InitializeEnemyWeights(currentWave);
        Debug.LogWarning($"Starting Wave {currentWaveIndex + 1} of Stage {currentStage.stageNumber}");
    }

    private IEnumerator StageTimerCoroutine(float currentProgress, float maxProgress)//UI progression, visuals
    {
        Debug.Log("Started stage progression");
        while (currentProgress < maxProgress)
        {
            stageUI.UpdateStageUI(currentProgress, maxProgress);
            yield return new WaitForSeconds(0.15f);
            currentProgress += 0.15f;
        }

        if (isStageActive)
        {
            currentProgress = maxProgress;
            stageUI.UpdateStageUI(currentProgress, maxProgress);
        }
    }

    public void HandleNextWave()
    {
        if (!isStageActive) return;

        currentWaveIndex++;

        if (wavesInStage == currentWaveIndex)
        {
            StartCoroutine(CompleteStage());
            return;
        }

        PrepareWave();
    }

    private IEnumerator CompleteStage()
    {
        isStageActive = false;
        currentStageIndex++;

        yield return new WaitForSeconds(1f);

        if (currentStageIndex > stageConfigs.Count)
        {
            EndGame();
            Debug.LogWarning($"No Boss Level, Game Completed.");
        }
        else
        {
            AudioManager.Instance.PlayUISound("Upgrade_Select");
            Debug.LogWarning($"Stage: {currentStageIndex} completed, upcoming stage: {currentStageIndex + 1}");
            EventManager.Instance.TriggerEvent("StageComplete");
        }
    }

    private void HandleBossStage()
    {
        //AudioManager.Instance.PlayUISound("Stage_Start");
        Debug.Log($"boss stage triggered");
        stageUI.DisableStageUI();
        stageUI.DisplayBossSplash();
    }

    private void StartBossStage()
    {
        if (!isStageActive) return;

        Debug.Log($"final stage");
        StageSettings currentStage = stageConfigs[0].Stages[currentStageIndex];
        WaveSettings currentWave = currentStage.wavesInStage[currentWaveIndex];
        EnemyPoolManager.Instance.InitializeBossWeight(currentWave);
        Debug.LogWarning($"Starting Wave {currentWaveIndex + 1} of Stage {currentStage.stageNumber}");
    }

    public void EndGame()
    {
        EventManager.Instance.TriggerEvent("GameVictory");
        Debug.Log("All stages COMPLETE!");
    }

    private void CleanupStages()
    {
        isStageActive = false;
        currentStageIndex = 0;
        currentWaveIndex = 0;
        StopAllCoroutines();
        stageConfigs.Clear();
    }
}
