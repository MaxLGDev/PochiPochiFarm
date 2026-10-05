using System;
using UnityEngine;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private RectTransform holeFrame;
    [SerializeField] private RectTransform testTarget;

    private void Start()
    {
        Highlight(testTarget);
    }

    public void Highlight(RectTransform target)
    {
        holeFrame.gameObject.SetActive(true);

        holeFrame.position = target.TransformPoint(target.rect.center);
        holeFrame.sizeDelta = target.rect.size;
        
        Debug.Log($"[Overlay] target: {target.name} | pivot: {target.pivot} | rect: {target.rect} | position: {target.position}");
        Debug.Log($"[Overlay] computed center: {target.TransformPoint(target.rect.center)}");
        Debug.Log($"[Overlay] hole position: {holeFrame.position} | hole rect size: {holeFrame.rect.size} | anchors: {holeFrame.anchorMin}-{holeFrame.anchorMax} | pivot: {holeFrame.pivot}");

        Canvas canvas = holeFrame.GetComponentInParent<Canvas>().rootCanvas;
        Debug.Log($"[Overlay] canvas mode: {canvas.renderMode} | scaleFactor: {canvas.scaleFactor} | hole parent: {holeFrame.parent.name}");
    }

    public void Hide() => holeFrame.gameObject.SetActive(false);
}
