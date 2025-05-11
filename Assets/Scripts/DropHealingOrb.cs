using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropHealingOrb : IOnDeathEffect
{
    private float dropChance;

    public DropHealingOrb(float dropChance)
    {
        this.dropChance = dropChance;
    }

    public void Execute(GameObject enemy)
    {
        if (Random.value <= dropChance) // Random.value gives a float between 0 and 1
        {
            ObjectPoolManager.Instance.GetFromPool("HealingOrb", enemy.transform.position, Quaternion.identity);
            Debug.Log("Healing orb spawned at " + enemy.name + "'s position");
        }
        else
        {
            Debug.Log("No healing orb dropped from " + enemy.name);
        }
    }
}
