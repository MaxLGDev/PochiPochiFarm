using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScrollImageBackground : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.05f, 0.05f);

    private Image bgImage;
    private Material runtimeMaterial;

    void Awake()
    {
        bgImage = GetComponent<Image>();

        // Create an instance of the material so we don't modify the asset file on disk
        if (bgImage.material != null)
        {
            runtimeMaterial = new Material(bgImage.material);
            bgImage.material = runtimeMaterial;
        }
        else
        {
            Debug.LogError("Image component needs a Material assigned with a repeatable texture!", this);
        }
    }

    void Update()
    {
        if (runtimeMaterial == null) return;

        // Shift the texture offset continuously over time
        Vector2 currentOffset = runtimeMaterial.mainTextureOffset;
        currentOffset += scrollSpeed * Time.deltaTime;
        runtimeMaterial.mainTextureOffset = currentOffset;
    }

    void OnDestroy()
    {
        // Clean up the instantiated material to avoid memory leaks
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
}