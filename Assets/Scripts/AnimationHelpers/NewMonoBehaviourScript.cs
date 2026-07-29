using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClickDebugger : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            // Check every Canvas + its raycaster setup
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvases)
            {
                var raycaster = canvas.GetComponent<GraphicRaycaster>();
                Debug.Log($"Canvas '{canvas.name}': RenderMode={canvas.renderMode}, " +
                          $"WorldCamera={(canvas.worldCamera != null ? canvas.worldCamera.name : "NULL")}, " +
                          $"HasGraphicRaycaster={(raycaster != null)}, " +
                          $"RaycasterEnabled={(raycaster != null && raycaster.enabled)}");
            }

            // Standard UI raycast
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePos
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"RaycastAll hit {results.Count} UI objects:");
            foreach(var result in results)
{
                Debug.Log(
                    $"{result.gameObject.name} | " +
                    $"depth={result.depth} | " +
                    $"sortingLayer={result.sortingLayer} | " +
                    $"sortingOrder={result.sortingOrder} | " +
                    $"module={result.module}"
                );
            }

            if (results.Count == 0)
            {
                Debug.LogWarning("No UI hit at all. Check: Canvas Render Camera assignment, " +
                                  "EventSystem's assigned UI camera (Input System UI Input Module), " +
                                  "or a disabled/misconfigured GraphicRaycaster.");
            }
        }
    }
}