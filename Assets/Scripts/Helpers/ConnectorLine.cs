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
        var positionA = nodeA.anchoredPosition;
        var positionB = nodeB.anchoredPosition;

        // Position the line between the two nodes.
        rectTransform.anchoredPosition = (positionA + positionB) / 2f;

        // Set the line's width to match the distance between the nodes.
        var distance = Vector2.Distance(positionA, positionB);
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);

        // Rotate the line so it points from node A to node B.
        var direction = positionB - positionA;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}