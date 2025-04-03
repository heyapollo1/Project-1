using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCooldownManager : MonoBehaviour
{
    public static WeaponCooldownManager Instance { get; private set; }

    private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>();
    private Dictionary<string, float> totalCooldowns = new Dictionary<string, float>();
    
    public event System.Action<string, float, float> OnCooldownUpdated; // dont use event listener, direct delegate call.
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
       //ApplyWeaponCooldownManager();
        //Debug.LogError("ACTIVATING WEAPON COOLDOWN MANAGER");
    }

    public void StartCooldown(string weaponKey, float cooldownDuration)
    {
        if (!cooldownTimers.ContainsKey(weaponKey) || cooldownTimers[weaponKey] <= 0)
        {
            cooldownTimers[weaponKey] = cooldownDuration;
            totalCooldowns[weaponKey] = cooldownDuration;
            StartCoroutine(CooldownCoroutine(weaponKey));
        }
    }

    private IEnumerator CooldownCoroutine(string weaponKey)
    {
        while (cooldownTimers[weaponKey] > 0)
        {
            cooldownTimers[weaponKey] -= Time.deltaTime;
            OnCooldownUpdated?.Invoke(weaponKey, cooldownTimers[weaponKey], totalCooldowns[weaponKey]);

            yield return null;
        }
        cooldownTimers[weaponKey] = 0;
        totalCooldowns[weaponKey] = 0;
        EventManager.Instance.TriggerEvent("CooldownComplete", weaponKey);
    }
    
    public float GetTotalCooldownTime(string weaponKey)
    {
        return totalCooldowns.TryGetValue(weaponKey, out float totalTime) ? totalTime : 1; // Avoid division by zero
    }
    
    public float GetCooldownTime(string weaponKey)
    {
        return cooldownTimers.ContainsKey(weaponKey) ? cooldownTimers[weaponKey] : 0;
    }

    public bool IsWeaponOnCooldown(string weaponKey)
    {
        return cooldownTimers.ContainsKey(weaponKey) && cooldownTimers[weaponKey] > 0;
    }
}
