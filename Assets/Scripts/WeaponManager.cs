using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    private GameObject weaponHolder; // attach weapon here
    private List<WeaponBase> playerWeapons = new List<WeaponBase>(); // abilities player owns
    private WeaponData currentWeaponData;
    public WeaponBase equippedWeapon;

    private PlayerController playerController;
    private PlayerStatManager playerStats;
    private EnemyDetector enemyDetector;
    private bool isDisabled = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStatManager>();
        enemyDetector = GetComponent<EnemyDetector>();
        weaponHolder = GameObject.FindWithTag("WeaponHolder");
        EventManager.Instance.StartListening("ToggleAutoAim", ToggleAutoAim);
        //EquipWeapon(currentWeaponData);
    }

    void Update()
    {
        if (equippedWeapon == null && isDisabled && GameStateManager.Instance.CurrentState == GameState.Playing) return;
    }


    public void ApplyWeapon(WeaponData weaponData)
    {
        AcquireWeapon(weaponData);
        UnlockWeaponSpecificUpgrades(weaponData);
    }

    public void AcquireWeapon(WeaponData weaponData)
    {
        //EventManager.Instance.TriggerEvent("WeaponUnlocked", weaponData);
        if (currentWeaponData != null)
        {
            Debug.Log($"swapping weapon: {currentWeaponData}");
            SwapWeapon();
        }

        GameObject weaponInstance = Instantiate(weaponData.weaponPrefab, weaponHolder.transform);
        WeaponBase newWeapon = weaponInstance.GetComponent<WeaponBase>();

        currentWeaponData = weaponData;
        equippedWeapon = newWeapon;

        //newWeapon.firePoint = firePoint.position;
        newWeapon.weaponData = weaponData;
        newWeapon.weaponHolder = weaponHolder.transform;
        newWeapon.enemyDetector = enemyDetector;
        newWeapon.playerController = playerController;
        newWeapon.playerStats = playerStats;

        playerWeapons.Add(newWeapon);

        newWeapon.InitializeWeapon();

        Debug.Log($"Unlocked weapon: {currentWeaponData}");
    }

    public void UnlockWeaponSpecificUpgrades(WeaponData data)
    {
        foreach (var weapon in playerWeapons)
        {
            if (weapon == null || weapon.weaponData == null)
            {
                Debug.LogWarning("Found a null entry in playerAbilities list!");
                continue;
            }

            if (weapon.weaponData == data)
            {
                Debug.Log($"Adding specific upgrades to: {data.weaponTitle}");
                weapon.AddWeaponSpecificUpgrades();
                return;
            }
        }
        Debug.LogWarning($"No matching ability found for: {data.weaponTitle}");
    }

    public void SwapWeapon()
    {
        foreach (Transform child in weaponHolder.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public bool HasWeapon(WeaponData weaponData)
    {
        foreach (WeaponBase weapon in playerWeapons)
        {
            if (weapon.weaponData = weaponData)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public void ToggleAutoAim()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.ToggleAutoAim();
        }
    }

    public bool IsAutoAiming()
    {
        return equippedWeapon.isAutoAiming;
    }

    public Vector2 GetLockedAimDirection()
    {
        return equippedWeapon != null ? equippedWeapon.lockedAimDirection : Vector2.zero;
    }

    public void DisableWeapon()
    {
        isDisabled = true;
    }

    public void EnableWeapon()
    {
        isDisabled = false;
    }
}