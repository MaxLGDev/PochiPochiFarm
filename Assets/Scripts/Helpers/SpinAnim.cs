using UnityEngine;

/// <summary>
/// Rotates this object continuously around a certain axis.
/// Useful for sunburst/ray backgrounds behind popups.
/// </summary>
public class SpinAnim : MonoBehaviour
{
    [SerializeField] private float zDegreesPerSecond = 45f;
    [SerializeField] private float xDegreesPerSecond = 45f;
    [SerializeField] private float yDegreesPerSecond = 45f;

    private void Update()
    {
        transform.Rotate(xDegreesPerSecond * Time.deltaTime, yDegreesPerSecond * Time.deltaTime, zDegreesPerSecond * Time.deltaTime);
    }
}
