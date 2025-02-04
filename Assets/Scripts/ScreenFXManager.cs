using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFXManager : MonoBehaviour
{
    public static ScreenFXManager Instance { get; private set; }

    private Image screenOverlay; // Reference to the overlay Image component
    private Color defaultColor = new Color(0, 0, 0, 0); // Fully transparent

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        screenOverlay = GetComponentInChildren<Image>();
        ResetOverlay();
    }

    public void TriggerLevelUpEffect()
    {
        StartCoroutine(LevelUpEffect());
    }

    public void TriggerLowHealthEffect()
    {
        StartCoroutine(LowHealthEffect());
    }

    public void TriggerGameOverEffect()
    {
        StartCoroutine(GameOverEffect());
    }

    public void ResetOverlay()
    {
        if (screenOverlay != null)
        {
            screenOverlay.color = defaultColor; // Reset to transparent
        }
    }

    private IEnumerator LevelUpEffect()
    {
        Color gold = new Color(1f, 0.84f, 0.0f, 0.5f); // Shiny gold color
        screenOverlay.color = gold;

        // Fade in and out effect
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            screenOverlay.color = new Color(gold.r, gold.g, gold.b, Mathf.PingPong(t, 0.5f));
            yield return null;
        }

        ResetOverlay();
    }

    private IEnumerator LowHealthEffect()
    {
        Color redGlow = new Color(0.6f, 0, 0, 0.3f); // Dark red glow
        while (PlayerHealthManager.Instance.CurrentHealth < PlayerHealthManager.Instance.MaxHealth * 0.25f)
        {
            screenOverlay.color = redGlow;
            yield return new WaitForSeconds(0.5f);

            screenOverlay.color = defaultColor;
            yield return new WaitForSeconds(0.5f); // Pulsate
        }

        ResetOverlay();
    }

    private IEnumerator GameOverEffect()
    {
        Color darkRed = new Color(0.3f, 0, 0, 0.8f); // Dark red ambiance
        screenOverlay.color = darkRed;

        yield return new WaitForSeconds(1f); // Hold the effect
        ResetOverlay();
    }
}