using UnityEngine;

public class WheelcartModel : MonoBehaviour
{
    [SerializeField]
    public float speed { get; private set; } = 2f;
    [SerializeField]
    public float maxHealth { get; private set; } = 100f;
    [SerializeField]
    public float currentHealth { get; private set; } = 100f;

    [SerializeField]
    public float distToBeAttacked { get; } = 5f;
    public void SetHealth(float health)
    {
        currentHealth = health;
    }
}
