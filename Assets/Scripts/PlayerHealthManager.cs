using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerHealthManager : BaseHealthManager
{
    public static PlayerHealthManager Instance { get; private set; }

    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    public GameObject damageNumberPrefab;
    public Canvas statDisplayCanvas;
    private PlayerStatManager playerStats;

    private bool isInvincible = false; 
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public bool playerIsDead = false;

    protected override void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EventManager.Instance.StartListening("StatsChanged", UpdateHealthStats);
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("PlayerRevived", OnPlayerRevive);
        damageNumberPrefab = Resources.Load<GameObject>("Prefabs/DamageNumbers");
        base.Awake();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("StatsChanged", UpdateHealthStats);
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("PlayerRevived", OnPlayerRevive);
    }

    public void AssignStatDisplayCanvas(Canvas statDisplayCanvas)
    {
        this.statDisplayCanvas = statDisplayCanvas;
    }

    private void OnSceneLoaded(string scene)
    {
        if (scene == "GameScene")
        {
            playerStats = PlayerStatManager.Instance;
            maxHealth = playerStats.baseMaxHealth;
            currentHealth = playerStats.currentMaxHealth;

            EventManager.Instance.TriggerEvent("HealthChanged");
            UpdateHealthStats();
        }
    }

    private void OnSceneUnloaded(string scene)
    {
        if (scene == "GameScene")
        {
            playerIsDead = false;
            currentHealth = maxHealth;
            UpdateHealthStats();
            Debug.Log("Player revived!");

            EventManager.Instance.TriggerEvent("HealthChanged");
            UpdateHealthStats();
        }
    }

    private void Update()//for testing
    {
        if (playerIsDead) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            AdjustHealthByPercentage(0.2f);  
        }
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            AdjustHealthByPercentage(-0.2f);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            TakeDamage(10, Vector2.zero, 0);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Heal(10);
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce, bool isFromStatusEffect = false)
    {
        if (playerIsDead || isInvincible) return;

        if (playerStats.IsDodgeSuccessful(playerStats.currentDodgeChance))
        {
            Debug.Log("Attack dodged!");
            return;
        }

        float finalDamage = playerStats.ArmourMitigation(damage, playerStats.currentArmour);

        base.TakeDamage(finalDamage);
        EventManager.Instance.TriggerEvent("HealthChanged", currentHealth, maxHealth);

        if (!isFromStatusEffect)
        {
            ShowDamageNumber(damage, Color.white);

            if (currentHealth <= 0)
            {
                Debug.Log("Ded from knockback");
                DieWithKnockback(knockbackDirection, knockbackForce + 8);
                return;
            }
            else
            {
                // Apply knockback if not dead
                if (knockbackForce > 0)
                {
                    knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: true);
                }
            }
        }
        else if (currentHealth <= 0)
        {
            Die();
        }

        CameraShakeManager.Instance.ShakeCamera(1f, 4f, 0.25f);
    }

    public void Heal(float amount)
    {
        float healableAmount = Mathf.Min(amount, maxHealth - currentHealth);

        currentHealth += healableAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        EventManager.Instance.TriggerEvent("HealthChanged", currentHealth, maxHealth);

        if (healableAmount > 0)
        {
            ShowDamageNumber(healableAmount, Color.green);
        }
    }

    private void UpdateHealthStats()
    {
        float previousMaxHealth = maxHealth;

        maxHealth = playerStats.GetStatValue(StatType.MaxHealth, playerStats.baseMaxHealth);

        currentHealth = Mathf.Round(currentHealth * (maxHealth / previousMaxHealth));

        EventManager.Instance.TriggerEvent("HealthChanged", currentHealth, maxHealth);
    }

    private void DieWithKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: true);
        Die();
    }

    protected override void Die()
    {
        if (playerIsDead) return;
        CameraShakeManager.Instance.ShakeCamera(2f, 6f, 0.25f);
        AudioManager.TriggerSound(clipPrefix: "Player_Death", position: transform.position);
        EventManager.Instance.TriggerEvent("PlayerDied");
        playerIsDead = true;

        Debug.Log("Player died.");
    }

    public void OnPlayerRevive()
    {
        if (!playerIsDead) return;

        playerIsDead = false;
        currentHealth = maxHealth;
        UpdateHealthStats();
        Debug.Log("Player revived!");

        StartInvincibility(2f);
    }

    public void ShowDamageNumber(float damage, Color color, float size = 12f)
    {
        if (statDisplayCanvas == null) return;

        Vector3 damagePosition = transform.position + new Vector3(0, 1, 0);

        GameObject damageNumber = Instantiate(damageNumberPrefab, damagePosition, Quaternion.identity, statDisplayCanvas.transform);

        DamageNumber damageNumberScript = damageNumber.GetComponent<DamageNumber>();
        if (damageNumberScript != null)
        {
            damageNumberScript.SetValue(damage);
            damageNumberScript.SetTextColor(color);
            damageNumberScript.SetTextSize(size);
        }
    }

    private void AdjustHealthByPercentage(float percentage)//FOr TESTING
    {
        float adjustment = maxHealth * percentage;

        if (percentage > 0)
        {
            Heal(adjustment);
        }
        else
        {
            TakeDamage(-adjustment, Vector2.zero, 0);
        }
    }

    public void StartInvincibility(float duration)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            StartCoroutine(InvincibilityTimer(duration));
        }
    }

    private IEnumerator InvincibilityTimer(float duration)
    {
        yield return new WaitForSeconds(duration); // Wait for the invincibility duration
        isInvincible = false; // End invincibility
    }

    public StatusEffect GetStatusEffectByType(Type effectType)
    {
        return activeEffects.FirstOrDefault(effect => effect.GetType() == effectType);
    }

    // Check if any status effect is active by type
    public bool HasStatusEffect<T>() where T : StatusEffect
    {
        return activeEffects.Any(effect => effect is T);
    }

    // Optionally: check if any status effect is active at all
    public bool HasAnyStatusEffect()
    {
        return activeEffects.Count > 0;
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        if (activeEffects.Contains(effect))
        {
            activeEffects.Remove(effect);
            Debug.Log($"Removed {effect.GetType().Name} from player's active effects.");
        }
    }

    protected override void CheckHealthThreshold() { }
}