using System.Collections;
using TMPro;
using UnityEngine;

public class Typewriter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float charactersPerSecond = 30f;

    private Coroutine TypeTextCo;

    public void ShowText(string message)
    {
        if(TypeTextCo != null)
            StopCoroutine(TypeTextCo);

        TypeTextCo = StartCoroutine(TypeText(message));
        
    }

    private IEnumerator TypeText(string message)
    {
        text.text = message;
        text.maxVisibleCharacters = 0;

        while(text.maxVisibleCharacters < message.Length)
        {
            text.maxVisibleCharacters++;
            yield return new WaitForSeconds(1f /  charactersPerSecond);
        }
    }
}
