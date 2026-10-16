using UnityEngine;

public class PulsingAnim : MonoBehaviour
{
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private float pulseSpeed = 2f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.05f;
        transform.localScale = originalScale * Mathf.Lerp(minScale, maxScale, t);
    }
}
