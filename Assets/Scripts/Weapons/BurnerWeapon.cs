using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnerWeapon : WeaponBase
{
    private static readonly Dictionary<Rarity, float> GetBurnChanceMultiplier = new()
    {
        { Rarity.Common, 50f },
        { Rarity.Rare, 60f },
        { Rarity.Epic, 70f },
        { Rarity.Legendary, 80f }
    };
    
    [Header("Burner Settings")]
    public float projectileSpeed = 15f;
    public float projectileSpread = 10f;
    public float bonusBurnChance;
    private float currentBurnChance = 0f;
    public Transform firePoint;
    
    private GameObject bulletProjectile;
    //private float currentBurnChance;
    private Dictionary<StatType, float> statusEffectTriggers = new();
    private bool isFiring = false;

    public override void InitializeWeapon(bool isLeftHand)
    {
        Debug.Log("Initializing Burner");
        EventManager.Instance.StartListening("CooldownComplete", CompleteCooldown);
        this.isLeftHand = isLeftHand;
        weaponKey = GenerateWeaponKey(isLeftHand);
        weaponUI = LoadoutUIManager.Instance.GetActiveSlotUI(isLeftHand);
        weaponData = weaponInstance.weaponData;
        weaponInstance.weaponData.enemyLayer = LayerMask.GetMask("Enemy");
        bonusBurnChance = GetBurnChanceMultiplier[weaponInstance.rarity];
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        isAutoAiming = false;
        if (weaponSpriteRenderer == null)
        {
            Debug.LogError("no spriterenderer");
        }
        
        EventManager.Instance.TriggerEvent("ObjectPoolUpdate", "BurnerProjectile", 10);
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "BurnerImpactFX", 10);
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "BurningFX", 10);
        EventManager.Instance.StartListening("StatsChanged", UpdateWeaponStats);

        SetBaseStats();
        UpdateWeaponStats();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("StatsChanged", UpdateWeaponStats);
    }

    private void SetBaseStats()
    {
        currentDamage = weaponInstance.baseDamage;
        currentCooldownRate = weaponInstance.baseCooldownRate;
        currentRange = weaponInstance.baseRange;
        currentKnockbackForce = weaponInstance.baseKnockbackForce;
        currentCriticalHitChance = weaponInstance.baseCriticalHitDamage;
        currentCriticalHitDamage = weaponInstance.baseCriticalHitDamage;
        currentBurnChance = playerAttributes.baseBurnChance + bonusBurnChance;
        //playerAttributes.currentBleedChance;
    }

    public override bool CanUseWeapon()
    {
        if (isOnCooldown || isFiring) return false; 
        if (isLeftHand)
        {
            return Input.GetMouseButton(0);
        }
        return Input.GetMouseButton(1);
    }

    public override void UseWeapon()
    {
        if (CanUseWeapon() && !isFiring)
        {
            Debug.Log("Using Gun");
            isFiring = true;
            if (isAutoAiming)
            {
                GameObject nearestEnemy = enemyDetector.FindNearestEnemyInRange(currentRange);
                
                if (nearestEnemy == null)
                {
                    isFiring = false;
                    return;
                }

                Vector2 enemyDirection = (nearestEnemy.transform.position - playerController.transform.position).normalized;
                lockedAimDirection = enemyDirection;
                FireWeapon(lockedAimDirection);
            }
            else
            {
                lockedAimDirection = playerController.aimDirection;
                FireWeapon(lockedAimDirection);
            }
            WeaponReset();
        }
    }

    private void FireWeapon(Vector2 aimDirection)
    {
        AudioManager.TriggerSound("Weapon_Fire", playerController.transform.position);
        float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Calculate random spread offset
        float spreadOffset = Random.Range(-projectileSpread, projectileSpread);
        float adjustedAngle = baseAngle + spreadOffset;

        Vector2 direction = new Vector3(Mathf.Cos(adjustedAngle * Mathf.Deg2Rad), Mathf.Sin(adjustedAngle * Mathf.Deg2Rad), 0).normalized;
        
        GameObject projectile = ObjectPoolManager.Instance.GetFromPool("BurnerProjectile", firePoint.position, Quaternion.identity);
        BurnerProjectile bulletScript = projectile.GetComponent<BurnerProjectile>();
        statusEffectTriggers = AttributeManager.Instance.GetActiveStatusEffectTriggers();
        statusEffectTriggers[StatType.BurnChance] = currentBurnChance;
        if (projectile != null)
        {
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
            if (bulletScript.trail != null) bulletScript.trail.Clear();
            bulletScript.Initialize(this, statusEffectTriggers);
        }
    }

    private void UpdateWeaponStats()
    {
        currentDamage = playerAttributes.GetStatValue(StatType.Damage, weaponInstance.baseDamage);
        currentRange = playerAttributes.GetStatValue(StatType.Range, weaponInstance.baseRange);
        currentCooldownRate = playerAttributes.GetStatValue(StatType.CooldownRate, weaponInstance.baseCooldownRate);
        currentKnockbackForce = playerAttributes.GetStatValue(StatType.KnockbackForce, weaponInstance.baseKnockbackForce);
        currentCriticalHitChance = playerAttributes.GetStatValue(StatType.CriticalHitChance, weaponInstance.baseCriticalHitChance);
        currentCriticalHitDamage = playerAttributes.GetStatValue(StatType.CriticalHitDamage, weaponInstance.baseCriticalHitDamage);
        currentBurnChance = playerAttributes.GetStatValue(StatType.BurnChance, playerAttributes.baseBurnChance) + bonusBurnChance; 
        
        weaponInstance.UpdateStatTags(currentDamage, currentCooldownRate, currentRange); 
        Debug.Log($"Updating pistol stats:DMG: {weaponInstance.baseDamage}, CDR: {weaponInstance.baseCooldownRate}, Burn baby:{currentBurnChance}");
    }
    
    public override void UpgradeWeapon()
    {
        bonusBurnChance = GetBurnChanceMultiplier[weaponInstance.rarity];
        Debug.Log($"burn chance upgraded: {bonusBurnChance}");
        UpdateWeaponStats();
    }

    public override void WeaponReset()
    {
        isFiring = false;
        StartCooldown(currentCooldownRate);
    }
}

