using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class CircleAoE : MonoBehaviour
{
    public Action<List<Collider2D>, Vector3> OnEnemiesDetected;  // Pass explosion origin
    //public Action<List<Collider2D>> OnEnemiesDetected;  // Event to notify enemies detected

    private CircleCollider2D circleCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void UpdateScale(float areaSize)
    {
        // Still apply the visual scaling
        float widthScale = areaSize * 2.0f;  // X-axis stretched
        float heightScale = areaSize * 1.5f; // Y-axis slightly squished
        transform.localScale = new Vector3(widthScale, heightScale, 1f);

        // Adjust the circle collider to match the smaller axis (height in this case)
        circleCollider.radius = heightScale / 2.0f;  // Use the height scale to prevent it from being too large
        Debug.Log($"Updated Collider Radius: {circleCollider.radius}, Visual Scale: Width = {widthScale}, Height = {heightScale}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log($"Enemy detected: {other.name}");

            List<Collider2D> detectedEnemies = new List<Collider2D> { other };
            OnEnemiesDetected?.Invoke(detectedEnemies, transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (circleCollider == null) return;

        Gizmos.color = Color.green;

        // Draw a wireframe circle to represent the collider
        Vector3 colliderPosition = transform.position;
        float radius = circleCollider.radius * transform.localScale.x;  // Scale radius properly

        Gizmos.DrawWireSphere(colliderPosition, radius);
    }
}

/*[RequireComponent(typeof(PolygonCollider2D))]
public class OvalCollider : MonoBehaviour
{
    public Action<List<Collider2D>> OnEnemiesDetected;  // Event to notify enemies detected

    private PolygonCollider2D polygonCollider;

    private void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        InitializeOvalShape();
    }

    private void InitializeOvalShape()
    {
        Vector2[] points = GenerateOvalPoints(12);  // 12 points for smooth shape
        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, points);
    }

    private Vector2[] GenerateOvalPoints(int segments)
    {
        Vector2[] points = new Vector2[segments];
        float angleStep = 2 * Mathf.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            points[i] = new Vector2(x, y);
        }

        return points;
    }

    public void UpdateScale(float areaSize)
    {
        transform.localScale = new Vector3(areaSize * 2.0f, areaSize * 1.5f, 1f);  // Maintain oval aspect ratio
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            List<Collider2D> detectedEnemies = new List<Collider2D> { other };
            OnEnemiesDetected?.Invoke(detectedEnemies);  // Notify BombAbility of detected enemies
        }
    }
}*/