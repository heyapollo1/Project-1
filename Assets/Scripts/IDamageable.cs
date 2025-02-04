using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce, bool isCriticalHit = false, bool isFromStatusEffect = false);
}
