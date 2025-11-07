using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class BallPop : MonoBehaviour
{
    public static event Action<int> OnAnyBallPopped;

    [Header("VFX/SFX")]
    [Tooltip("VFX prefab to spawn at the ball position on pop (from Impact Effects).")]
    public GameObject popVfxPrefab;
    public AudioClip popSfxClip;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Behavior")]
    public float destroyDelay = 0.05f;
    public bool disableRendererOnPop = true;

    bool popped;
    public bool IsPopped => popped;

    public void Pop()
    {
        if (popped) return;
        popped = true;

        int points = 0;
        var tag = GetComponent<ColorTag>();
        if (tag != null) points = tag.ScoreValue();
        OnAnyBallPopped?.Invoke(points);

        // --- Spawn VFX prefab at ball position ---
        if (popVfxPrefab)
        {
            var vfx = Instantiate(popVfxPrefab, transform.position, Quaternion.identity);
            // If it has a ParticleSystem, auto-destroy after it finishes
            var ps = vfx.GetComponent<ParticleSystem>();
            if (ps)
            {
                var total = ps.main.duration + ps.main.startLifetime.constantMax + 0.25f;
                Destroy(vfx, total);
            }
            else
            {
                Destroy(vfx, 2f);
            }
        }

        // --- Play one-shot SFX (no AudioSource required on the ball) ---
        if (popSfxClip)
            AudioSource.PlayClipAtPoint(popSfxClip, transform.position, sfxVolume);

        if (disableRendererOnPop)
            foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
        var rb = GetComponent<Rigidbody>(); if (rb) rb.isKinematic = true;

        Destroy(gameObject, destroyDelay);
    }
}
