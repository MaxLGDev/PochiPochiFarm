using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps the world-space grid scaled exactly like the Canvas UI.
/// Reads the canvas scale factor, so it follows whatever Canvas Scaler
/// settings (reference resolution, match width/height) the UI uses.
/// Only adjusts the camera, owns no game state.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    // --- References ---
    [SerializeField] private Canvas canvas;

    // --- Runtime Data ---
    private Camera cam;
    private CanvasScaler canvasScaler;

    // Orthographic size that looks right at the Canvas Scaler's reference resolution.
    private float referenceSize;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        cam = GetComponent<Camera>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();

        // Set the Game view to the reference resolution and tune the
        // camera size by eye before pressing Play; it is saved here.
        referenceSize = cam.orthographicSize;
    }

    // LateUpdate so the canvas has already updated its scale factor this frame.
    private void LateUpdate()
    {
        Fit();
    }


    // ==============================
    // Fitting
    // ==============================

    /// <summary>
    /// Resizes the camera so the grid grows and shrinks with the UI.
    /// </summary>
    private void Fit()
    {
        // Screen height expressed in canvas units.
        float screenHeightInCanvasUnits = Screen.height / canvas.scaleFactor;

        // At the reference resolution this is exactly the reference height,
        // so the camera keeps its original size there.
        float referenceHeight = canvasScaler.referenceResolution.y;

        cam.orthographicSize = referenceSize * screenHeightInCanvasUnits / referenceHeight;
    }
}
