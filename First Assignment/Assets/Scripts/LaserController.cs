using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeamController : MonoBehaviour
{
    [Header("Laser")]
    public float maxDistance = 10f;
    public bool laserEnabled = true;
    public Transform originOverride;
    public Color baseColor = Color.white;

    [Header("Emission do overlay")]
    [Range(1f, 100f)] public float overlayEmissionIntensity = 45f;

    [Header("Ball Pop")]
    [Tooltip("Radius used to detect balls along the laser path (world units).")]
    public float popHitRadius = 0.06f;
    [Tooltip("Layers considered as ball.")]
    public LayerMask ballLayerMask = 8;


    private LineRenderer baseLR;
    private readonly List<LineRenderer> overlayLRs = new(); // lista de LineRenderers para os segmentos
    private readonly List<Material> overlayMats = new(); // materiais correspondentes 

    private static readonly string[] EMISSION_PROPS = { "_EmissionColor", "_EmissiveColor", "_Emission" };
    private const float MIN_SQR_LENGTH = 1e-8f;
    public Color LastLaserColor { get; private set; }

    void Awake()
    {
        baseLR = GetComponent<LineRenderer>();
        baseLR.positionCount = 2;
    }

    void Update()
    {
        baseLR.enabled = laserEnabled;
        if (!laserEnabled) return;

        Vector3 origin = originOverride ? originOverride.position : transform.position;
        Vector3 dir = transform.forward;
        
        float endDistance = maxDistance;
        
        if (Physics.Raycast(origin, dir, out RaycastHit endHit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
            endDistance = endHit.distance;
        Vector3 endPoint = origin + dir * endDistance;

        var rawHits = Physics.RaycastAll(origin, dir, endDistance, ~0, QueryTriggerInteraction.Collide);
        var frames = rawHits
            .Where(h => h.collider != null && h.collider.isTrigger)
            .Select(h => (hit: h, ctrl: h.collider.GetComponentInParent<FrameColorController>()))
            .Where(x => x.ctrl != null)
            .OrderBy(x => x.hit.distance)
            .ToArray();

        while (overlayLRs.Count < frames.Length)
            CreateOverlayLR();

        for (int i = frames.Length; i < overlayLRs.Count; i++)
            overlayLRs[i].enabled = false;

        Vector3 start = origin;
        Vector3 end = (frames.Length > 0) ? frames[0].hit.point : endPoint;
        baseLR.SetPosition(0, start);
        baseLR.SetPosition(1, end);
        baseLR.startColor = baseColor;
        baseLR.endColor = baseColor;

        Color accum = Color.black; 

        for (int i = 0; i < frames.Length; i++)
        {
            var frameHit = frames[i].hit;
            var ctrl = frames[i].ctrl;
            Color c = ctrl ? ctrl.frameColor : baseColor;

            Color primary = (c.r > c.g && c.r > c.b) ? Color.red
                          : (c.g > c.r && c.g > c.b) ? Color.green
                          : Color.blue;


            accum.r = Mathf.Clamp01(accum.r + primary.r);
            accum.g = Mathf.Clamp01(accum.g + primary.g);
            accum.b = Mathf.Clamp01(accum.b + primary.b);

            Vector3 segStart = frameHit.point;
            Vector3 segEnd = (i == frames.Length - 1) ? endPoint : frames[i + 1].hit.point;

            var lr = overlayLRs[i];
            var mat = overlayMats[i];

            lr.widthMultiplier = baseLR.widthMultiplier;
            lr.widthCurve = baseLR.widthCurve;

            if ((segEnd - segStart).sqrMagnitude < MIN_SQR_LENGTH)
            {
                lr.enabled = false;
                continue;
            }

            lr.enabled = true;
            lr.positionCount = 2;
            lr.SetPosition(0, segStart);
            lr.SetPosition(1, segEnd);

            SetOverlayEmission(mat, accum, overlayEmissionIntensity);

          
        }
        if (frames.Length == 0)
        {
            LastLaserColor = baseColor;
        }
        else
        {
            LastLaserColor = accum;
        }

        // ---- POP balls that match the laser's final color ----
            Vector3 laserStart = origin;
            Vector3 laserEnd   = endPoint;
            Vector3 seg        = laserEnd - laserStart;
            float   segLen     = seg.magnitude;

            if (segLen > 1e-4f)
            {
                Vector3 direction = seg / segLen;
                var ray = new Ray(laserStart, direction);

                var hits = Physics.SphereCastAll(ray, popHitRadius, segLen, ballLayerMask, QueryTriggerInteraction.Ignore);
                if (hits != null && hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var col = hits[i].collider;
                        if (!col) continue;

                        var pop = col.GetComponentInParent<BallPop>();
                        if (pop == null || pop.IsPopped) continue;

                        var tag = col.GetComponentInParent<ColorTag>();
                        if (tag == null) continue;

                        if (tag.Matches(LastLaserColor))
                            pop.Pop();
                    }
                }
            }
    }

    private void CreateOverlayLR()
    {
        var child = new GameObject("LaserOverlay_" + overlayLRs.Count);
        child.transform.SetParent(transform, false);
        var lr = child.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        CopyLineRendererSettings(baseLR, lr);

        var srcMat = baseLR.sharedMaterial != null ? baseLR.sharedMaterial : baseLR.material;
        var mat = new Material(srcMat);
        mat.EnableKeyword("_EMISSION");
        lr.material = mat;
        lr.sortingLayerID = baseLR.sortingLayerID;
        lr.sortingOrder = baseLR.sortingOrder + 1 + overlayLRs.Count;

        overlayLRs.Add(lr);
        overlayMats.Add(mat);
    }

    private static void CopyLineRendererSettings(LineRenderer src, LineRenderer dst)
    {
        
        dst.widthMultiplier = src.widthMultiplier;
        dst.widthCurve = src.widthCurve;
        dst.numCornerVertices = src.numCornerVertices;
        dst.numCapVertices = src.numCapVertices;
        dst.textureMode = src.textureMode;
        dst.alignment = src.alignment;
        dst.generateLightingData = src.generateLightingData;
        dst.shadowCastingMode = src.shadowCastingMode;
        dst.receiveShadows = src.receiveShadows;
        dst.sortingLayerID = src.sortingLayerID;
        dst.sortingOrder = src.sortingOrder;
    }

    private void SetOverlayEmission(Material mat, Color color, float intensity)
    {
        if (!mat) return;
        mat.EnableKeyword("_EMISSION");

        Color hdr = color * Mathf.Max(1f, intensity);
        hdr.a = 1f;

        foreach (var p in EMISSION_PROPS)
        {
            if (mat.HasProperty(p))
            {
                mat.SetColor(p, hdr);
                return;
            }
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < overlayMats.Count; i++)
        {
            var m = overlayMats[i];
            if (m != null)
#if UNITY_EDITOR
                DestroyImmediate(m);
#else
                Destroy(m);
#endif
        }
        overlayMats.Clear();
    }

    public void SetActive(bool on)
{
    laserEnabled = on;

    if (baseLR) baseLR.enabled = on;

    if (!on)
    {
        for (int i = 0; i < overlayLRs.Count; i++)
            if (overlayLRs[i]) overlayLRs[i].enabled = false;
    }
}

}
