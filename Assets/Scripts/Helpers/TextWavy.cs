using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- Colors ---
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    // --- Animation ---
    [SerializeField] private float moveAmount = 5f;
    [SerializeField] private float animationSpeed = 8f;

    // --- State ---
    private TMP_Text text;
    private Vector3 startPosition;
    private Coroutine animationCoroutine;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        startPosition = transform.localPosition;

        text.color = normalColor;
    }

    private void OnDisable()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        ResetVisual();
    }


    // ==============================
    // Pointer Events
    // ==============================

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartAnimation(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartAnimation(false);
    }


    // ==============================
    // Animation
    // ==============================

    private void StartAnimation(bool hover)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(Animate(hover));
    }

    private IEnumerator Animate(bool hover)
    {
        Vector3 targetPosition = hover
            ? startPosition + Vector3.up * moveAmount
            : startPosition;

        Color targetColor = hover ? hoverColor : normalColor;

        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f ||
               text.color != targetColor)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * animationSpeed
            );

            text.color = Color.Lerp(
                text.color,
                targetColor,
                Time.deltaTime * animationSpeed
            );

            yield return null;
        }

        transform.localPosition = targetPosition;
        text.color = targetColor;
    }


    // ==============================
    // Reset
    // ==============================

    private void ResetVisual()
    {
        // Restore the text to its default appearance.
        transform.localPosition = startPosition;
        text.color = normalColor;
    }
}