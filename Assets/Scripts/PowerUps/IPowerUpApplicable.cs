using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerUpApplicable
{
    // Prop�sito: Aumenta el multiplicador de da�o del jugador temporalmente.
    // Precondici�n: multiplier > 0, duration > 0
    void ApplyAttackBoost(float multiplier, float duration);

    // Prop�sito: Restaura una cantidad de vida al jugador.
    // Precondici�n: amount > 0
    void ApplyHealing(float amount);

    // Prop�sito: Aumenta la velocidad de movimiento del jugador temporalmente.
    // Precondici�n: bonus > 0, duration > 0
    void ApplySpeedBoost(float bonus, float duration);
}
