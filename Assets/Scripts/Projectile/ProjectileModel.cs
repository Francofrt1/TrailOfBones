using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModel : MonoBehaviour
{
    [SerializeField] float speed;
    private float speedFactor = 1f;
    [SerializeField] float baseDamage = 6f;
    public float BaseDamage => baseDamage;
    [SerializeField] float lifeTime = 8f;
    public float LifeTime => lifeTime;


    public Vector3 CalculateMovement(Vector3 referenceForward, float deltaTime)
    {
        return referenceForward * speed * deltaTime * speedFactor;
    }

    public void BlockMovement(bool val)
    {
        speedFactor = val ? 0f : 1f;
    }
}
