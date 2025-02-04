using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAI : EnemyAI
{
    [Header("Abilities")]
    public MeleeAbility meleeAbility;
    public SpreadShotAbility spreadShotAbility;

    protected override void InitializeStatsAndAbilities()
    {
        baseHealth = 100f;
        baseDamage = 10f;
        baseMoveSpeed = 4f;

        if (meleeAbility == null)
        {
            meleeAbility = gameObject.AddComponent<MeleeAbility>();
        }
        if (spreadShotAbility == null)
        {
            spreadShotAbility = gameObject.AddComponent<SpreadShotAbility>();
        }


        if (meleeAbility != null)
        {
            abilityManager.AddAbility(meleeAbility);
        }
        if (spreadShotAbility != null)
        {
            abilityManager.AddAbility(spreadShotAbility);
        }

        meleeAbility.meleeAbilityData = new EnemyAbilityData
        {
            abilityName = "Bite",
            damage = currentDamage,
            cooldown = 1f,
            knockbackForce = 5f,
            attackRange = 1.5f,
            areaOfEffect = 2f,
            detectRadius = 1.5f
        };

        spreadShotAbility.spreadShotAbilityData = new EnemyAbilityData
        {
            abilityName = "Shotgun",
            damage = currentDamage,
            cooldown = 4f,
            knockbackForce = 5f,
            detectRadius = 7f,
            abilityVFXPrefab = Resources.Load<GameObject>("Prefabs/EnemyProjectile")
        };

        base.InitializeStatsAndAbilities();

    }
}