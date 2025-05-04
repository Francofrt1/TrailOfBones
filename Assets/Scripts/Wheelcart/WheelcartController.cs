using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class WheelcartController : MonoBehaviour
{
    private WheelcartModel wheelcart;
    private SplineAnimate splineAnimate;

    private void Awake()
    {
        wheelcart = new WheelcartModel();
        splineAnimate = GetComponent<SplineAnimate>();
    }

    private void Start()
    {
        splineAnimate.MaxSpeed = wheelcart.getSpeed();
        splineAnimate.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")    // Cambiar el tag al que tengan puestos los enemigos
        {
            wheelcart.takeDamage(10);
        }
    }
}
