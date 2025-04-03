
/*using UnityEngine;
using System.Collections;
//using System;

public class WeaponScript : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float baseCooldownRate = 0.5f;
    public float baseCriticalHitDamage = 0f;  // Default critical hit multiplier for this weapon
    public float baseDamage = 10f;
    public float baseCriticalHitChance = 0f;

    public int maxBullets = 30; // Total bullets in the magazine
    public int currentBullets;  // Current bullets in the magazine
    public float reloadTime = 2f; // Time it takes to reload
    public ReloadBar reloadBar;
    public Sprite[] gunSprites; // Array for gun sprites
    public SpriteRenderer gunSpriteRenderer;
    public Transform gunTransform;
    //public Transform player;

    private bool isReloading = false;
    private float timer;
    private bool canFire = true;

    public UIManager uiManager; // Reference to a UIManager script that handles UI updates

    private PlayerStats playerStats;
    private float currentDamage;
    private float currentCriticalHitDamage;
    private float currentCooldownRate;
    private float currentCriticalHitChance = 0f;


    void Start()
    {
        playerStats = PlayerStats.Instance;

        // Initialize the current damage and attack speed from PlayerStats
        currentDamage = baseDamage;
        float critBonus = playerStats.GetStatValue(StatType.CriticalHitDamage, 0f);  // Get percentage bonus from stats
        currentCriticalHitDamage = currentDamage * (2 + (critBonus / 100));
        currentCooldownRate = baseCooldownRate;

        playerStats.OnStatsChanged += UpdateWeaponStats;

        UpdateWeaponStats();

        //playerStats.OnDamageChanged += UpdateWeaponStats;
        //playerStats.OnAttackSpeedChanged += UpdateWeaponStats;

        reloadBar = FindObjectOfType<ReloadBar>();
        currentBullets = maxBullets;
        if (uiManager != null)
        {
            uiManager.UpdateBulletCount(currentBullets, maxBullets);
        }
    }

    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > currentCooldownRate)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire && currentBullets > 0)
        {
            Fire();
            canFire = false;
        }
        else if ((Input.GetKeyDown(KeyCode.R) && currentBullets < maxBullets) || (currentBullets <= 0 && !isReloading))
        {
            StartCoroutine(Reload());
        }
    }

    // The GunAiming function to handle gun rotation and sprite change
    public void GunAiming(Vector2 aimDirection)
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        // Use the same direction index for the gun sprite
        int gunDirectionIndex = GetDirectionIndex(angle);

        // Ensure the index is within bounds of the array
        if (gunDirectionIndex >= 0 && gunDirectionIndex < gunSprites.Length)
        {
            // Switch gun sprite based on the index
            gunSpriteRenderer.sprite = gunSprites[gunDirectionIndex];
        }

        // Flip the gun sprite if the mouse is on the left side of the player
        gunSpriteRenderer.flipY = (aimDirection.x < 0); // Flip if aiming left
    }

    int GetDirectionIndex(float angle)
    {
        // Map angles to direction indices
        if (angle >= -22.5f && angle < 22.5f)
            return 1; // Right
        if (angle >= 22.5f && angle < 67.5f)
            return 2; // Up-Right
        if (angle >= 67.5f && angle < 112.5f)
            return 3; // Up
        if (angle >= 112.5f && angle < 157.5f)
            return 5; // Up-Left
        if (angle >= -67.5f && angle < -22.5f)
            return 4; // Down-Right
        if (angle >= -112.5f && angle < -67.5f)
            return 6; // Down
        if (angle >= -157.5f && angle < -112.5f)
            return 7; // Down-Left

        return 0; // Left
    }

    void Fire()
    {
        CinemachineShake.instance.ShakeCamera(0.6f, 0.13f);
        GameObject projectile = Instantiate(bullet, firePoint.position, firePoint.rotation);
        projectile.GetComponent<Rigidbody2D>().AddForce(firePoint.right * projectileSpeed, ForceMode2D.Impulse);

        if (!playerStats.IsCriticalHit(currentCriticalHitChance))
        {
            projectile.GetComponent<Bullet>().SetDamage(currentDamage);
        }
        else
        {
            projectile.GetComponent<Bullet>().SetDamage(currentCriticalHitDamage);
            Debug.Log("Critical hit!");
        }

        currentBullets--;

        if (uiManager != null)
        {
            uiManager.UpdateBulletCount(currentBullets, maxBullets);
        }

        if (currentBullets <= 0)
        {
            StartCoroutine(Reload());
        }

        canFire = false;
    }

    // Handle when the attack speed stat changes
    private void UpdateWeaponStats()
    {
        currentDamage = playerStats.GetStatValue(StatType.Damage, baseDamage);
        currentCooldownRate = playerStats.GetStatValue(StatType.CooldownRate, baseCooldownRate);
        currentCriticalHitDamage = playerStats.GetStatValue(StatType.CriticalHitDamage, baseCriticalHitDamage);
    }

    IEnumerator Reload()
    {
        
        if (reloadBar != null)
        {
           reloadBar.StartReloading();
        }

        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        CinemachineShake.instance.ShakeCamera(1.1f, 0.15f);
        currentBullets = maxBullets;
        isReloading = false;
        // Play reload animation or sound if necessary

        if (uiManager != null)
        {
            uiManager.UpdateBulletCount(currentBullets, maxBullets);
        }
        canFire = true;
    }

    IEnumerator PlayMuzzleFlash()
    {
        isMuzzleFlashPlaying = true;
        // Activate and play animation
        muzzleFlash.gameObject.SetActive(true);

        muzzleFlash.SetTrigger("Fire");

        // Wait for the duration of the animation
        yield return new WaitForSeconds(0.3f); // Match this with the animation length

        // Deactivate after animation finishes
        muzzleFlash.gameObject.SetActive(false);

        isMuzzleFlashPlaying = false;
    }
}*/
