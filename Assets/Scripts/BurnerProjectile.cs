using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnerProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    //public SpriteRenderer spriteRend;
    //public GameObject impactEffect;
    //public Sprite muzzleFlash;
    public TrailRenderer trail;
    public float destroyTime = 2f;  
    //private Sprite defaultSprite;

    private WeaponInstance weaponInstance;
    private WeaponBase sourceWeapon;
    private float damageAmount;  
    private float knockBackAmount;
    private float criticalHitChance;  
    private float criticalHitDamage;
    private float burnChance;
    
    private bool hasHit = false; 
    private Dictionary<StatType, float> statusEffectTriggers = new();
    
    public void Initialize(WeaponBase sourceWeapon, Dictionary<StatType, float> statusEffectTriggers)
    {
        hasHit = false;
        if (trail != null) trail.Clear();
        this.sourceWeapon = sourceWeapon;
        damageAmount = sourceWeapon.currentDamage;
        knockBackAmount = sourceWeapon.currentKnockbackForce;
        criticalHitChance = sourceWeapon.currentCriticalHitChance;
        criticalHitDamage = sourceWeapon.currentCriticalHitDamage;
        //burnChance = currentBurnChance;

        this.statusEffectTriggers = statusEffectTriggers;
        //statusEffectTriggers.
        //defaultSprite = spriteRend.sprite;
        Debug.Log($"Burner Projectile {sourceWeapon}, {damageAmount}, {knockBackAmount}, {criticalHitChance}, {criticalHitDamage}");
        
        StartCoroutine(ReturnToPoolAfterTime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        if (collision.TryGetComponent(out EnemyHealthManager damageable))
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            CombatContext context = new CombatContext {damageDealt = damageAmount, source = gameObject, target = collision.gameObject,
                sourceTags = sourceWeapon.classTags, statusTriggers = statusEffectTriggers};
            
            List<StatModifier> statMods = PlayerCombat.Instance.GetModifiedStats(context); // modify stats based on accrued player combat bonuses
            if (statMods.Count > 0) ApplyStatMods(statMods);
            bool isCriticalHit = AttributeManager.Instance.IsCriticalHit(criticalHitChance);
            float finalDamage = isCriticalHit ? damageAmount : AttributeManager.Instance.CalculateCriticalHitDamage(damageAmount, criticalHitDamage);

            PlayerCombat.Instance.HandleOnHit(context);
            damageable.TakeDamage(Mathf.Ceil(finalDamage), knockbackDirection, knockBackAmount, DamageSource.Player, isCriticalHit);
            
            foreach (var trigger in statusEffectTriggers)
            {
                Debug.Log($"Status Effect: {trigger.Key}, {trigger.Value}");
                if (AttributeManager.Instance.ShouldTrigger(trigger.Value)) StatusEffectManager.Instance.TriggerStatusEffect(collision.gameObject, trigger.Key, finalDamage);
            }
            Impact();
        }
        else if (collision.gameObject.CompareTag("InvincibleObstacle"))
        {
            Impact();
        }
    }

    private void Impact()
    {
        hasHit = true;
        rb.velocity = Vector2.zero;
        StopAllCoroutines();
        AudioManager.TriggerSound("Weapon_Impact", transform.position);
        FXManager.Instance.PlayFX("BurnerImpactFX", transform.position);
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }

    private void ApplyStatMods(List<StatModifier> statMods)
    {
        foreach (var mod in statMods)
        {
            switch (mod.statType)
            {
                case StatType.Damage:
                    damageAmount += mod.flatBonus;
                    damageAmount *= (1 + mod.percentBonus / 100f);
                    break;
                case StatType.KnockbackForce:
                    knockBackAmount += mod.flatBonus;
                    knockBackAmount *= (1 + mod.percentBonus / 100f);
                    break;
                case StatType.CriticalHitChance:
                    criticalHitChance += mod.flatBonus;
                    break;
                case StatType.CriticalHitDamage:
                    criticalHitDamage += mod.flatBonus;
                    break;
                case StatType.BurnChance:
                    criticalHitDamage += mod.flatBonus;
                    break;
            }
        }
    }
        
    private IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        if (!hasHit)
        {
            hasHit = true;
            statusEffectTriggers.Clear();
            rb.velocity = Vector2.zero;
            if (gameObject.activeSelf)
            {
                ObjectPoolManager.Instance.ReturnToPool(gameObject);
            }
        }
    }
}

