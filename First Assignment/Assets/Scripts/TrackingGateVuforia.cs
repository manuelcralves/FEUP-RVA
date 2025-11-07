using UnityEngine;
using Vuforia;

public class TrackingGateVuforia : MonoBehaviour
{
    public bool IsTracked { get; private set; } = true;

    void Awake()
    {
        var handler = GetComponent<DefaultObserverEventHandler>();
        if (handler)
        {
            handler.OnTargetFound.AddListener(() => IsTracked = true);
            handler.OnTargetLost.AddListener(() => IsTracked = false);
            return;
        }

        var obs = GetComponent<ObserverBehaviour>();
        if (obs)
            obs.OnTargetStatusChanged += (_, s) =>
                IsTracked = s.Status == Status.TRACKED || s.Status == Status.EXTENDED_TRACKED;
    }
}
