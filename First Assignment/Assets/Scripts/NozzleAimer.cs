using UnityEngine;
using Vuforia;

public class NozzleAimer : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [SerializeField] private LaserBeamController laser;

    [Header("Scene refs")]
    public Transform tower;       
    public Transform nozzle;      
    public Transform arCamera;
    public Transform laserTarget;

    [Header("Aim behavior")]
    public Axis targetUpAxis = Axis.Y;
    public float turnSpeed = 360f;
    public Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Bind to Vuforia tracking (optional)")]
    public bool requireTracking = true;

    ObserverBehaviour _observer;
    bool _isTracked = true;
    Quaternion _rotationOffset;

    void Awake()
    {
        _rotationOffset = Quaternion.Euler(rotationOffsetEuler);

        if (!requireTracking) return;

        _observer = GetComponent<ObserverBehaviour>();
        if (_observer != null)
            _observer.OnTargetStatusChanged += OnTargetStatusChanged;

        var handler = GetComponent<DefaultObserverEventHandler>();
        if (handler != null)
        {
            handler.OnTargetFound.AddListener(() =>
            {
                _isTracked = true;
                if (laser) laser.SetActive(true);
            });

            handler.OnTargetLost.AddListener(() =>
            {
                _isTracked = false;
                if (laser) laser.SetActive(false);
            });
        }
    }

    void OnDestroy()
    {
        if (_observer != null)
            _observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    void Update()
    {
        if (!tower || !nozzle || !arCamera || !laserTarget) return;
        if (requireTracking && !_isTracked) return;

        Vector3 upWS = GetAxisWorld(laserTarget, targetUpAxis).normalized;

        Vector3 camToNozzle = nozzle.position - arCamera.position;

        Vector3 aimDir = Vector3.ProjectOnPlane(camToNozzle, upWS);
        
        if (aimDir.sqrMagnitude < 1e-6f)
        {
            // fallback
            aimDir = Vector3.ProjectOnPlane(laserTarget.forward, upWS);
        }
        aimDir.Normalize();

        
        Quaternion desired = Quaternion.LookRotation(aimDir, upWS) * _rotationOffset;

        if (turnSpeed <= 0f)
        {
            tower.SetPositionAndRotation(tower.position, desired);
        }
        else
        {
            Quaternion smooth = Quaternion.RotateTowards(
                tower.rotation,
                desired,
                turnSpeed * Time.deltaTime
            );
            tower.SetPositionAndRotation(tower.position, smooth);
        }
    }

    void OnTargetStatusChanged(ObserverBehaviour _, TargetStatus status)
    {
        _isTracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;
        if (requireTracking && laser) laser.SetActive(_isTracked);
    }

    static Vector3 GetAxisWorld(Transform t, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return t.right;
            case Axis.Y: return t.up;
            default:     return t.forward;
        }
    }
}
