using System.Collections;
using UnityEngine;

public abstract class PlayerAbilityBase : MonoBehaviour
{
    public PlayerAbilityData abilityData;

    protected bool isOnCooldown = false;
    protected bool isDisabled = false;
    private float cooldownTimer = 0f;

    public abstract void Initialize();
    public abstract bool CanUseAbility();
    public abstract void UseAbility();
    public abstract void AbilityReset();
    public abstract void AddAbilitySpecificUpgrades();

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

        if (CanUseAbility())
        {
            UseAbility();
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
