using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradesZoom : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private TMP_Text zoomText;
    [SerializeField] private float wheelStep = 0.05f;

    private void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0f)
            zoomSlider.value += scroll * wheelStep;
    }

    private void OnEnable()
    {
        zoomSlider.onValueChanged.AddListener(ApplyZoom);
        ApplyZoom(zoomSlider.value);
    }

    private void OnDisable()
    {
        zoomSlider.onValueChanged.RemoveListener(ApplyZoom);
    }

    private void ApplyZoom(float value)
    {
        // 1. World position of the window center, and which point of the tree is under it
        Vector3 center = viewport.TransformPoint(viewport.rect.center);
        Vector3 localPoint = content.InverseTransformPoint(center);
        
        // 2. Change the scale (this moves that point)
        content.localScale = Vector3.one * value;

        // 3. Move the tree so that point is back under the center
        Vector3 newCenter = content.TransformPoint(localPoint);
        content.position += center - newCenter;

        zoomText.text = $"Zoom: x{value:F2}";
    }
}
