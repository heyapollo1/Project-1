using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    public int currentXP;
    public int currentLevel;
    [HideInInspector] public int xpToNextLevel;

    private bool isProcessingLevelUp = false;
    private Queue<int> levelUpQueue = new Queue<int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
    }
    
    private void OnSceneLoaded(string scene)
    {
        if (scene == "GameScene")
        {
            GameData gameData = SaveManager.LoadGame();
            if (gameData.isNewGame) 
            {
                Debug.Log("New Game!");
                currentXP = 0;
                currentLevel = 1;
                xpToNextLevel = GetXPRequiredForLevel(currentLevel);
                UIManager.Instance.UpdateXPUI(currentXP, xpToNextLevel);
                UIManager.Instance.UpdateLevelUI(currentLevel);
                //EventManager.Instance.TriggerEvent("XPChanged", currentXP, xpToNextLevel);
                //EventManager.Instance.TriggerEvent("UpdateLevelUI", currentLevel);
            }
        }
    }
    

    public void AddXP(int xpAmount)
    {
        if (PlayerHealthManager.Instance.playerIsDead) return;

        currentXP += xpAmount;
        Debug.Log($"Added {xpAmount} XP. New XP: {currentXP}/{xpToNextLevel}");
        //EventManager.Instance.TriggerEvent("XPChanged", currentXP, xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            int excessXP = xpToNextLevel - currentXP;
            currentXP = excessXP;
            currentLevel++;
            xpToNextLevel = GetXPRequiredForLevel(currentLevel);
            levelUpQueue.Enqueue(currentLevel); // Enqueue the next level
        }

        if (!isProcessingLevelUp)
        {
            StartCoroutine(ProcessLevelUpQueue());
        }
        
        EventManager.Instance.TriggerEvent("XPChanged", currentXP, xpToNextLevel);
    }

    private IEnumerator ProcessLevelUpQueue()
    {
        isProcessingLevelUp = true;

        while (levelUpQueue.Count > 0)
        {
            int nextLevel = levelUpQueue.Dequeue();
            yield return StartCoroutine(HandleLevelUp(nextLevel));
        }

        isProcessingLevelUp = false;
    }

    private IEnumerator HandleLevelUp(int nextLevel)
    {
        currentLevel = nextLevel;
        xpToNextLevel = GetXPRequiredForLevel(currentLevel);
        AudioManager.Instance.PlayUISound("Player_LevelUp");
        GameObject levelUpFX = FXManager.Instance.PlayFX("LevelUpFX", transform.position);

        if (levelUpFX != null)
        {
            levelUpFX.transform.SetParent(gameObject.transform);
            levelUpFX.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        }

        yield return StartCoroutine(SlowDownTime());

        GameStateManager.Instance.SetGameState(GameState.LevelUp);
        EventManager.Instance.TriggerEvent("UpdateLevelUI", currentLevel);

        if (currentLevel == 10 || currentLevel == 20)
        {
            EventManager.Instance.TriggerEvent("ShowAbilityChoices", 3);
        }
        else
        {
            EventManager.Instance.TriggerEvent("ShowUpgradeChoices", 3);
        }

        yield return new WaitUntil(() => GameStateManager.Instance.CurrentState == GameState.Playing);

        Debug.Log($"Level {currentLevel} rewards processed.");
    }

    public void GetFreeXP(int xpAmount)
    {
        AddXP(xpAmount);
    }

    private IEnumerator SlowDownTime()
    {
        float duration = 1.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1f, 0.3f, elapsed / duration);
            yield return null;
        }

        EventManager.Instance.TriggerEvent("LevelUp");
    }
    
    public int GetCurrentXP() => currentXP;
    
    public int GetXPToNextLevel() => xpToNextLevel;
    
    public void LoadLevelProgressFromSave(PlayerState playerData)
    {
        Debug.Log($"Loading XP: {playerData.xpAmount}, Level: {playerData.level}");

        currentXP = playerData.xpAmount;
        currentLevel = playerData.level;
        xpToNextLevel = GetXPRequiredForLevel(currentLevel);
        //UIManager.Instance.UpdateXPUI(currentXP, xpToNextLevel);
        //UIManager.Instance.UpdateLevelUI(currentLevel);
    }
    
    private int GetXPRequiredForLevel(int level)
    {
        return 100;
        //return Mathf.FloorToInt(100 * Mathf.Pow(1.2f, level - 1));
    }
    
}
