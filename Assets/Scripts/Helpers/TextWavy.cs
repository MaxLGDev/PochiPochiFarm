using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- Colors ---
    [SerializeField] private bool animateColor = true;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    // --- Animation ---
    [SerializeField] private float moveAmount = 5f;
    [SerializeField] private float animationSpeed = 8f;

    // --- State ---
    private Graphic graphic;
    private Vector3 startPosition;
    private Coroutine animationCoroutine;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
        startPosition = transform.localPosition;

        if (animateColor && graphic != null)
            graphic.color = normalColor;
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

    public void OnPointerEnter(PointerEventData eventData) => StartAnimation(true);

    public void OnPointerExit(PointerEventData eventData) => StartAnimation(false);


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
               graphic.color != targetColor)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.unscaledDeltaTime * animationSpeed
            );

            graphic.color = Color.Lerp(
                graphic.color,
                targetColor,
                Time.unscaledDeltaTime * animationSpeed
            );

            yield return null;
        }

        transform.localPosition = targetPosition;
        graphic.color = targetColor;
    }


    // ==============================
    // Reset
    // ==============================

    private void ResetVisual()
    {
        // Restore the text to its default appearance.
        transform.localPosition = startPosition;
        graphic.color = normalColor;
    }
}