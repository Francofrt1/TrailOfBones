using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePresenter : MonoBehaviour
{
    private ProjectileModel _model;
    private ProjectileView _view;

    private void Awake()
    {
        _model = GetComponent<ProjectileModel>();
        _view = GetComponent<ProjectileView>();
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        transform.position += _model.CalculateMovement(Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        _view.ImpactVFX();
        _model.SetSpeedFactor(0f);
        StartCoroutine(DeactivateDeferred(_view.ExplosionTime));
    }

    private IEnumerator DeactivateDeferred(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        gameObject.SetActive(false);
    }
}
