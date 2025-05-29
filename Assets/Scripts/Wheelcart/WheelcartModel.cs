using UnityEngine;

public class WheelcartModel : MonoBehaviour
{
    public float duration = 60f;
    public float maxHealth = 1000f;
    public float currentHealth { get; private set; }
    public void Start()
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
}
