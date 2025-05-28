using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public string ID { get; private set; }
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    public float baseDamage = 10f;
    public float attackCooldown = 2f;
    public float attackDurationAdjustment = -0.2f;
    public float movementSpeed = 3.5f;
    public bool inPlayer { get; set; } = false;

    // enemy initialization
    private void Awake()
    {
        ID = System.Guid.NewGuid().ToString();
    }
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetHealth(float amount)
    {
        currentHealth = amount;
    }
}
