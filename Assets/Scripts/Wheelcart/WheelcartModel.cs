using UnityEngine;

public class WheelcartModel : MonoBehaviour
{
    public float duration = 60f;
    public float maxHealth = 1000f;
    public float currentHealth;
    public float distToBeAttacked { get; } = 5f;
    public void Start()
    {
        currentHealth = maxHealth;
    }
    public void SetHealth(float health)
    {
        currentHealth = health;
    }
}
