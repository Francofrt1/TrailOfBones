using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Splines;

public class WheelcartMovement : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody rb;

    [SerializeField, Range(0f, 1f)] private float splineProgress;

    private bool isBlocked = false;

    public event Action<float> OnWheelcartprogress;
    public event Action Completed;
    private WheelcartController wheelcartController;
    private IStopWheelcart stopWheelcart;

    private void Awake()
    {
        wheelcartController = GetComponent<WheelcartController>();
        stopWheelcart = GetComponent<IStopWheelcart>();

    }

    private void OnEnable()
    {
        stopWheelcart.OnBlockWheelcartRequested += BlockMovement;
    }

    private void OnDisable()
    {
        stopWheelcart.OnBlockWheelcartRequested -= BlockMovement;
    }

    void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        if (isBlocked || spline == null) return;

        splineProgress += (speed / spline.CalculateLength()) * Time.fixedDeltaTime;
        splineProgress = Mathf.Min(splineProgress, 1f);

        Vector3 pos = spline.EvaluatePosition(splineProgress);
        Vector3 tan = spline.EvaluateTangent(splineProgress);

        Vector3 newPos = new Vector3(pos.x, rb.position.y, pos.z);
        Vector3 forward = Vector3.ProjectOnPlane(tan, Vector3.up).normalized;

        float yaw = Quaternion.LookRotation(forward).eulerAngles.y;
        Vector3 euler = rb.rotation.eulerAngles;
        Quaternion rot = Quaternion.Euler(euler.x, yaw, euler.z);

        rb.MovePosition(newPos);
        rb.MoveRotation(rot);

        OnWheelcartprogress?.Invoke(splineProgress);
        if (splineProgress == 1f) { Completed?.Invoke(); }
    }

    private void BlockMovement(bool val)
    {
        isBlocked = val;
    }

    public float GetDuration()
    {
        float length = spline != null ? spline.CalculateLength() : 0;
        return length / speed;
    }

    public void SetSpline(SplineContainer splineContainer)
    {
        spline = splineContainer;
    }
}
