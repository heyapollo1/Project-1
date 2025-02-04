
using UnityEngine;

[System.Serializable]
public class EnemyAbilityData
{
    public string abilityName;
    public float damage;
    public float cooldown;
    public float knockbackForce;
    public float speed;
    public float attackRange;
    public float areaOfEffect;
    public float detectRadius;

    public GameObject abilityVFXPrefab;
}
