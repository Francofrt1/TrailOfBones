using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    public float baseAttackSpeed = 1f;
    public float baseDamage = 10f;
    public float currentAttackSpeed;
    public float currentShield = 0;
    //If moveSpeed default changed, walk and run animation transitions must be changed too
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float acceleration = 1f;
    public List<GameObject> enemies { get; set; }
    public bool isDead { get; set; } = false;
    public float distToBeAttacked { get; } = 2f;

    private void Awake()
    {
        enemies = new List<GameObject>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        currentAttackSpeed = baseAttackSpeed;
    }
    public void SetHealth(float amount)
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