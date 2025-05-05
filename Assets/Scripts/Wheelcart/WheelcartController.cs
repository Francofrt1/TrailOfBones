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

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")    // Cambiar el tag al que tengan puestos los enemigos
        {
            TakeDamage(wheelcartModel.maxHealth * 5 / 100);
        }
    }

    public void TakeDamage(float damageAmout)
    {
        wheelcartModel.SetHealth(wheelcartModel.currentHealth - damageAmout);

        if (wheelcartModel.currentHealth <= 0)
        {
            OnDeath();
        }
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
}
