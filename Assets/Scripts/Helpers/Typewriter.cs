using System.Collections;
using TMPro;
using UnityEngine;

public class Typewriter : MonoBehaviour
{
    // --- References ---
    [SerializeField] private TMP_Text text;

    // --- Settings ---
    [SerializeField] private float charactersPerSecond = 30f;

    // --- State ---
    private Coroutine typeTextCoroutine;


    // ==============================
    // Public Methods
    // ==============================

    public void ShowText(string message)
    {
        // Stop the previous animation before starting a new one.
        if (typeTextCoroutine != null)
            StopCoroutine(typeTextCoroutine);

        typeTextCoroutine = StartCoroutine(TypeText(message));
    }


    // ==============================
    // Animation
    // ==============================

    private IEnumerator TypeText(string message)
    {
        text.text = message;
        text.maxVisibleCharacters = 0;

        while (text.maxVisibleCharacters < message.Length)
        {
            text.maxVisibleCharacters++;
            yield return new WaitForSeconds(1f / charactersPerSecond);
        }
    }
}