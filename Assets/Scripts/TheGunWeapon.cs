using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheGunWeapon : WeaponBase
{
    //public int baseBurstCount = 4;
    public float projectileSpeed = 15f;
    public float burstDelay = 0.15f;
    public float projectileSpread = 10f;

    public Transform firePoint;
    //public SpriteRenderer weaponSpriteRenderer;
    private GameObject bulletProjectile;
    //private Transform firePoint;

    private float currentDamage;
    private float currentCooldownRate;
    private float currentRange;
    private float currentKnockbackForce;
    private float currentCriticalHitDamage;

    private bool IsFiring = false;

    public override void InitializeWeapon()
    {
        Debug.Log("Initializing Default Weapon...");

        weaponData.enemyLayer = LayerMask.GetMask("Enemy");
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        if (weaponSpriteRenderer == null)
        {
            Debug.LogError("no spriterenderer");
        }
        EventManager.Instance.StartListening("StatsChanged", UpdateWeaponStats);

        SetBaseStats();
        UpdateWeaponStats();
        AddWeaponSpecificUpgrades();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("StatsChanged", UpdateWeaponStats);
    }

    private void SetBaseStats()
    {
        currentDamage = weaponData.baseDamage;
        currentCooldownRate = weaponData.baseCooldownRate;
        currentRange = weaponData.baseRange;
        currentKnockbackForce = weaponData.baseKnockbackForce;
        //currentBurstCount = baseBurstCount;
        currentCriticalHitDamage = PlayerStatManager.Instance.CalculateCriticalHitDamage(currentDamage);
        Debug.Log("Crit damage = " + currentCriticalHitDamage + "and base:" + weaponData.baseCriticalHitDamage);
    }

    public override bool CanUseWeapon()
    {
        return !isOnCooldown && (isAutoAiming || Input.GetMouseButton(0));
    }

    public override void UseWeapon()
    {
        if (CanUseWeapon() && !IsFiring)
        {
            IsFiring = true;
            if (isAutoAiming)
            {
                GameObject nearestEnemy = enemyDetector.FindNearestEnemyInRange(currentRange);

                if (nearestEnemy == null) return;

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

    /*private IEnumerator BurstFire()
    {
        for (int i = 0; i < currentBurstCount; i++)
        {
            // Use the locked aim direction instead of recalculating
            GunAiming(lockedAimDirection);

            // Fire the weapon in the locked direction
            FireWeapon(lockedAimDirection);

            // Wait before firing the next projectile
            yield return new WaitForSeconds(burstDelay);
        }

        AbilityReset();
    }*/

    private void FireWeapon(Vector2 aimDirection)
    {
        /*if (aimDirection == Vector2.zero)
        {
            WeaponReset();
            return;
        }*/
        AudioManager.TriggerSound("Weapon_Fire", playerController.transform.position);
        float baseAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Calculate random spread offset
        float spreadOffset = Random.Range(-projectileSpread, projectileSpread);
        float adjustedAngle = baseAngle + spreadOffset;

        Vector2 adjustedDirection = new Vector2(Mathf.Cos(adjustedAngle * Mathf.Deg2Rad), Mathf.Sin(adjustedAngle * Mathf.Deg2Rad));

        bulletProjectile = ObjectPoolManager.Instance.GetFromPool("Bullet", firePoint.position, Quaternion.identity);

        TrailRenderer trail = bulletProjectile.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
        }

        bulletProjectile.GetComponent<Rigidbody2D>().AddForce(adjustedDirection * projectileSpeed, ForceMode2D.Impulse);

        if (!playerStats.IsCriticalHit())
        {
            bulletProjectile.GetComponent<Bullet>().SetStats(currentDamage, currentKnockbackForce);
        }
        else
        {
            bulletProjectile.GetComponent<Bullet>().SetStats(currentCriticalHitDamage, currentKnockbackForce, true);
        }
    }

    private void UpdateWeaponStats()
    {
        currentDamage = playerStats.GetStatValue(StatType.Damage, weaponData.baseDamage);
        currentRange = playerStats.GetStatValue(StatType.Range, weaponData.baseRange);
        currentCooldownRate = playerStats.GetStatValue(StatType.CooldownRate, weaponData.baseCooldownRate);
        currentCriticalHitDamage = PlayerStatManager.Instance.CalculateCriticalHitDamage(currentDamage);
        //currentBurstCount = weaponData.playerStats.GetStatValue(StatType.BurstFire, baseBurstCount);
    }

    public override void WeaponReset()
    {
        IsFiring = false;
        StartCooldown(currentCooldownRate);
    }

    public override void AddWeaponSpecificUpgrades()
    {
        /*Sprite burstUpgradeIcon = Resources.Load<Sprite>("Icons/BurstFireIcon");

        BurstFireUpgrade burstFireUpgrade = new BurstFireUpgrade(burstUpgradeIcon);
        UpgradeDatabase.Instance.AddNewUpgrade(burstFireUpgrade);

        Debug.Log("Weapon-specific burst upgrade added to the upgrade pool.");*/
    }
}
