using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EnemyModel
{
    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }
    public int attackDamage { get; private set; }
    public float attackCooldown { get; private set; }
    public float lastAttackTime { get; private set; }

    // enemy initialization
    public EnemyModel(int maxHealth, int attackDamage, float attackCooldown)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.attackDamage = attackDamage; 
        this.attackCooldown = attackCooldown;
        lastAttackTime = 0;
    }
    // applies damage to the enemy and triggers death if health reaches 0
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Console.WriteLine("Enemy died.");
    }

    public bool CanAttack(float time)
    {
        return time - lastAttackTime >= attackCooldown; // if enough time has passed then it can attack
    }

    public void Attack() // registers the current time as the moment of the last attack
    {
        lastAttackTime = Time.time;
    }
}
