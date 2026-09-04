using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScrollImageBackground : MonoBehaviour
{
    // --- Settings ---
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.05f, 0.05f);

    // --- References ---
    private Image bgImage;
    private Material runtimeMaterial;


    // ==============================
    // Unity Lifecycle
    // ==============================

    private void Awake()
    {
        bgImage = GetComponent<Image>();

        // Create a runtime copy so the shared material is not modified.
        if (bgImage.material != null)
        {
            runtimeMaterial = new Material(bgImage.material);
            bgImage.material = runtimeMaterial;
        }
        else
        {
            Debug.LogError(
                "Image component needs a Material assigned with a repeatable texture!",
                this
            );
        }
    }

    private void Update()
    {
        if (runtimeMaterial == null)
            return;

        // Move the texture offset over time to create the scrolling effect.
        Vector2 currentOffset = runtimeMaterial.mainTextureOffset;
        currentOffset += scrollSpeed * Time.deltaTime;
        runtimeMaterial.mainTextureOffset = currentOffset;
    }

    private void OnDestroy()
    {
        // Clean up the runtime material instance.
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
}