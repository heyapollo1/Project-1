
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability Data")]
public class PlayerAbilityData : ScriptableObject
{
    public string abilityTitle;
    public string abilityDescription;
    public Sprite abilityIcon;

    public float baseDamage;
    public float baseCooldownRate;
    public float baseRange;
    public float baseKnockbackForce;

    public GameObject abilityPrefab;
    public Transform player;
    public PlayerStatManager playerStats;
    public EnemyDetector enemyDetector;
    public Slider cooldownSlider;
    public LayerMask enemyLayer;
}