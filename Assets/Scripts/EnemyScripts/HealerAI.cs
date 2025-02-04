using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAI : EnemyAI
{
    [Header("Abilities")]
    public MeleeAbility meleeAbility;
    public HealAbility healAbility;

    protected override void InitializeStatsAndAbilities()
    {
        baseHealth = 1000f;
        baseDamage = 20f;
        baseMoveSpeed = 2.5f;

        if (meleeAbility == null)
        {
            meleeAbility = gameObject.AddComponent<MeleeAbility>();
        }

        if (healAbility == null)
        {
            healAbility = gameObject.AddComponent<HealAbility>();
        }


        if (meleeAbility != null)
        {
            abilityManager.AddAbility(meleeAbility);
        }

        if (healAbility != null)
        {
            abilityManager.AddAbility(healAbility);
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

        healAbility.healAbilityData = new EnemyAbilityData
        {
            abilityName = "Healing",
            damage = currentDamage * 10,
            cooldown = 6f,
            detectRadius = 6f
        };

        base.InitializeStatsAndAbilities();
    }
}
