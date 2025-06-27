using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PowerUpModel model;
    private PowerUpView view;

    private void Awake()
    {
        view = GetComponent<PowerUpView>(); // Propósito: Inicializa las referencias del PowerUp.
    }

    private void OnTriggerEnter(Collider other)
    {   // Propósito: Detectar colisiones con objetos que puedan recibir PowerUps.
        // Precondición: El otro objeto debe implementar IPowerUpApplicable.
        IPowerUpApplicable target = other.GetComponent<IPowerUpApplicable>();
        if (target == null) return;

        ApplyTo(target);
        view?.PlayFeedback();
        Destroy(gameObject);
    }

    private void ApplyTo(IPowerUpApplicable target)
    {   // Propósito: Aplica el efecto del PowerUp según su tipo al objetivo.
        // Precondición: El objetivo no debe ser nulo.
        switch (model.powerUpType)
        {
            case PowerUpType.AttackBoost:
                target.ApplyAttackBoost(model.value, model.duration);
                break;
            case PowerUpType.Healing:
                target.ApplyHealing(model.value);
                break;
            case PowerUpType.SpeedBoost:
                target.ApplySpeedBoost(model.value, model.duration);
                break;
        }
    }
}
