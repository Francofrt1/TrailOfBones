using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public int maxHealth { get; private set; } = 100;
    public int currentHealth { get; private set; }
    public float baseAttackSpeed { get; private set; } = 1f;
    public float baseDamage { get; private set; } = 10f;
    public float currentAttackSpeed { get; private set; }
    public int currentShield { get; private set; } = 0;
    //If moveSpeed default changed, walk and run animation transitions must be changed too
    public float moveSpeed { get; private set; } = 5f;
    public float jumpForce { get; private set; } = 7f;
    public float acceleration { get; set; } = 1f;
    public List<GameObject> enemies { get; set; }

    private void Awake()
    {
        enemies = new List<GameObject>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        currentAttackSpeed = baseAttackSpeed;
    }

    public void SetHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void SetAttackSpeed(float multiplier, float duration)
    {
        //StopCoroutine("ResetAttackSpeed");
        currentAttackSpeed = baseAttackSpeed * multiplier;
        //StartCoroutine(ResetAttackSpeed(duration));
    }

    public void SetShield(int amount, float duration)
    {
        //StopCoroutine("RemoveShield");
        currentShield = amount;
        //StartCoroutine(RemoveShield(duration));
    }

    //private IEnumerator ResetAttackSpeed(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    currentAttackSpeed = baseAttackSpeed;
    //    Debug.Log("Attack Speed reset to base value.");
    //}


    //private IEnumerator RemoveShield(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    currentShield = 0;
    //    Debug.Log("Shield expired.");
    //}

    // Optional: Method to absorb damage using shield
    //public void TakeDamage(int damage)
    //{
    //    if (currentShield > 0)
    //    {
    //        int remainingDamage = damage - currentShield;
    //        currentShield -= damage;

    //        if (currentShield < 0)
    //            currentShield = 0;

    //        if (remainingDamage > 0)
    //            currentHealth -= remainingDamage;
    //    }
    //    else
    //    {
    //        currentHealth -= damage;
    //    }

    //    if (currentHealth <= 0)
    //    {
    //        currentHealth = 0;
    //        Debug.Log("Player died.");
    //    }
    //}
}