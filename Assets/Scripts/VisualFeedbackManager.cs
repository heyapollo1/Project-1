using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualFeedbackManager : MonoBehaviour
{
    public static PlayerHealthManager Instance { get; private set; }
    
    private MaterialPropertyBlock propertyBlock;
    private SpriteRenderer spriteRenderer;
    private float whiteFlashCooldownTimer = 0f;
    private float whiteFlashCooldown = 0.05f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (whiteFlashCooldownTimer > 0)
        {
            whiteFlashCooldownTimer -= Time.deltaTime;
        }
    }

    public void TriggerHitEffect(float flashTime, float holdTime, Color color)
    {
        StartCoroutine(FlashEffect(flashTime, holdTime, color)); 
    }

    private IEnumerator FlashEffect(float flashTime, float holdTime, Color color, float intensity = 5f)
    {
        Color hdrColor = new Color(color.r * intensity, color.g * intensity, color.b * intensity, 1f);
        //propertyBlock.SetColor("_EmissionColor", hdrColor);
        propertyBlock.SetColor("_Flashcolor", hdrColor);
        propertyBlock.SetFloat("_FlashAmount", 1f);  // Set to full intensity
        spriteRenderer.SetPropertyBlock(propertyBlock); // Apply property block with full flash

        // Hold at full intensity for the specified duration
        yield return new WaitForSeconds(holdTime);

        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            float smoothFlashAmount = Mathf.SmoothStep(1f, 0f, elapsedTime / flashTime);
            propertyBlock.SetFloat("_FlashAmount", smoothFlashAmount);
            spriteRenderer.SetPropertyBlock(propertyBlock);

            yield return null;
        }

        // Reset flash amount after flashing
        propertyBlock.SetFloat("_FlashAmount", 0f);
        spriteRenderer.SetPropertyBlock(propertyBlock);

        whiteFlashCooldownTimer = whiteFlashCooldown;
    }

    public IEnumerator FadeOut(float fadeDuration, GameObject target)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float fadeValue = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            propertyBlock.SetFloat("_FadeAmount", fadeValue);  // Set fade amount for the shader
            spriteRenderer.SetPropertyBlock(propertyBlock);  // Apply property block for fading effect

            yield return null;
        }

        // Ensure fade is completely off at the end
        propertyBlock.SetFloat("_FadeAmount", 0f);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}
