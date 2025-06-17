using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModel : MonoBehaviour
{
    [SerializeField] float speed;
    private float speedFactor = 1f;
    [SerializeField] float baseDamage = 6f;
    public float BaseDamage => baseDamage;
    public Vector3 CalculateMovement(float deltaTime)
    {
        return new Vector3(0f, 0f, speed * deltaTime * speedFactor);
    }

    public void BlockMovement(bool val)
    {
        speedFactor = val ? 0f : 1f;
    }
}
