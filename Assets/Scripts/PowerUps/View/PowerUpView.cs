using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpView : MonoBehaviour
{
    [SerializeField] private ParticleSystem pickupEffect;
    [SerializeField] private AudioClip pickupSound;

    public void PlayFeedback()
    {   // Prop�sito: Ejecuta los efectos visuales y sonoros cuando se recoge el PowerUp.
        // Precondici�n: El objeto debe tener asignado el efecto y/o sonido en el Inspector.
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity).Play();

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }
}
