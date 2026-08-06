using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a quick fade animation.
/// Useful for emphasizing interactions or completed actions.
/// </summary>
public class FadeAnim : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.1f;
    [SerializeField] private CanvasGroup canvasGroup;

    private Coroutine fadeCO;

    //==========================================================================
    // Animation
    //==========================================================================

    /// <summary>
    /// Starts the fade animation.
    /// If already playing, restarts it from the beginning.
    /// </summary>
    public void Fade(bool fadeIn)
    {
        if (fadeCO != null)
            StopCoroutine(fadeCO);

        fadeCO = StartCoroutine(FadeCo(fadeDuration, fadeIn));
    }

    /// <summary>
    /// Fades in or fades out the button.
    /// </summary>
    public IEnumerator FadeCo(float duration, bool fadeIn)
    {
        float fadeTimer = 0f;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        // Fade
        while (fadeTimer < duration)
        {
            fadeTimer += Time.deltaTime;
            float t = fadeTimer / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}