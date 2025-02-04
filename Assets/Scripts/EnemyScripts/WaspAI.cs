using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspAI : EnemyAI
{
    [Header("Abilities")]
    public MeleeAbility meleeAbility;
    public ShootAbility shootAbility;

    protected override void InitializeStatsAndAbilities()
    {
        baseHealth = 100f;
        baseDamage = 10f;
        baseMoveSpeed = 4f;

        minRangeToPlayer = 6f;
        effectiveRangeToPlayer = 4f;

        if (meleeAbility == null)
        {
            meleeAbility = gameObject.AddComponent<MeleeAbility>();
        }

        if (meleeAbility != null)
        {
            abilityManager.AddAbility(meleeAbility);
        }

        if (shootAbility == null)
        {
            shootAbility = gameObject.AddComponent<ShootAbility>();
        }

        if (shootAbility != null)
        {
            abilityManager.AddAbility(shootAbility);
        }

        shootAbility.shootAbilityData = new EnemyAbilityData
        {
            abilityName = "Spit",
            damage = currentDamage,
            cooldown = 5f,
            knockbackForce = 2f,
            detectRadius = 8f,
            abilityVFXPrefab = Resources.Load<GameObject>("Prefabs/EnemyProjectile")
        };

        meleeAbility.meleeAbilityData = new EnemyAbilityData
        {
            abilityName = "Bite",
            damage = currentDamage,
            cooldown = 1f,
            knockbackForce = 5f,
            attackRange = 1.5f,
            areaOfEffect = 2,
            detectRadius = 1.5f,
        };

        EventManager.Instance.TriggerEvent("ObjectPoolUpdate", "EnemyProjectile", 3);
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "EnemyBulletImpactFX", 3);

        base.InitializeStatsAndAbilities();

    }
}
