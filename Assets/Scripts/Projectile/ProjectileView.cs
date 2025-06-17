using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    [SerializeField] GameObject travelParticles;
    [SerializeField] GameObject explosionParticles;
    [SerializeField] float explosionTime = 2;
    public float ExplosionTime => explosionTime;


    public void ImpactFX()
    {
        travelParticles.gameObject.SetActive(false);
        explosionParticles.gameObject.SetActive(true);
    }

    public void ResetView()
    {
        travelParticles.gameObject.SetActive(true);
        explosionParticles.gameObject.SetActive(false);
    }
}
