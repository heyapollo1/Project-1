using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageNumber : MonoBehaviour
{
    public float riseSpeed = 1f;         // How fast the number rises
    public float duration = 1f;          // How long it stays on screen
    public float fadeDuration = 0.5f;    // How long it takes to fade out

    public TextMeshProUGUI damageText;  // Reference to the text component
    private Color originalColor;

    void Start()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        originalColor = damageText.color;

        // Start rising and fading
        StartCoroutine(RiseAndFade());
    }

    public void SetValue(float value)
    {
        damageText.text = value.ToString();  // Set the damage value as text
    }

    public void SetTextColor(Color color)
    {
        damageText.color = color; // Change the color of the text
        originalColor = color;    // Update original color so it fades out correctly
    }

    public void SetTextSize(float size)
    {
        damageText.fontSize = size; // Change the size of the text
    }

    public void SetTextStyle(FontStyles style)
    {
        damageText.fontStyle = style; // Apply bold, italic, etc.
    }

    private IEnumerator RiseAndFade()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Move the text upwards
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);

            // Fade out over time
            if (elapsed >= (duration - fadeDuration))
            {
                float alpha = Mathf.Lerp(1f, 0f, (elapsed - (duration - fadeDuration)) / fadeDuration);
                damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
