using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEffect : StatusEffect
{
    private float duration;
    private float radius;
    private EnemyHealthManager targetEnemy;
    private GameObject AoEIndicatorPrefab;
    private GameObject activeAoEIndicator;
    private VisualFeedbackManager visualFeedbackManager;
    private EnemyAI enemy;
    private float flashDuration = 0.3f;
    private float startInterval = 0.6f;
    private float endInterval = 0.05f;
    private float elapsedTime = 0f;

    public KamikazeEffect(GameObject target, float duration, float damage, float radius, MonoBehaviour owner, GameObject explosion = null)
        : base(duration, damage, owner, isEternal: false)
    {
        this.duration = duration;
        this.radius = radius;
        targetEnemy = target.GetComponent<EnemyHealthManager>();
        visualFeedbackManager = target.GetComponent< VisualFeedbackManager>();
        enemy = target.GetComponent<EnemyAI>();
        AoEIndicatorPrefab = Resources.Load<GameObject>("Prefabs/BombAoEIndicator");
    }

    protected override IEnumerator EffectCoroutine()
    {
        activeAoEIndicator = Object.Instantiate(AoEIndicatorPrefab, targetEnemy.transform.position, Quaternion.identity);

        if (activeAoEIndicator != null)
        {
            Debug.Log("AoE Indicator instantiation nig-");
            UpdateAoEIndicatorScale();  // Apply the correct scaling
        }
        else
        {
            Debug.LogError("AoE Indicator instantiation failed!");
        }

        float nextFlashTime = 0f;
        while (elapsedTime < duration && targetEnemy.currentHealth >= 0f)
        {
            //activeAoEIndicator.transform.localScale = new Vector3(20f, 12f, 1f);
            //activeAoEIndicator.transform.position = targetEnemy.transform.position;

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
        enemy.animator.SetTrigger("Power");
        enemy.healthManager.InstantKill(enemy.healthManager.currentHealth, Vector2.zero, 0f);
        Object.Destroy(activeAoEIndicator);
        Remove(targetEnemy.gameObject, false);
    }

    private Vector2 GetAoESize()
    {
        float dampingFactor = 5f;
        float width = (radius / dampingFactor) * 2.0f;  
        float height = (radius / dampingFactor) * 1.25f;
        return new Vector2(width, height);
    }

    private void UpdateAoEIndicatorScale()
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
    }
}