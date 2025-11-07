using UnityEngine;

public class BallLifetime : MonoBehaviour
{
    public float lifetime = 2f;
    float _t0;

    void OnEnable() => _t0 = Time.time;

    void Update()
    {
        if (Time.time - _t0 >= lifetime)
            Destroy(gameObject);
    }
}
