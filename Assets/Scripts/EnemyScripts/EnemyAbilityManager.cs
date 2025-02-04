using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityManager : MonoBehaviour
{
    private List<EnemyAbilityBase> abilities = new List<EnemyAbilityBase>();

    public void AddAbility(EnemyAbilityBase ability)
    {
        abilities.Add(ability);
    }

    public EnemyAbilityBase GetAvailableAbility(EnemyAI enemy)
    {
        foreach (var ability in abilities)
        {
           // Debug.Log("Using Ability Check");
            if (ability.CanUseAbility(enemy))
            {
                return ability;
            }
        }
        return null; 
    }

    public void TriggerAbility(EnemyAbilityBase ability, EnemyAI enemy)
    {
        if (ability != null)
        {
            ability.UseAbility(enemy);
        }
    }

    public void LogAbilities()
    {
        foreach (var ability in abilities)
        {
            Debug.Log("Ability found: " + ability.GetType().Name);
        }
    }
}