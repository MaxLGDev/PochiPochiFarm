using UnityEngine;

public class ConnectorUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    public void SetEndpoints(RectTransform nodeA, RectTransform nodeB)
    {
        Vector2 a = nodeA.anchoredPosition;
        Vector2 b = nodeB.anchoredPosition;

        rectTransform.anchoredPosition = (a + b) / 2f;

        float distance = Vector2.Distance(a, b);
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);

        Vector2 direction = b - a;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}