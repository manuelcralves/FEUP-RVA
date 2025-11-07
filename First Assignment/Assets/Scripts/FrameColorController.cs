using UnityEngine;

public class FrameColorController : MonoBehaviour
{
    [Header("Frame Colors")]
    public Color frameColor = Color.red;      // Tint color for the bars
    public Color windowColor = Color.red;     // Tint color for the window (usually lighter)

    private Renderer[] barRenderers;
    private Renderer windowRenderer;

    void Awake()
    {
        // Automatically find all renderers under the frame
        barRenderers = GetComponentsInChildren<Renderer>(true);

        // Find the one named "Window"
        foreach (var r in barRenderers)
        {
            if (r.gameObject.name.ToLower().Contains("window"))
            {
                windowRenderer = r;
            }
        }

        // Filter only bars (not window)
        barRenderers = System.Array.FindAll(barRenderers, r => r != windowRenderer);
    }

    void Start()
    {
        ApplyColors();
    }

    public void ApplyColors()
    {
        // Tint all bars (they all share BarsMaterial)
        foreach (var r in barRenderers)
        {
            if (r != null)
                r.material.color = frameColor;
        }

        // Tint the window (uses WindowMaterial)
        if (windowRenderer != null)
            windowRenderer.material.color = new Color(windowColor.r, windowColor.g, windowColor.b, 0.7f);
    }

    // Optional: to change dynamically later
    public void SetColors(Color newFrameColor, Color newWindowColor)
    {
        frameColor = newFrameColor;
        windowColor = newWindowColor;
        ApplyColors();
    }
}
