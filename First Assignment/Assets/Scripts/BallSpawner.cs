using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("Ball Setup")]
    public GameObject ballPrefab;
    [Range(1, 100)] public int maxBalls = 10;
    public float lifetimeSeconds = 2f;
    public float spawnIntervalSeconds = 0.25f;
    [Range(0.01f, 5f)] public float ballScale = 1f;

    [Header("Placement (XZ ring around target)")]
    public float minRadius = 0.15f;
    public float maxRadius = 0.30f;
    public float yOffset = 0f;

    [Header("Gating")]
    public bool requireTracking = true;
    public TrackingGateVuforia trackingGate; // assign same GO as your Vuforia observer, or leave null if not needed

    [Header("Palette")]
    public BallColor[] palette = new BallColor[]
    {
        BallColor.R, BallColor.G, BallColor.B,
        BallColor.C, BallColor.M, BallColor.Y
    };

    readonly System.Collections.Generic.Queue<GameObject> _queue = new();
    float _nextSpawnTime;

    void Update()
    {
        if (!ballPrefab) return;
        if (requireTracking && trackingGate && !trackingGate.IsTracked) return;

        // Trim by lifetime: handled per-ball (BallLifetime)
        // Trim by max
        while (_queue.Count > maxBalls)
        {
            var oldest = _queue.Dequeue();
            if (oldest) Destroy(oldest);
        }

        if (Time.time >= _nextSpawnTime)
        {
            SpawnOne();
            _nextSpawnTime = Time.time + Mathf.Max(0.01f, spawnIntervalSeconds);
        }
    }

    void SpawnOne()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float r = Random.Range(minRadius, maxRadius);
        Vector3 localPos = new(Mathf.Cos(angle) * r, yOffset, Mathf.Sin(angle) * r);

        var go = Instantiate(ballPrefab, transform);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one * ballScale;

        // ensure lifetime component
        var life = go.GetComponent<BallLifetime>() ?? go.AddComponent<BallLifetime>();
        life.lifetime = lifetimeSeconds;

        // pick & apply color
        var bc = palette[Random.Range(0, palette.Length)];
        var tag = go.GetComponent<ColorTag>() ?? go.AddComponent<ColorTag>();
        tag.Set(bc);

        var app = go.GetComponent<BallAppearance>() ?? go.AddComponent<BallAppearance>();
        app.ApplyColor(bc.ToUnityColor());

        _queue.Enqueue(go);
    }
}
