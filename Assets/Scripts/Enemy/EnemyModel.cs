using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    public float baseDamage = 10f;
    public float attackCooldown = 2f;
    public float lastAttackTime = 0f;
    public bool inPlayer { get; set; } = false;

    // enemy initialization

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetHealth(float amount)
    {
        currentHealth = amount;
    }

    public bool CanAttack(float time)
    {
        return time - lastAttackTime >= attackCooldown; // if enough time has passed then it can attack
    }

    public void Attack() // registers the current time as the moment of the last attack
    {
        lastAttackTime = Time.time;
        Debug.Log($"Enemy attacked at time {lastAttackTime}"); // TESTEANDO
    }
}
