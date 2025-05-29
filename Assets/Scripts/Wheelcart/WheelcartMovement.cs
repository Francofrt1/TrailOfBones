using System;
using UnityEngine;
using UnityEngine.Splines;

public class WheelcartMovement : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody rb;

    float t;

    public event Action<float> OnWheelcartprogress;
    public event Action Completed;

    void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        t += (speed / spline.CalculateLength()) * Time.fixedDeltaTime;
        t = Mathf.Min(t, 1f);

        Vector3 pos = spline.EvaluatePosition(t);
        Vector3 tan = spline.EvaluateTangent(t);

        Vector3 newPos = new Vector3(pos.x, rb.position.y, pos.z);
        Vector3 forward = Vector3.ProjectOnPlane(tan, Vector3.up).normalized;

        float yaw = Quaternion.LookRotation(forward).eulerAngles.y;
        Vector3 euler = rb.rotation.eulerAngles;
        Quaternion rot = Quaternion.Euler(euler.x, yaw, euler.z);

        rb.MovePosition(newPos);
        rb.MoveRotation(rot);

        OnWheelcartprogress?.Invoke(t);
        if (t == 1f) { Completed?.Invoke(); }
    }

    public float GetDuration()
    {
        float length = spline.CalculateLength();
        return length / speed;
    }

    
}
