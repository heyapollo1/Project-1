using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingOrb : MonoBehaviour, IPullable
{
    public int healingAmount = 20;

    private Transform playerTransform;
    private float pickupDistance = 0.5f;       
    private bool isBeingPulled = false;       
    private float currentPullSpeed = 0f;       
    private float maxPullSpeed = 14f;          
    private float accelerationRate = 3f;     
    private CircleCollider2D playerPickupRadius;

    private void Start()
    {
        playerTransform = PlayerHealthManager.Instance?.transform;
        playerPickupRadius = playerTransform?.GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (isBeingPulled && playerTransform != null && IsWithinPickupRadius())
        {
            currentPullSpeed = Mathf.Min(currentPullSpeed + accelerationRate * Time.deltaTime, maxPullSpeed);
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)(direction * currentPullSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, playerTransform.position) <= pickupDistance)
            {
                PlayerHealthManager.Instance.Heal(healingAmount);

                AudioManager.TriggerSound(clipPrefix:"Pickup_HealingOrb", position: playerTransform.position);
                GameObject healingFX = FXManager.Instance.PlayFX("HealingFX", playerTransform.position);
                if (healingFX != null)
                {
                    healingFX.transform.SetParent(playerTransform);
                    healingFX.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                }

                ObjectPoolManager.Instance.ReturnToPool(gameObject);
            }
        }
    }

    public void StartPull()
    {
        isBeingPulled = true;
        currentPullSpeed = 2f;
    }

    private bool IsWithinPickupRadius()
    {
        float radius = playerPickupRadius != null ? playerPickupRadius.radius * playerTransform.lossyScale.x : 0f;
        return Vector2.Distance(transform.position, playerTransform.position) <= radius;
    }
}