using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUpModel
{   
    public PowerUpType powerUpType; // Tipo de PowerUp (Curación, Ataque, Velocidad, etc.).
    public float value;             // Valor que representa el efecto del PowerUp (ej: +25 salud, x2 ataque, etc.).
    public float duration;          // // Duración del efecto si aplica (en segundos).


    public PowerUpModel(PowerUpType type, float value, float duration)
    {   // Propósito: Crear un modelo de PowerUp con tipo, valor y duración.
        // Precondición: value >= 0, duration >= 0
        this.powerUpType = type;
        this.value = value;
        this.duration = duration;
    }
}

public enum PowerUpType
{   // Enum que representa los distintos tipos de PowerUps disponibles.
    AttackBoost, // Aumenta el daño temporalmente.
    Healing,     // Recupera salud.
    SpeedBoost   // Aumenta la velocidad temporalmente.
}