using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(WheelcartModel))]
[RequireComponent(typeof(SplineAnimate))]
public class WheelcartController : MonoBehaviour, IDamageable, IDeath, IHealthVariation, IWheelcartDuration
{
    private WheelcartModel wheelcartModel;
    public SplineAnimate splineAnimate;

    public event Action OnDie;
    public event Action<float, float> OnHealthVariation;
    public event Action<float> OnWheelcartDuration;


    private void Awake()
    {
        wheelcartModel = GetComponent<WheelcartModel>();
        splineAnimate = GetComponent<SplineAnimate>();
    }

    private void Start()
    {
        splineAnimate.Duration = wheelcartModel.duration;
        OnWheelcartDuration?.Invoke(wheelcartModel.duration);
        splineAnimate.Play();
        OnHealthVariation?.Invoke(wheelcartModel.currentHealth, wheelcartModel.maxHealth);
    }

    public void TakeDamage(float damageAmout, string hittedById)
    {
        wheelcartModel.SetHealth(wheelcartModel.currentHealth - damageAmout);

        if (wheelcartModel.currentHealth <= 0)
        {
            OnDeath(hittedById);
        }
        Debug.Log(wheelcartModel.currentHealth);
        OnHealthVariation?.Invoke(wheelcartModel.currentHealth, wheelcartModel.maxHealth);
    }

    public void OnDeath(string killedById)
    {
        OnDie?.Invoke();
    }

    public string GetTag()
    {
        return gameObject.tag;
    }
}
