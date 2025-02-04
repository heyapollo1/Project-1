using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StageUI : MonoBehaviour
{
    public static StageUI Instance;

    [SerializeField] private GameObject stageCanvas;
    [SerializeField] private Slider stageProgressSlider; 
    [SerializeField] private TextMeshProUGUI stageText; 
    [SerializeField] private TextMeshProUGUI stageProgressText;
    [SerializeField] private float fadeDuration = 1f; 
    [SerializeField] private float displayDuration = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        Debug.LogWarning($"Initialized StageUI");
        GameManager.Instance.RegisterDependency("StageUI", this);
        EventManager.Instance.StartListening("ResetStageUI", StageCompleteUI);
        stageText.gameObject.SetActive(false);
        GameManager.Instance.MarkSystemReady("StageUI");
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("ResetStageUI", StageCompleteUI);
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

    public void StageCompleteUI()
    {
        stageProgressSlider.gameObject.SetActive(false);
        stageProgressText.text = $"Stage Complete! Enter the Portal!";
    }

    public void DisplayStageSplash(float stageNumber)
    {
        Debug.LogWarning($"Starting Splash");
        stageText.gameObject.SetActive(true);
        StartCoroutine(FadeStageSplash(stageNumber + 1));
    }

    public void DisplayBossSplash()
    {
        stageText.gameObject.SetActive(true);
        StartCoroutine(FadeBossSplash());
    }

    private IEnumerator FadeBossSplash()
    {
        stageText.alpha = 0;
        stageText.text = $"Final Stage";
        Debug.LogWarning($"Starting Splash");
        yield return StartCoroutine(FadeText(stageText, 0, 1, fadeDuration));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeText(stageText, 1, 0, fadeDuration));

        AudioManager.Instance.PlayUISound("Boss_Announcement");
        stageText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        EventManager.Instance.TriggerEvent("BossStageStart");
    }

    private IEnumerator FadeStageSplash(float stageNumber)
    {
        stageText.alpha = 0;
        stageText.text = $"Stage {stageNumber}";
        Debug.LogWarning($"Starting Splash");
        yield return StartCoroutine(FadeText(stageText, 0, 1, fadeDuration));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeText(stageText, 1, 0, fadeDuration));

        EventManager.Instance.TriggerEvent("StageStart");
        stageText.gameObject.SetActive(false);
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

    public void DisableStageUI()
    {
        stageProgressText.text = null;
        stageCanvas.SetActive(false);
    }

    public void ActivateStageUI(float stageNumber, float waveNumber)
    {
        stageCanvas.SetActive(true);
        UpdateWaveUI(stageNumber, waveNumber);
        stageProgressSlider.gameObject.SetActive(true);
        stageProgressSlider.value = 0;
        stageProgressSlider.maxValue = 1;
    }
}
