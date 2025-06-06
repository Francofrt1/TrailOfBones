using System;
using UnityEngine;
using UnityEngine.Splines;

public class SpawnerCarrier : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private float speed = 5f;
    [SerializeField, Range(0f, 1f)] private float initialProgressOffset = 0.1f;
    [SerializeField] private float routeEnd = 0.92f;

    [SerializeField, Range(0f, 1f)] private float splineProgress;

    public event Action<float> OnProgress;
    public event Action OnCompleted;

    private void Start()
    {
        splineProgress = Mathf.Clamp01(initialProgressOffset);
        transform.position = spline.EvaluatePosition(splineProgress);
        Vector3 forward = (Vector3)spline.EvaluateTangent(splineProgress);
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
        if (splineProgress >= routeEnd)
            return;

        splineProgress += (speed / spline.CalculateLength()) * Time.deltaTime;
        splineProgress = Mathf.Clamp01(splineProgress);

        transform.position = spline.EvaluatePosition(splineProgress);

        Vector3 forward = (Vector3)spline.EvaluateTangent(splineProgress);
        forward.Normalize();
        if (forward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        OnProgress?.Invoke(splineProgress);
        if (splineProgress >= routeEnd)
            OnCompleted?.Invoke();
    }

    public float GetDuration()
    {
        return spline.CalculateLength() / speed;
    }
}
