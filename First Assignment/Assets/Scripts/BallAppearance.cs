using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BallAppearance : MonoBehaviour
{
    [Header("Target Renderers")]
    [Tooltip("Leave empty to auto-detect a Renderer on this object or its children. You can target multiple parts (e.g., rock + core).")]
    public Renderer[] targets;

    [Header("Tint Mode")]
    [Tooltip("Multiply/Blend the tint with the material's original base color, instead of replacing it.")]
    [Range(0f, 1f)] public float tintStrength = 0.6f; // 0 = keep original, 1 = full tint
    [Tooltip("Extra brightness multiplier applied after blending (helps readability without washing out textures).")]
    [Range(0.5f, 2f)] public float brightness = 1.0f;

    [Header("Surface Polish (optional)")]
    [Tooltip("Set to >=0 to override Metallic on URP Lit materials.")]
    [Range(-1f, 1f)] public float metallicOverride = -1f; // -1 = don't touch
    [Tooltip("Set to >=0 to override Smoothness on URP Lit materials.")]
    [Range(-1f, 1f)] public float smoothnessOverride = -1f; // -1 = don't touch

    [Header("Emission (optional)")]
    public bool useEmission = false;
    [Range(0f, 50f)] public float emissionIntensity = 6f;

    // Common shader property IDs
    static readonly int BaseColorID   = Shader.PropertyToID("_BaseColor");
    static readonly int ColorID       = Shader.PropertyToID("_Color");
    static readonly int EmissionID    = Shader.PropertyToID("_EmissionColor");
    static readonly int MetallicID    = Shader.PropertyToID("_Metallic");
    static readonly int SmoothnessID  = Shader.PropertyToID("_Smoothness");

    MaterialPropertyBlock _mpb;

    // Cache the original base colors so we can blend without destroying the look
    Color[] _originalBaseColors;

    void Awake()
    {
        if (targets == null || targets.Length == 0)
        {
            var r = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();
            targets = r ? new[] { r } : new Renderer[0];
        }

        _mpb = new MaterialPropertyBlock();
        _originalBaseColors = new Color[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            var r = targets[i];
            if (!r) continue;

            var mat = r.sharedMaterial;
            if (mat != null)
            {
                if (mat.HasProperty(BaseColorID))
                    _originalBaseColors[i] = mat.GetColor(BaseColorID);
                else if (mat.HasProperty(ColorID))
                    _originalBaseColors[i] = mat.GetColor(ColorID);
                else
                    _originalBaseColors[i] = Color.white; 
            }
            else
            {
                _originalBaseColors[i] = Color.white;
            }
        }
    }


    public void ApplyColor(Color tint)
    {
        if (targets == null) return;

        for (int i = 0; i < targets.Length; i++)
        {
            var r = targets[i];
            if (!r) continue;

            r.GetPropertyBlock(_mpb);

            var orig = (i < _originalBaseColors.Length) ? _originalBaseColors[i] : Color.white;
            var blended = Color.Lerp(orig, tint, Mathf.Clamp01(tintStrength));
            blended *= brightness;
            blended.a = 1f;

            var mat = r.sharedMaterial;

            if (mat && mat.HasProperty(BaseColorID))
                _mpb.SetColor(BaseColorID, blended);
            else
                _mpb.SetColor(ColorID, blended);

            if (metallicOverride >= 0f && mat && mat.HasProperty(MetallicID))
                _mpb.SetFloat(MetallicID, metallicOverride);
            if (smoothnessOverride >= 0f && mat && mat.HasProperty(SmoothnessID))
                _mpb.SetFloat(SmoothnessID, smoothnessOverride);

            if (useEmission)
            {
                var emiss = tint * Mathf.Max(0f, emissionIntensity);
                emiss.a = 1f;
                _mpb.SetColor(EmissionID, emiss);
            }

            r.SetPropertyBlock(_mpb);
        }

    }
}
