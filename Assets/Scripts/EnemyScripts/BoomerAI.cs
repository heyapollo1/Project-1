using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerAI : EnemyAI
{
    [Header("Abilities")]
    public KamikazeAbility kamikazeAbility;

    protected override void InitializeStatsAndAbilities()
    {
        baseHealth = 200f;
        baseDamage = 100f;
        baseMoveSpeed = 6f;

        if (kamikazeAbility == null)
        {
            kamikazeAbility = gameObject.AddComponent<KamikazeAbility>();
        }

        if (kamikazeAbility != null)
        {
            abilityManager.AddAbility(kamikazeAbility);
        }

        kamikazeAbility.kamikazeAbilityData = new EnemyAbilityData
        {
            abilityName = "Kamikaze",
            damage = currentDamage,
            cooldown = 8f,
            knockbackForce = 18f,
            areaOfEffect = 4.5f,
            detectRadius = 3.5f,
            abilityVFXPrefab = Resources.Load<GameObject>("Prefabs/KamikazeExplosion")
        };

        base.InitializeStatsAndAbilities();

    }
}
