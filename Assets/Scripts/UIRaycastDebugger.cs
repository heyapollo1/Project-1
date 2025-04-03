using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"Raycast hit {results.Count} objects.");
            foreach (var result in results)
            {
                Debug.Log($"Blocked by: {result.gameObject.name} (Layer: {LayerMask.LayerToName(result.gameObject.layer)})");
            }
        }
    }
}
