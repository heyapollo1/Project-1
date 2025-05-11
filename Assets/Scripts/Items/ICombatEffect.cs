using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatEffect
{
    void OnHit(CombatContext context);
    void OnHurt(float currentHealth, float maxHealth);
    void OnEnemyKilled(GameObject enemy, AttributeManager playerStats);
    List<StatModifier> ModifyStats(CombatContext context)
    {
        return null;
    }
}

public struct CombatContext
{
    public float damageDealt;
    public GameObject source;
    public GameObject target;
    public List<TagType> sourceTags;
    public Dictionary<StatType, float> statusTriggers;
}