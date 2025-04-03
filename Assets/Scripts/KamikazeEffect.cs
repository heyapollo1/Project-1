using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEffect : StatusEffect
{
    private float duration;
    private float radius;
    private ILivingEntity targetEnemy;
    private GameObject AoEIndicatorPrefab;
    private GameObject activeAoEIndicator;
    private VisualFeedbackManager visualFeedbackManager;
    private Animator animator;
    private float flashDuration = 0.3f;
    private float startInterval = 0.6f;
    private float endInterval = 0.05f;
    private float elapsedTime = 0f;

    public KamikazeEffect(ILivingEntity target, float duration, float damage, float radius, MonoBehaviour owner, GameObject explosion = null)
        : base(duration, damage, target, owner, isEternal: false)
    {
        this.duration = duration;
        this.radius = radius;
        targetEnemy = target;
        //visualFeedbackManager = target.GetVisualFeedbackManager();
        visualFeedbackManager = owner.GetComponent<VisualFeedbackManager>();
        animator = owner.GetComponent<Animator>();
        AoEIndicatorPrefab = Resources.Load<GameObject>("Prefabs/BombAoEIndicator");
    }

    protected override IEnumerator EffectCoroutine()
    {
        /*activeAoEIndicator = Object.Instantiate(AoEIndicatorPrefab, targetEnemy.transform.position, Quaternion.identity);

        if (activeAoEIndicator != null)
        {
            Debug.Log("AoE Indicator instantiation nig-");
            UpdateAoEIndicatorScale();  // Apply the correct scaling
        }
        else
        {
            Debug.LogError("AoE Indicator instantiation failed!");
        }*/

        float nextFlashTime = 0f;
        while (elapsedTime < duration && targetEnemy.GetCurrentHealth() >= 0f)
        {
            if (elapsedTime >= nextFlashTime)
            {
                visualFeedbackManager.TriggerHitEffect(flashDuration, 0.25f, Color.white);

                // Calculate the progressively shorter interval and set the next flash time
                float flashInterval = Mathf.Lerp(startInterval, endInterval, elapsedTime / duration);
                nextFlashTime = elapsedTime + flashInterval;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        animator.SetTrigger("Power");
        targetEnemy.TakeDamage(targetEnemy.GetCurrentHealth(), Vector2.zero, 0f, DamageSource.Execution);
        Object.Destroy(activeAoEIndicator);
        RemoveEffect();
    }

    private Vector2 GetAoESize()
    {
        float dampingFactor = 5f;
        float width = (radius / dampingFactor) * 2.0f;  
        float height = (radius / dampingFactor) * 1.25f;
        return new Vector2(width, height);
    }

    /*private void UpdateAoEIndicatorScale()
    {
        if (activeAoEIndicator != null)
        {
            Vector2 targetSize = GetAoESize();
            SpriteRenderer spriteRenderer = activeAoEIndicator.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                float spriteWidth = spriteRenderer.sprite.bounds.size.x;
                float spriteHeight = spriteRenderer.sprite.bounds.size.y;

                // Log for debugging
                Debug.Log($"Sprite Width: {spriteWidth}, Sprite Height: {spriteHeight}");
                Debug.Log($"Target Width: {targetSize.x}, Target Height: {targetSize.y}");

                // Ensure sprite dimensions are reasonable before applying scale
                if (spriteWidth > 0 && spriteHeight > 0)
                {
                    float scaleX = targetSize.x / spriteWidth;
                    float scaleY = targetSize.y / spriteHeight;
                    activeAoEIndicator.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                }
                else
                {
                    Debug.LogWarning("Sprite dimensions are zero; check the sprite asset.");
                }
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found on the AoE indicator.");
            }
        }
    }*/
}