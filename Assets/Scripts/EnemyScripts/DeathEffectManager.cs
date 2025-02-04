using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectManager : BaseManager
{
    public static DeathEffectManager Instance { get; private set; }
    public override int Priority => 30;

    private Dictionary<GameObject, List<IOnDeathEffect>> enemyDeathEffects = new Dictionary<GameObject, List<IOnDeathEffect>>();

    protected override void OnInitialize()
    {
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterOnDeathEffect(GameObject enemy, IOnDeathEffect effect)
    {
        if (!enemyDeathEffects.ContainsKey(enemy))
        {
            enemyDeathEffects[enemy] = new List<IOnDeathEffect>();
        }
        enemyDeathEffects[enemy].Add(effect);
    }

    public void TriggerDeathEffects(GameObject enemy)
    {
        if (enemyDeathEffects.TryGetValue(enemy, out List<IOnDeathEffect> effects))
        {
            foreach (var effect in effects)
            {
                ExplosionQueueManager.Instance.EnqueueExplosion(effect, enemy);
            }
        }
    }

    public void ClearDeathEffects(GameObject enemy)
    {
        if (enemyDeathEffects.ContainsKey(enemy))
        {
            enemyDeathEffects.Remove(enemy);
        }
    }

    public void DeregisterOnDeathEffect(GameObject enemy, IOnDeathEffect effect)
    {
        if (enemyDeathEffects.TryGetValue(enemy, out var effects))
        {
            effects.Remove(effect);
            if (effects.Count == 0) enemyDeathEffects.Remove(enemy);
        }
    }
}