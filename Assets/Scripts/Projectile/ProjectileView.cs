using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    [SerializeField] GameObject bodyMesh;
    [SerializeField] GameObject travelParticles;
    [SerializeField] GameObject explosionParticles;
    [SerializeField] float explosionTime = 2;
    public float ExplosionTime => explosionTime;


    public void ImpactVFX()
    {
        bodyMesh.gameObject.SetActive(false);
        travelParticles.gameObject.SetActive(false);
        explosionParticles.gameObject.SetActive(true);

    }
}
