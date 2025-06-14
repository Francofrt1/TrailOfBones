using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : NetworkBehaviour
{
    public string ID { get; private set; }
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
    public bool isDead { get; set; } = false;
    private void Awake()
    {
        ID = Guid.NewGuid().ToString();
        currentHealth = maxHealth;
        isDead = false;
        currentAttackSpeed = baseAttackSpeed;
    }

    public Vector3 CalculateLocalVelocity(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        return direction * moveSpeed * acceleration;
    }

    public void SetHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
    public void SetAttackSpeed(float multiplier, float duration)
    {
        currentAttackSpeed = baseAttackSpeed * multiplier;
    }

    public void SetShield(int amount, float duration)
    {
        currentShield = amount;
    }
}