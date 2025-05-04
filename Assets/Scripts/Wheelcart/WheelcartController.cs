using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class WheelcartController : MonoBehaviour
{
    private WheelcartModel wheelcart;
    public SplineAnimate splineAnimate;

    public event Action OnWheelcartDestroyed;

    private void Awake()
    {
        wheelcart = new WheelcartModel();
        splineAnimate = GetComponent<SplineAnimate>();
    }

    private void Start()
    {
        splineAnimate.MaxSpeed = wheelcart.GetSpeed();
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
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        wheelcart.SetHealth(wheelcart.currentHealth - damageAmount);

        if (wheelcart.currentHealth <= 0)
        {
            OnWheelcartDestroyed?.Invoke();
        }

    }
}
