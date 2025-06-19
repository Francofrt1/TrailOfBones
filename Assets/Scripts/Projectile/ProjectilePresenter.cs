using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Interfaces;
using FishNet.Object;
using UnityEngine;

public class ProjectilePresenter : NetworkBehaviour, IShootable
{
    private ProjectileModel _model;
    private ProjectileView _view;
    private Collider _selfCollider;
    [SerializeField] private LayerMask enemyLayer;

    private string shootedByID;
    private Coroutine _deactivationCoroutine;

    private void Awake()
    {
        _model = GetComponent<ProjectileModel>();
        _view = GetComponent<ProjectileView>();
        _selfCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        // Undock from any parent transform
        transform.parent = null;
    }

    void Update()
    {
        PerformMovement();
    }

    public void Shoot(string shooterID)
    {
        shootedByID = shooterID;

        gameObject.SetActive(true);
        StartDeactivationCoroutine(_model.LifeTime);
    }

    private void PerformMovement()
    {
        transform.position += _model.CalculateMovement(transform.forward, Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;

        if (IsEnemyLayer(layer))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.TakeDamage(_model.BaseDamage, shootedByID);
        }

        _selfCollider.enabled = false;
        _view.ImpactFX();
        _model.BlockMovement(true);
        shootedByID = null;

        StartDeactivationCoroutine(_view.ExplosionTime);
    }

    private void StartDeactivationCoroutine(float seconds)
    {
        // Cancelar cualquier corrutina previa
        if (_deactivationCoroutine != null)
        {
            StopCoroutine(_deactivationCoroutine);
        }

        // Iniciar nueva corrutina
        _deactivationCoroutine = StartCoroutine(DeactivateDeferred(seconds));
    }

    private IEnumerator DeactivateDeferred(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        _selfCollider.enabled = true;
        _view.ResetView();
        _model.BlockMovement(false);
        _deactivationCoroutine = null;
    }

    private bool IsEnemyLayer(int layer)
    {
        return (enemyLayer.value & (1 << layer)) != 0;
    }
}