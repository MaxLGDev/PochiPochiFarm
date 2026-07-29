using UnityEngine;
using UnityEngine.EventSystems;

public class TestRaycast : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CropBox Image clicked");
    }
}