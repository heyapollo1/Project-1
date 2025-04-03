using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZapperEffect : IPassiveEffect
{
    private float baseDamage;
    private float baseRange;
    private float knockbackForce;
    private float critChance;
    private float critDamage;

    private Transform player;
    private EnemyDetector enemyDetector;
    private GameObject lightningPrefab;
    private ObjectPoolManager pool;
    private ChainLightning chainLightning;

    public ZapperEffect(float damage, float range, float knockback, float criticalHitDamage, float criticalHitChance)
    {
        baseDamage = damage;
        baseRange = range;
        knockbackForce = knockback;
        critChance = criticalHitChance;
        critDamage = criticalHitDamage;
    }

    public void Apply()
    {
        player = PlayerController.Instance.transform;
        enemyDetector = EnemyDetector.Instance;
        pool = ObjectPoolManager.Instance;
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "LightningProjectile", 10);
    }

    public void Remove()
    {
        FXManager.Instance.RemovePool("LightningProjectile");
        // Just clear the combat effect
        //PlayerCombat.Instance.UnregisterCombatEffect(new ZapperCombatHook(this)); // You'll want better equality tracking here
    }

    public void TriggerZap()
    {
        GameObject target = enemyDetector.FindNearestEnemyInRange(baseRange);
        if (target == null) return;

        Vector3 origin = player.position;
        Vector3 end = target.transform.position;
        Vector2 direction = (end - origin).normalized;

        if (target.TryGetComponent(out EnemyHealthManager enemyHealth))
        {
            bool isCrit = true;
            float dmg = critDamage > 0 ? critDamage : baseDamage;
            enemyHealth.TakeDamage(dmg, direction, knockbackForce, DamageSource.Player, isCrit);
        }

        DrawLightning(origin, end);
    }

    private void DealDamage(GameObject target, Vector3 origin)
    {
        if (target.TryGetComponent(out EnemyHealthManager enemyHealth))
        {
            Vector2 knockbackDir = (target.transform.position - origin).normalized;
            enemyHealth.TakeDamage(baseDamage, knockbackDir, knockbackForce, DamageSource.Player, false);
        }
    }

    private void DrawLightning(Vector3 start, Vector3 end)
    {
        GameObject bolt = pool.GetFromPool("LightningProjectile", start, Quaternion.identity);
        LineRenderer line = bolt.GetComponent<LineRenderer>();

        int segments = 8;
        line.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            point.x += Random.Range(-0.2f, 0.2f);
            point.y += Random.Range(-0.2f, 0.2f);
            line.SetPosition(i, point);
        }

        player.GetComponent<MonoBehaviour>().StartCoroutine(EndLightning(bolt, 0.1f));
    }

    private IEnumerator EndLightning(GameObject bolt, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.Instance.ReturnToPool(bolt);
    }
    
    /*private class ZapperCombatHook : ICombatEffect
    {
        private ZapperEffect zap;

        public ZapperCombatHook(ZapperEffect effect) => zap = effect;

        public void OnHit(GameObject target, float damageDealt)
        {
            if (AttributeManager.Instance.IsCriticalHit())
            {
                zap.TriggerZap();
            }
        }

        public void OnHurt(float currentHealth, float maxHealth) { }
        public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) { }
        public float ModifyDamage(float damage, GameObject target) => damage;
    }*/
}