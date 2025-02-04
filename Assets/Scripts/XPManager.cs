using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    private int currentXP;
    private int currentLevel = 1;
    private int xpToNextLevel = 100;

    private bool isProcessingLevelUp = false;
    private Queue<int> levelUpQueue = new Queue<int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EventManager.Instance.StartListening("InstantLevelUp", LevelUp);
        InitializeXP();
    }

    private void InitializeXP()
    {
        currentXP = 0;
        currentLevel = 1;
        EventManager.Instance.TriggerEvent("XPChanged", currentXP, xpToNextLevel);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("InstantLevel", LevelUp);
    }

    public void AddXP(int xpAmount)
    {
        if (PlayerHealthManager.Instance.playerIsDead) return;

        currentXP += xpAmount;
        EventManager.Instance.TriggerEvent("XPChanged", currentXP, xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            levelUpQueue.Enqueue(currentLevel + 1); // Enqueue the next level
        }

        if (!isProcessingLevelUp)
        {
            StartCoroutine(ProcessLevelUpQueue());
        }
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

        if (currentLevel == 2 || currentLevel == 5)
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

    private void LevelUp()
    {
        if (PlayerHealthManager.Instance.playerIsDead) return;

        currentLevel++;
        currentXP -= xpToNextLevel;
        levelUpQueue.Enqueue(currentLevel);

        if (!isProcessingLevelUp)
        {
            StartCoroutine(ProcessLevelUpQueue());
        }
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


    public void ResetXP()
    {
        InitializeXP();
    }
}
