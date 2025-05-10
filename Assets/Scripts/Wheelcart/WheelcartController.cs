using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(WheelcartModel))]
[RequireComponent(typeof(SplineAnimate))]
public class WheelcartController : MonoBehaviour, IDamageable, IDeath
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
        splineAnimate.Duration = wheelcartModel.duration;
        splineAnimate.Play();
    }

    public void TakeDamage(float damageAmout, string killedById)
    {
        wheelcartModel.SetHealth(wheelcartModel.currentHealth - damageAmout);

        if (wheelcartModel.currentHealth <= 0)
        {
            OnDeath(killedById);
        }
        Debug.Log(wheelcartModel.currentHealth);
        OnWheelcartHealthVariation?.Invoke(wheelcartModel.currentHealth);
    }

    public void OnDeath(string killedById)
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
}
