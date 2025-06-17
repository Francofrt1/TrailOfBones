using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Interfaces;
//using FishNet.Object;
using UnityEngine;

public class ProjectilePresenter : MonoBehaviour
{
    private ProjectileModel _model;
    private ProjectileView _view;
    private Collider _selfCollider;
    [SerializeField] private LayerMask enemyLayer;

    private string shootedByID;

    private void Awake()
    {
        _model = GetComponent<ProjectileModel>();
        _view = GetComponent<ProjectileView>();
        _selfCollider = GetComponent<Collider>();

        GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        PerformMovement();
    }

    public void Activate(string shooterID)
    {
        shootedByID = shooterID;
        gameObject.SetActive(true);
    }

    private void PerformMovement()
    {
        transform.position += _model.CalculateMovement(Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;

        bool isEnemy = IsEnemyLayer(layer);

        if (isEnemy)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.TakeDamage(_model.BaseDamage, shootedByID);
        }

        _selfCollider.enabled = false;
        _view.ImpactFX();
        _model.BlockMovement(true);
        shootedByID = null;
        StartCoroutine(DeactivateDeferred(_view.ExplosionTime));
    }

    private IEnumerator DeactivateDeferred(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        gameObject.SetActive(false);
        _selfCollider.enabled = true;
        _view.ResetView();
        _model.BlockMovement(false);
    }

    private bool IsEnemyLayer(int layer)
    {
        return (enemyLayer.value & (1 << layer)) != 0;
    }
}