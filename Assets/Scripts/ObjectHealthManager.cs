using System.Collections;
using UnityEngine;

public class ObjectHealthManager : BaseHealthManager
{
    public ParticleSystem destructionParticles;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float baseMaxHealth = 200f;
    public GameObject damageNumberPrefab;
    public Canvas damageCanvas;
    private Vector3 offsetPosition;

    [HideInInspector] public bool hasMinorDamage = false;
    [HideInInspector] public bool hasMajorDamage = false;
    [HideInInspector] public bool isDestroyed = false;

    protected override void Awake()
    {
        base.Awake();
        damageNumberPrefab = Resources.Load<GameObject>("Prefabs/DamageNumbers");
        damageCanvas = GameObject.FindWithTag("StatCanvas").GetComponent<Canvas>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        destructionParticles = GetComponentInChildren<ParticleSystem>();
        animator.enabled = true;
    }

    protected override void Start()
    {
        base.Start();
        maxHealth = baseMaxHealth;
        currentHealth = baseMaxHealth;
        offsetPosition = transform.position + new Vector3(0.1f, 0.8f, 0);
    }

    public void TakeDamage(float damage, bool isCriticalHit = false)
    {
        if (isDestroyed) { return; }

        float modifiedDamage = PlayerCombat.Instance.ApplyDamageModifiers(damage, gameObject);
        float onHitDamage = PlayerCombat.Instance.ApplyOnHitModifiers(0f, gameObject);
        float totalDamage = modifiedDamage + onHitDamage;
        base.TakeDamage(totalDamage);

        ShowDamageNumber(totalDamage, isCriticalHit ? Color.yellow : Color.white, isCriticalHit ? 14f : 12f);
        if (onHitDamage != 0)
        {
            ShowDamageNumber(onHitDamage, Color.yellow, 10f, offsetPosition);
        }

        //TriggerShake();
        CheckHealthThreshold();
    }

    protected override void CheckHealthThreshold() { }

    public void TriggerDamageParticles(int particleCount = 3, float burstTime = 0.1f)
    {
        var emission = destructionParticles.emission;

        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(burstTime, particleCount) });

        destructionParticles.Play();
    }

    private void TriggerShake()
    {
        StartCoroutine(Shake(0.05f, 0.05f));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public void ShowDamageNumber(float damage, Color color, float size = 12, Vector3? customPosition = null)
    {
        Vector3 damagePosition = customPosition ?? transform.position + new Vector3(0, 1, 0);

        GameObject damageNumber = Instantiate(damageNumberPrefab, damagePosition, Quaternion.identity, damageCanvas.transform);

        DamageNumber damageNumberScript = damageNumber.GetComponent<DamageNumber>();
        if (damageNumberScript != null)
        {
            damageNumberScript.SetValue(damage);
            damageNumberScript.SetTextColor(color);
            damageNumberScript.SetTextSize(size);
        }
    }

    protected override void Die()
    {
        Obstacle obstacleScript = GetComponent<Obstacle>();
        if (obstacleScript != null)
        {
            Debug.LogWarning("Walkability set to false");
            obstacleScript.UpdateWalkability(transform.position, false);  // Reset nodes to walkable
        }
        Destroy(GetComponent<CircleCollider2D>());
        Destroy(GetComponent<BoxCollider2D>());
    }
}