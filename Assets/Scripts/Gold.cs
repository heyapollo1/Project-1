using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour, IPullable
{
    public int goldAmount = 20;

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
                EventManager.Instance.TriggerEvent("CurrencyIncrease", goldAmount);
                AudioManager.TriggerSound(clipPrefix: "Pickup_Gold", position: playerTransform.position);
                GameObject goldPickupFX = FXManager.Instance.PlayFX("GoldPickupFX", playerTransform.position);
                goldPickupFX.transform.SetParent(playerTransform);
                goldPickupFX.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                goldPickupFX.transform.transform.localScale = Vector3.one;

                ObjectPoolManager.Instance.ReturnToPool(gameObject);
            }
        }
    }

    public void StartPull()
    {
        isBeingPulled = true;
        currentPullSpeed = 2f;
        Debug.Log("Gold item pull initiated");
    }

    private bool IsWithinPickupRadius()
    {
        float radius = playerPickupRadius != null ? playerPickupRadius.radius * playerTransform.lossyScale.x : 0f;
        return Vector2.Distance(transform.position, playerTransform.position) <= radius;
    }
}
