using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UziWeapon : WeaponBase
{
    [Header("Uzi Settings")]
    public float projectileSpeed = 15f;
    public float projectileSpread = 10f;
    public Transform firePoint;
    
    private GameObject bulletProjectile;
    private bool isFiring = false;

    public override void InitializeWeapon(bool isLeftHand)
    {
        Debug.Log("Initializing Uzi");
        EventManager.Instance.StartListening("CooldownComplete", CompleteCooldown);
        this.isLeftHand = isLeftHand;
        weaponKey = GenerateWeaponKey(isLeftHand);
        weaponUI = LoadoutUIManager.Instance.GetActiveSlotUI(isLeftHand);
        weaponData = weaponInstance.weaponData;
        weaponInstance.weaponData.enemyLayer = LayerMask.GetMask("Enemy");
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        isAutoAiming = false;
        
        if (weaponSpriteRenderer == null)
        {
            Debug.LogError("no spriterenderer");
        }
        
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
        
        GameObject bullet = ObjectPoolManager.Instance.GetFromPool("Bullet", firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
            if (bulletScript.trail != null) bulletScript.trail.Clear();
            bulletScript.Initialize(this, playerAttributes.GetActiveStatusEffectTriggers());
        }
    }

    private void UpdateWeaponStats()
    {
        currentDamage = playerAttributes.GetStatValue(StatType.Damage, weaponInstance.baseDamage);
        currentRange = playerAttributes.GetStatValue(StatType.Range, weaponInstance.baseRange);
        currentCooldownRate = playerAttributes.GetStatValue(StatType.CooldownRate, weaponInstance.baseCooldownRate);
        currentCriticalHitChance = playerAttributes.GetStatValue(StatType.CriticalHitChance, weaponInstance.baseCriticalHitChance);
        currentCriticalHitDamage = playerAttributes.GetStatValue(StatType.CriticalHitDamage, weaponInstance.baseCriticalHitDamage);

        weaponInstance.UpdateStatTags(currentDamage, currentCooldownRate, currentRange); //UI
        Debug.Log($"Updating pistol stats:DMG: {weaponInstance.baseDamage}, CDR: {weaponInstance.baseCooldownRate}, RNG:{weaponInstance.baseRange}");
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
