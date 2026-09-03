using System.Collections;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Plays a quick scale "punch" animation.
/// Useful for emphasizing interactions or completed actions.
/// </summary>
public class PunchAnim : MonoBehaviour
{
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float punchUpDuration = 0.1f;
    [SerializeField] private float punchDownDuration = 0.1f;

    private Vector3 originalScale;

    private Coroutine punchScaleCO;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    //==========================================================================
    // Animation
    //==========================================================================

    /// <summary>
    /// Starts the punch animation.
    /// If already playing, restarts it from the beginning.
    /// </summary>
    public void PunchScale()
    {
        if (punchScaleCO != null)
            StopCoroutine(punchScaleCO);

        punchScaleCO = StartCoroutine(PunchScale(punchUpDuration, punchDownDuration));
    }

    /// <summary>
    /// Scales the object up, then smoothly returns it to its original size.
    /// </summary>
    public IEnumerator PunchScale(float upDuration, float downDuration)
    {
        float punchTimer = 0f;

        // Scale up.
        while (punchTimer < upDuration)
        {
            punchTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * maxScale, punchTimer / upDuration);
            yield return null;
        }

        punchTimer = 0f;

        // Scale back down.
        while (punchTimer < downDuration)
        {
            punchTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * maxScale, originalScale, punchTimer / downDuration);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}