using System.Collections;
using UnityEngine;

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

    public void Wiggle()
    {
        if (wiggleCO != null)
            StopCoroutine(wiggleCO);

        wiggleCO = StartCoroutine(WiggleCo());
    }

    private IEnumerator WiggleCo()
    {
        float timer;

        Quaternion leftRot = Quaternion.Euler(0f, 0f, -wiggleAngle);
        Quaternion rightRot = Quaternion.Euler(0f, 0f, wiggleAngle);

        timer = 0f;
        while (timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(originalRotation, leftRot, timer / wiggleDuration);
            yield return null;
        }

        timer = 0f;
        while(timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(leftRot, rightRot, timer / wiggleDuration);
            yield return null;
        }

        timer = 0f;

        while(timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(rightRot, leftRot, timer / wiggleDuration);
            yield return null;
        }

        timer = 0f;

        while(timer < wiggleDuration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(leftRot, originalRotation, timer / wiggleDuration);
            yield return null;
        }

        transform.localRotation = originalRotation;
    }
}
