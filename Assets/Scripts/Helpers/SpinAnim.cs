using UnityEngine;

/// <summary>
/// Rotates this object continuously around Z.
/// Useful for sunburst/ray backgrounds behind popups.
/// </summary>
public class SpinAnim : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 45f;

    private void Update()
    {
        transform.Rotate(0f, 0f, -degreesPerSecond * Time.deltaTime);
    }
}
