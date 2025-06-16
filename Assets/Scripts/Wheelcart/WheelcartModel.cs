using UnityEngine;

public class WheelcartModel : MonoBehaviour
{
    public float duration = 60f;
    public float maxHealth = 1000f;
    public float currentHealth { get; private set; }

    public int logStorage = 0;
    public const int logToRepair = 5; // cambiar este valor si se desea que se necesiten más o menos troncos para reparar el carro.

    public void Awake()
    {
        currentHealth = maxHealth;
    }
    public void SetHealth(float health)
    {
        currentHealth = health;
    }

    public float StopWheelcartPercent()
    {
        return maxHealth * 0.3f;
    }

    public void AddLog(int amount)
    {
        logStorage += amount;
    }

    public int LogsNeededToRepair()
    {
        return logToRepair - logStorage;
    }

    public void UseAllLogs()
    {
        logStorage = 0;
    }
}
