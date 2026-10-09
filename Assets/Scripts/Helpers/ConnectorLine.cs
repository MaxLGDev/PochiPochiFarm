using UnityEngine;

public class ConnectorLine : MonoBehaviour
{
    // --- References ---
    [SerializeField] private RectTransform rectTransform;


    // ==============================
    // Public Methods
    // ==============================

    public void SetEndpoints(RectTransform nodeA, RectTransform nodeB)
{
    Transform parent = rectTransform.parent;

    Vector2 positionA = parent.InverseTransformPoint(nodeA.TransformPoint(nodeA.rect.center));
    Vector2 positionB = parent.InverseTransformPoint(nodeB.TransformPoint(nodeB.rect.center));

    rectTransform.localPosition = (positionA + positionB) / 2f;

    float distance = Vector2.Distance(positionA, positionB);
    rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);

    Vector2 direction = positionB - positionA;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
}

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}