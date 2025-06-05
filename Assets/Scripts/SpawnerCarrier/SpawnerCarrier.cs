using System;
using UnityEngine;
using UnityEngine.Splines;

public class SpawnerCarrier : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private float speed = 5f;
    [SerializeField, Range(0f, 1f)] private float progressOffset = 0.1f;

    private float t;

    public event Action<float> OnProgress;
    public event Action OnCompleted;

    private void Start()
    {
        t = Mathf.Clamp01(progressOffset);
        transform.position = spline.EvaluatePosition(t);
        Vector3 forward = (Vector3)spline.EvaluateTangent(t);
        forward.Normalize();
        if (forward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    private void Update()
    {
        MoveAlongSpline();
    }

    private void MoveAlongSpline()
    {
        if (t >= 1f)
            return;

        t += (speed / spline.CalculateLength()) * Time.deltaTime;
        t = Mathf.Clamp01(t);

        transform.position = spline.EvaluatePosition(t);

        Vector3 forward = (Vector3)spline.EvaluateTangent(t);
        forward.Normalize();
        if (forward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        OnProgress?.Invoke(t);
        if (t >= 1f)
            OnCompleted?.Invoke();
    }

    public float GetDuration()
    {
        return spline.CalculateLength() / speed;
    }
}
