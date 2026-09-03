using System.Collections;

using UnityEngine;

/// <summary>
/// Plays a quick left-right rotation animation.
/// Useful for giving feedback when an object is interacted with.
/// </summary>
public class WiggleAnim : MonoBehaviour
{
    [SerializeField] private float wiggleAngle = 20f;
    [SerializeField] private float wiggleDuration = 0.05f;

    private Quaternion originalRotation;
    private Coroutine wiggleCO;

    private void Awake()
    {
        originalRotation = transform.localRotation;
    }

    //==========================================================================
    // Animation
    //==========================================================================

    /// <summary>
    /// Starts the wiggle animation.
    /// If already playing, restarts it from the beginning.
    /// </summary>
    public void Wiggle()
    {
        if (wiggleCO != null)
            StopCoroutine(wiggleCO);

        wiggleCO = StartCoroutine(WiggleCo());
    }

    /// <summary>
    /// Rotates the object left, right, left, then back to its original rotation.
    /// </summary>
    private IEnumerator WiggleCo()
    {
        float timer;

        Quaternion leftRot = Quaternion.Euler(0f, 0f, -wiggleAngle);
        Quaternion rightRot = Quaternion.Euler(0f, 0f, wiggleAngle);

        // Rotate left.
        timer = 0f;
        while (timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(originalRotation, leftRot, timer / wiggleDuration);
            yield return null;
        }

        // Rotate right.
        timer = 0f;
        while (timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(leftRot, rightRot, timer / wiggleDuration);
            yield return null;
        }

        // Rotate back left.
        timer = 0f;
        while (timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(rightRot, leftRot, timer / wiggleDuration);
            yield return null;
        }

        // Return to the original rotation.
        timer = 0f;
        while (timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(leftRot, originalRotation, timer / wiggleDuration);
            yield return null;
        }

        transform.localRotation = originalRotation;
    }
}