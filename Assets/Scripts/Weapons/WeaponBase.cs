using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponInstance weaponInstance;
    public WeaponData weaponData;
    public SpriteRenderer weaponSpriteRenderer;
    
    [HideInInspector] public EnemyDetector enemyDetector;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public AttributeManager playerAttributes;
    [HideInInspector] public Transform equippedLocation;
    [HideInInspector] public Vector2 lockedAimDirection;
    
    [HideInInspector] public List<TagType> classTags = new List<TagType>();
    
    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentCooldownRate;
    [HideInInspector] public float currentRange;
    [HideInInspector] public float currentKnockbackForce;
    [HideInInspector] public float currentCriticalHitChance;
    [HideInInspector] public float currentCriticalHitDamage;
    
    protected bool isOnCooldown = false;
    protected bool isDisabled = false;
    private float cooldownTimer = 0f;
    [HideInInspector] public bool isAutoAiming = false;

    public abstract void InitializeWeapon();
    public abstract bool CanUseWeapon();
    public abstract void UseWeapon();
    public abstract void UpgradeWeapon();
    public abstract void WeaponReset();
    
    public void OnDestroy()
    {
        EventManager.Instance.StopListening("CooldownComplete", CompleteCooldown);
    }
    
    public void StartCooldown(float cooldownDuration)
    {
        if (!isOnCooldown)
        {
            if (weaponInstance.assignedUI == null)
            {
                Debug.Log("weapon UI null");
            }
            isOnCooldown = true;
            WeaponCooldownManager.Instance.StartCooldown(weaponInstance.uniqueID, cooldownDuration);
            weaponInstance.assignedUI.StartCooldownUI(cooldownDuration);
        }
    }
    
    public void TryUseWeapon()
    {
        if (isDisabled || isOnCooldown) return;
        if (CanUseWeapon() && !UIManager.Instance.IsDraggingItem)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            UseWeapon();
        }
    }

    public void WeaponAiming(Vector2 aimDirection)
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        if (equippedLocation != null)
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
            float weaponDistance = 0.5f; // Adjust this based on how far the weapon should be from the hand
            transform.position = equippedLocation.position + (Vector3)(aimDirection.normalized * weaponDistance);
        }

        int gunDirectionIndex = playerController.GetDirectionIndex(angle);

        if (gunDirectionIndex >= 0 && gunDirectionIndex < weaponInstance.weaponData.weaponSprites.Length)
        {
            weaponSpriteRenderer.sprite = weaponInstance.weaponData.weaponSprites[gunDirectionIndex];
        }

        if (gunDirectionIndex == 0 || gunDirectionIndex == 1 || gunDirectionIndex == 7)
        {
            weaponSpriteRenderer.flipY = true;
        }
        else
        {
            weaponSpriteRenderer.flipY = false;
        }
        
        if (gunDirectionIndex == 5 || gunDirectionIndex == 6 || gunDirectionIndex == 7)
        {
            weaponSpriteRenderer.sortingLayerName = "Shadow";
        }
        else
        {
            weaponSpriteRenderer.sortingLayerName = "Default";
        }
    }

    public void ToggleAutoAim()
    {
        isAutoAiming = !isAutoAiming;
    }
    
    public void CompleteCooldown(string key)
    {
        if (weaponInstance.uniqueID != key) return;
        isOnCooldown = false;
    }
    
    public bool IsWeaponDisabled()
    {
        return isDisabled;
    }
    
    public bool isWeaponOnCooldown()
    {
        return isOnCooldown;
    }
    
    public void ToggleWeaponState(bool active)
    {
        isDisabled = active;
    }
    
}
