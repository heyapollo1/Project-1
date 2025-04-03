using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : WeaponBase
{
    [Header("Shotgun Settings")]
    public float projectileSpeed = 15f;
    public int bulletCount = 6;
    public float spreadAngle = 30f;
    public Transform firePoint;

    [HideInInspector] public GameObject bulletProjectile;
    private bool isFiring = false;

    public override void InitializeWeapon(bool isLeft)
    {
        Debug.Log($"Initializing Shotgun, {isLeft}.");
        EventManager.Instance.StartListening("CooldownComplete", CompleteCooldown);
        EventManager.Instance.StartListening("StatsChanged", UpdateWeaponStats);
        weaponUI = LoadoutUIManager.Instance.GetActiveSlotUI(isLeftHand);
        weaponKey = GenerateWeaponKey(isLeft);
        weaponData = weaponInstance.weaponData;
        weaponInstance.weaponData.enemyLayer = LayerMask.GetMask("Enemy");
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        isLeftHand = isLeft;
        isAutoAiming = false;
        
        SetBaseStats();
        UpdateWeaponStats();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("CooldownComplete", CompleteCooldown);
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
            FireShotgun(lockedAimDirection);
        }
        else
        {
            lockedAimDirection = playerController.aimDirection;
            FireShotgun(lockedAimDirection);
        }
            
        WeaponReset();
    }

    private void FireShotgun(Vector2 aimDirection)
    {
        if (aimDirection == Vector2.zero) return;
        float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float angleStep = spreadAngle / (bulletCount - 1);
        
        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = baseAngle - (spreadAngle / 2) + (angleStep * i);
            Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized;
            
            GameObject bullet = ObjectPoolManager.Instance.GetFromPool("Bullet", firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
                TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
                if (trail != null) trail.Clear();
                bulletScript.Initialize(this, playerAttributes.GetActiveStatusEffectTriggers());
            }
            else
            {
                Debug.LogError("Failed to spawn bullet from pool!");
            }
        }
        AudioManager.TriggerSound("Weapon_Fire", playerController.transform.position);
    }

    private void UpdateWeaponStats()
    {
        currentDamage = playerAttributes.GetStatValue(StatType.Damage, weaponInstance.baseDamage);
        currentRange = playerAttributes.GetStatValue(StatType.Range, weaponInstance.baseRange);
        currentCooldownRate = playerAttributes.GetStatValue(StatType.CooldownRate, weaponInstance.baseCooldownRate);
        currentKnockbackForce = playerAttributes.GetStatValue(StatType.KnockbackForce, weaponInstance.baseKnockbackForce);
        currentCriticalHitChance = playerAttributes.GetStatValue(StatType.CriticalHitChance, weaponInstance.baseCriticalHitChance);
        currentCriticalHitDamage = playerAttributes.GetStatValue(StatType.CriticalHitDamage, weaponInstance.baseCriticalHitDamage);

        weaponInstance.UpdateStatTags(currentDamage, currentCooldownRate, currentRange); //UI on tooltips
        Debug.Log($"Updating shotgun stats:DMG: {weaponInstance.baseDamage}, CDR: {weaponInstance.baseCooldownRate}, RNG:{weaponInstance.baseRange}");
    }
    
    public override void UpgradeWeapon()
    {
        UpdateWeaponStats();
    }

    public override void WeaponReset()
    {
        isFiring = false;
        StartCooldown(currentCooldownRate);
    }
}
