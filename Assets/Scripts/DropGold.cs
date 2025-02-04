using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropGold : IOnDeathEffect
{
    private float dropChance;

    public DropGold(float dropChance)
    {
        this.dropChance = dropChance;
    }

    public void Execute(GameObject enemy)
    {
        if (Random.value <= dropChance)
        {
            ObjectPoolManager.Instance.GetFromPool("Gold", enemy.transform.position, Quaternion.identity);
        }
    }
}