using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerUpApplicable
{
    // Propósito: Aumenta el multiplicador de daño del jugador temporalmente.
    // Precondición: multiplier > 0, duration > 0
    void ApplyAttackBoost(float multiplier, float duration);

    // Propósito: Restaura una cantidad de vida al jugador.
    // Precondición: amount > 0
    void ApplyHealing(float amount);

    // Propósito: Aumenta la velocidad de movimiento del jugador temporalmente.
    // Precondición: bonus > 0, duration > 0
    void ApplySpeedBoost(float bonus, float duration);
}
