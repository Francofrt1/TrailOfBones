using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(WheelcartModel))]
[RequireComponent(typeof(SplineAnimate))]
public class WheelcartController : MonoBehaviour, IDamageable, IDeath, IAttackRangeProvider
{
    private WheelcartModel wheelcartModel;
    public SplineAnimate splineAnimate;

    public event Action OnWheelcartDestroyed;
    public event Action<float> OnWheelcartHealthVariation;

    private void Awake()
    {
        wheelcartModel = GetComponent<WheelcartModel>();
        splineAnimate = GetComponent<SplineAnimate>();
    }

    private void Start()
    {
        splineAnimate.MaxSpeed = wheelcartModel.speed;
        splineAnimate.Play();
    }

    public void TakeDamage(float damageAmout)
    {
        wheelcartModel.SetHealth(wheelcartModel.currentHealth - damageAmout);

        if (wheelcartModel.currentHealth <= 0)
        {
            OnDeath();
        }
        Debug.Log(wheelcartModel.currentHealth);
        OnWheelcartHealthVariation?.Invoke(wheelcartModel.currentHealth);
    }

    public void OnDeath()
    {
        Destroy(splineAnimate);
        Destroy(wheelcartModel);
        OnWheelcartDestroyed?.Invoke();
        Destroy(gameObject);
    }

    public string GetTag()
    {
        return gameObject.tag;
    }

    public float RangeToBeAttacked()
    {
        return wheelcartModel.distToBeAttacked;
    }
}
