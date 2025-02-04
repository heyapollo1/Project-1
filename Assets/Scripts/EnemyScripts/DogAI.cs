using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAI : EnemyAI
{
    [Header("Abilities")]
    public MeleeAbility meleeAbility;

    protected override void InitializeStatsAndAbilities()
    {
        baseHealth = 100f;
        baseDamage = 10f;
        baseMoveSpeed = 3.5f;

        if (meleeAbility == null)
        {
            meleeAbility = gameObject.AddComponent<MeleeAbility>();
        }

        if (meleeAbility != null)
        {
            abilityManager.AddAbility(meleeAbility);
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

        base.InitializeStatsAndAbilities();
    }
}
