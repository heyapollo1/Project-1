using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StageUI : MonoBehaviour
{
    public static StageUI Instance;

    [SerializeField] public GameObject stageCanvas;
    [SerializeField] private Slider stageProgressSlider; 
    [SerializeField] private TextMeshProUGUI stageSplashText; 
    [SerializeField] private TextMeshProUGUI stageProgressText;
    [SerializeField] private float fadeDuration = 0.1f; 
    [SerializeField] private float displayDuration = 0.1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        StageManager stageManager = StageManager.Instance;
        stageSplashText.gameObject.SetActive(false);
        
        Debug.LogWarning($"Initialized StageUI");
        if (stageManager.isStageActive && !EncounterManager.Instance.CheckIfEncounterActive())
        {
            Debug.LogWarning($"Initialized Encounter StageUI");
            ActivateStageUI(stageManager.GetCurrentStageIndex(), 0);
            DisplayStageSplash(stageManager.GetCurrentStageIndex());
        }
        else
        {
            stageCanvas.SetActive(false);
        }
    }
    
    public void UpdateStageUI(float currentProgress, float maxProgress)
    {
        stageProgressSlider.value = currentProgress / maxProgress;
    }

    public void UpdateWaveUI(float stageNumber, float waveNumber)
    {
        Debug.LogWarning($"Update WaveUI");
        stageProgressText.text = $"\n\nStage {stageNumber + 1} - Wave {waveNumber + 1}";
    }
    
    public void DisplayStageSplash(float stageNumber)
    {
        Debug.LogWarning($"Starting Splash for Stage {stageNumber}");

        // ✅ Ensure canvas is active before fading
        stageCanvas.SetActive(true); 

        if (stageSplashText.gameObject == null)
        {
            Debug.LogError("stageSplashText is NULL before enabling!");
            return;
        }

        stageSplashText.gameObject.SetActive(true);

        if (!stageSplashText.isActiveAndEnabled)
        {
            Debug.LogError("stageSplashText failed to activate!");
            return;
        }

        Debug.LogWarning($"Splash Text Activated: {stageSplashText.gameObject.activeSelf}");
        //StartCoroutine(DelayedStageSplash(stageNumber + 1));
        StartCoroutine(FadeStageSplash(stageNumber + 1));
    }

    public void DisplayBossSplash()
    {
        if (stageCanvas == false) return;
        stageSplashText.gameObject.SetActive(true);
        StartCoroutine(FadeBossSplash());
    }
    
    private IEnumerator FadeBossSplash()
    {
        yield return new WaitForEndOfFrame();
        
        stageSplashText.alpha = 0;
        stageSplashText.text = $"Final Stage";
        Debug.LogWarning($"Starting Splash");
        yield return StartCoroutine(FadeText(stageSplashText, 0, 1, fadeDuration));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeText(stageSplashText, 1, 0, fadeDuration));

        AudioManager.Instance.PlayUISound("Boss_Announcement");
        stageSplashText.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        EventManager.Instance.TriggerEvent("BossStageStart");
    }

    public IEnumerator FadeStageSplash(float stageNumber)
    {
        yield return new WaitForEndOfFrame();
        
        stageSplashText.alpha = 0;
        stageSplashText.text = $"Stage {stageNumber}";
        Debug.LogWarning($"Starting Splash");
        yield return StartCoroutine(FadeText(stageSplashText, 0, 1, fadeDuration));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeText(stageSplashText, 1, 0, fadeDuration));

        //EventManager.Instance.TriggerEvent("StageStart");
        StageManager.Instance.PrepareNextWave();
        stageSplashText.gameObject.SetActive(false); 
        Debug.LogWarning($"Ending Splash");
    }

    private IEnumerator FadeText(TextMeshProUGUI stageText, float start, float end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            stageText.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        stageText.alpha = end;
    }

    public void HandleStageCompleteUI()
    {
        stageProgressSlider.gameObject.SetActive(false);
        stageProgressText.text = $"Stage Complete! Enter the Portal!";
    }
    
    public void DisableStageUI()
    {
        stageProgressText.text = null;
        stageCanvas.SetActive(false);
    }

    public void ActivateStageUI(float stageNumber, float waveNumber)
    {
        Debug.Log($"Activating StageUI");
        stageCanvas.SetActive(true);
        UpdateWaveUI(stageNumber, waveNumber);
        stageProgressSlider.gameObject.SetActive(true);
        stageProgressSlider.value = 0;
        stageProgressSlider.maxValue = 1;
    }
}
