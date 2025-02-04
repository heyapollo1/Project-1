using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponTitle;
    public string weaponDescription;

    public float baseDamage;
    public float baseCooldownRate;
    public float baseRange;
    public float baseKnockbackForce;
    public float baseCriticalHitChance;
    public float baseCriticalHitDamage;
    public int price;

    public GameObject weaponPrefab;  // Prefab for the weapon instance
    public Sprite[] weaponSprites;
    public Sprite weaponIcon;
    //public SpriteRenderer weaponSpriteRenderer;
    /*public EnemyDetector enemyDetector;
    public PlayerController playerController;
    public PlayerStatManager playerStats;
    public Transform weaponHolder;*/
    public LayerMask enemyLayer;
}
