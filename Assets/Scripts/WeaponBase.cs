using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponData weaponData;
    public SpriteRenderer weaponSpriteRenderer;
    //public Sprite weaponIcon;
    [HideInInspector] public EnemyDetector enemyDetector;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public PlayerStatManager playerStats;
    [HideInInspector] public Transform weaponHolder;
    [HideInInspector]public Vector2 lockedAimDirection;
    //[HideInInspector]public Vector2 firePoint;

    protected bool isOnCooldown = false;
    protected bool isDisabled = false;
    private float cooldownTimer = 0f;
    [HideInInspector] public bool isAutoAiming = true;
    [HideInInspector] public bool eligibleTarget = false;

    public abstract void InitializeWeapon();
    public abstract bool CanUseWeapon();
    public abstract void UseWeapon();
    public abstract void WeaponReset();
    public abstract void AddWeaponSpecificUpgrades();

    public void StartCooldown(float cooldownDuration)
    {
        if (!isOnCooldown)
        {
            StartCoroutine(CooldownCoroutine(cooldownDuration));
        }
    }

    private void Update()
    {
        if (isDisabled) return;

        if (CanUseWeapon())
        {
            UseWeapon();
        }
    }

    private IEnumerator CooldownCoroutine(float cooldownDuration)
    {
        isOnCooldown = true;

        while (cooldownTimer < cooldownDuration)
        {
            cooldownTimer += Time.deltaTime; // Increment timer each frame
            yield return null; // Wait until the next frame
        }

        cooldownTimer = 0;
        isOnCooldown = false;
    }

    public void GunAiming(Vector2 aimDirection)
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        if (weaponHolder != null)
        {
            weaponHolder.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        int gunDirectionIndex = playerController.GetDirectionIndex(angle);

        if (gunDirectionIndex >= 0 && gunDirectionIndex < weaponData.weaponSprites.Length)
        {
            weaponSpriteRenderer.sprite = weaponData.weaponSprites[gunDirectionIndex];
        }

        if (gunDirectionIndex == 0 || gunDirectionIndex == 1 || gunDirectionIndex == 7)
        {
            weaponSpriteRenderer.flipY = true;
        }
        else
        {
            weaponSpriteRenderer.flipY = false;
        }
    }

    public void ToggleAutoAim()
    {
        isAutoAiming = !isAutoAiming;
    }

    public bool IsAbilityReady()
    {
        return !isOnCooldown && !isDisabled;
    }

    public void DisableAbility()
    {
        isDisabled = true;
    }

    public void EnableAbility()
    {
        isDisabled = false;
    }
}
