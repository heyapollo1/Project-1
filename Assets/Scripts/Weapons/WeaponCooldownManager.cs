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
    }

    public void StartCooldown(string id, float cooldownDuration)
    {
        if (!cooldownTimers.ContainsKey(id) || cooldownTimers[id] <= 0)
        {
            cooldownTimers[id] = cooldownDuration;
            totalCooldowns[id] = cooldownDuration;
            StartCoroutine(CooldownCoroutine(id));
        }
    }

    private IEnumerator CooldownCoroutine(string id)
    {
        while (cooldownTimers[id] > 0)
        {
            cooldownTimers[id] -= Time.deltaTime;
            
            OnCooldownUpdated?.Invoke(id, cooldownTimers[id], totalCooldowns[id]);
            //Debug.Log($"{id} cooldown: {cooldownTimers[id]:F1}s remaining");
            yield return null;
        }
        cooldownTimers[id] = 0;
        totalCooldowns[id] = 0;
        EventManager.Instance.TriggerEvent("CooldownComplete", id);
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
