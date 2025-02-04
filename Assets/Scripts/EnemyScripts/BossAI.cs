using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : EnemyAI
{
    [Header("Boss UI")]
    public BossUIManager bossUIManager;

    [Header("Abilities")]
    public ChargeAbility chargeAbility;
    public MeleeAbility meleeAbility;
    public BulletHellAbility bulletHellAbility;

    [Header("Effects")]
    [SerializeField] private GameObject chargeTrailPrefab;

    protected override void InitializeStatsAndAbilities()
    {
        Debug.LogWarning($"Initializing enemy {gameObject.name}");
        baseHealth = 100f;
        baseDamage = 50f;
        baseMoveSpeed = 3.5f;
        
        if (chargeAbility == null)
        {
            chargeAbility = gameObject.AddComponent<ChargeAbility>();
        }
        if (meleeAbility == null)
        {
            meleeAbility = gameObject.AddComponent<MeleeAbility>();
        }
        if (bulletHellAbility == null)
        {
            bulletHellAbility = gameObject.AddComponent<BulletHellAbility>();
        }


        if (chargeAbility != null)
        {
            abilityManager.AddAbility(chargeAbility);
        }
        if (meleeAbility != null)
        {
            abilityManager.AddAbility(meleeAbility);
        }
        if (bulletHellAbility != null)
        {
            abilityManager.AddAbility(bulletHellAbility);
        }

        chargeAbility.chargeAbilityData = new EnemyAbilityData
        {
            abilityName = "Charge",
            damage = currentDamage,
            cooldown = 8f,
            knockbackForce = 16f,
            speed = 22f,
            detectRadius = 10f,
            abilityVFXPrefab = chargeTrailPrefab
        };

        meleeAbility.meleeAbilityData = new EnemyAbilityData
        {
            abilityName = "Bite",
            damage = currentDamage,
            cooldown = 1f,
            knockbackForce = 10f,
            attackRange = 4f,
            areaOfEffect = 3f,
            detectRadius = 4f
        };

        bulletHellAbility.bulletHellAbilityData = new EnemyAbilityData
        {
            abilityName = "Bullet Hell",
            damage = currentDamage,
            cooldown = 10f,
            knockbackForce = 4f,
            detectRadius = 12f,
            abilityVFXPrefab = Resources.Load<GameObject>("Prefabs/EnemyProjectile")
        };

        EventManager.Instance.TriggerEvent("ObjectPoolUpdate", "EnemyProjectile", 200);
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "EnemyBulletImpactFX", 200);

        base.InitializeStatsAndAbilities();

        if (bossUIManager != null)
        {
            bossUIManager.Initialize(gameObject.name, baseHealth);
            bossUIManager.Show();
        }
    }
}