using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModel : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float speedFactor = 1f;

    public Vector3 CalculateMovement(float deltaTime)
    {
        return new Vector3(0f, 0f, speed * deltaTime * speedFactor);
    }

    public void SetSpeedFactor(float val)
    {
        speedFactor = val;
    }
}
