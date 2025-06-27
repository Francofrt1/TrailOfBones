using UnityEngine;

public class WheelcartModel : MonoBehaviour
{
    public float duration = 60f;
    public float maxHealth = 1000f;
    public float currentHealth { get; private set; }

    [SerializeField] private float stopWheelcartPercent = 0.8f;

    public int logStorage = 0;
    public const int logToRepair = 4;
    public float interactionDistance = 5.0f;

    public bool isInEvent = false;

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
        return maxHealth * stopWheelcartPercent;
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

    public int GetLogToRepair()
    {
        return logToRepair;
    }
}
