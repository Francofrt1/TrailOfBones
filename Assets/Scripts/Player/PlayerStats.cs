using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public float baseAttackSpeed = 1f;
    private float currentAttackSpeed;
    private int currentShield = 0;

    private void Start()
    {
        currentHealth = maxHealth;
        currentAttackSpeed = baseAttackSpeed;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Healed: " + amount + " | Current Health: " + currentHealth);
    }

    public void ModifyAttackSpeed(float multiplier, float duration)
    {
        StopCoroutine("ResetAttackSpeed");
        currentAttackSpeed = baseAttackSpeed * multiplier;
        StartCoroutine(ResetAttackSpeed(duration));
        Debug.Log("Attack Speed increased to: " + currentAttackSpeed);
    }

    private System.Collections.IEnumerator ResetAttackSpeed(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentAttackSpeed = baseAttackSpeed;
        Debug.Log("Attack Speed reset to base value.");
    }

    public void AddShield(int amount, float duration)
    {
        StopCoroutine("RemoveShield");
        currentShield = amount;
        StartCoroutine(RemoveShield(duration));
        Debug.Log("Shield applied: " + amount);
    }

    private System.Collections.IEnumerator RemoveShield(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentShield = 0;
        Debug.Log("Shield expired.");
    }

    // Optional: Method to absorb damage using shield
    public void TakeDamage(int damage)
    {
        if (currentShield > 0)
        {
            int remainingDamage = damage - currentShield;
            currentShield -= damage;

            if (currentShield < 0)
                currentShield = 0;

            if (remainingDamage > 0)
                currentHealth -= remainingDamage;
        }
        else
        {
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player died.");
        }
    }
}