using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text floatingText;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatDuration = 1f;
    private float timer;

    public void Setup(string text, Color color)
    {
        floatingText.text = text;
        floatingText.color = color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
        floatingText.alpha = 1f - (timer / floatDuration);
        
        if(timer >= floatDuration)
            Destroy(gameObject);
    }
}
