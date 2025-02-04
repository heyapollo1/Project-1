using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAI : EnemyAI
{
    [Header("Abilities")]
    public ChargeAbility chargeAbility;
    public MeleeAbility meleeAbility;

    [Header("Effects")]
    [SerializeField] private GameObject chargeTrailPrefab;

    protected override void InitializeStatsAndAbilities()
    {
        baseHealth = 200f;
        baseDamage = 20f;
        baseMoveSpeed = 3.5f;
        
        if (chargeAbility == null)
        {
            chargeAbility = gameObject.AddComponent<ChargeAbility>();
        }
        if (meleeAbility == null)
        {
            meleeAbility = gameObject.AddComponent<MeleeAbility>();
        }


        if (chargeAbility != null)
        {
            abilityManager.AddAbility(chargeAbility);
            //chargeAbility = gameObject.AddComponent<ChargeAbility>();
        }

        if (meleeAbility != null)
        {
            abilityManager.AddAbility(meleeAbility);
            //meleeAbility = gameObject.AddComponent<MeleeAbility>();
        }

        chargeAbility.chargeAbilityData = new EnemyAbilityData
        {
            abilityName = "Charge",
            damage = currentDamage,
            cooldown = 8f,
            knockbackForce = 8f,
            speed = 18f,
            detectRadius = 6f,
            abilityVFXPrefab = chargeTrailPrefab
        };

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