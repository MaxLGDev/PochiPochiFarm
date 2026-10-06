using System;
using UnityEngine;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private RectTransform holeFrame;

    public void Highlight(RectTransform target)
    {
        holeFrame.gameObject.SetActive(true);

        holeFrame.position = target.TransformPoint(target.rect.center);
        holeFrame.sizeDelta = target.rect.size;
        
        Canvas canvas = holeFrame.GetComponentInParent<Canvas>().rootCanvas;
    }

    public void Hide() => holeFrame.gameObject.SetActive(false);
}
